using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*草叢*/
public class MapArea : MonoBehaviour
{
    [SerializeField] List<PokemonEncounterRecord> wildPokemons;

    private void Start()
    {
        int totalChance = 0;
        foreach (var record in wildPokemons)
        {
            record.chanceLower = totalChance;
            record.chanceUpper = totalChance + record.chanceRate;

            totalChance = totalChance + record.chanceRate;
        }
    }

    /*開始時為所有pokemon初始化狀態 初始化招式以及血量*/
    public Pokemon GetRandomWildPokemon()
    {
        int randVal = Random.Range(1, 101);
        var pokemonRecord = wildPokemons.First(p => randVal >= p.chanceLower && randVal <= p.chanceUpper);

        var levelRange = pokemonRecord.levelRange;
        int level = levelRange.y == 0 ? levelRange.x : Random.Range(levelRange.x, levelRange.y + 1);
        var wildPokemon = new Pokemon(pokemonRecord.pokemon, level);
        
        wildPokemon.Init();
        return wildPokemon;
    }

}
[System.Serializable]
public class PokemonEncounterRecord
{
    public PokemonBase pokemon;
    public Vector2Int levelRange;
    public int chanceRate;

    public int chanceLower { get; set; }
    public int chanceUpper { get; set; }
}
