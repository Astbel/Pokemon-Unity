using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*草叢*/
public class MapArea : MonoBehaviour
{
    [SerializeField] List<PokemonEncounterRecord> wildPokemons;
    [SerializeField] List<PokemonEncounterRecord> wildPokemonsInWater;
    //totalChance 為了可以被調用宣告成SerializeField ,為了Insoector不要看到而hide起來
    [HideInInspector]
    [SerializeField] int totalChance = 0;
    [HideInInspector]
    [SerializeField] int totalChance_Water = 0;
    //驗證是否遇敵機率是否滿足100
    private void OnValidate()
    {
        //草叢
        totalChance = 0;
        foreach (var record in wildPokemons)
        {
            record.chanceLower = totalChance;
            record.chanceUpper = totalChance + record.chanceRate;

            totalChance = totalChance + record.chanceRate;
        }
        //水
        totalChance_Water = 0;
        foreach (var record in wildPokemonsInWater)
        {
            record.chanceLower = totalChance_Water;
            record.chanceUpper = totalChance_Water + record.chanceRate;

            totalChance_Water = totalChance_Water + record.chanceRate;
        }
    }

    private void Start()
    {

    }

    /*開始時為所有pokemon初始化狀態 初始化招式以及血量*/
    public Pokemon GetRandomWildPokemon(BattleTrigger trigger)
    {
        var pokemonList = (trigger == BattleTrigger.LongGrass) ? wildPokemons : wildPokemonsInWater;
        int randVal = Random.Range(1, 101);
        var pokemonRecord = pokemonList.First(p => randVal >= p.chanceLower && randVal <= p.chanceUpper);

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
