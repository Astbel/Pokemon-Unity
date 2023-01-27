using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonDB
{
    static Dictionary<string, PokemonBase> pokemons;

    public static void Init()
    {
        pokemons = new Dictionary<string, PokemonBase>();

        var pokemonArray = Resources.LoadAll<PokemonBase>("");
        foreach (var pokemon in pokemonArray)
        {
            /*檢測是否陣列中有相同的pokemon,大寫N跟小寫n差別在Resource查找物件時
            大寫N->Script名稱
            小寫n->物件名稱
            */
            if (pokemons.ContainsKey(pokemon.Name))
            {
                Debug.LogError($"There are two pokemons with the name {pokemon.Name}");
                continue;
            }
            pokemons[pokemon.Name] = pokemon;
        }

    }


    public static PokemonBase GetPokemonByName(string name)
    {
        if (!pokemons.ContainsKey(name))
        {
            Debug.LogError($"Pokemons with the name {name} not found in the database");
            return null;
        }
        return pokemons[name];
    }
}
