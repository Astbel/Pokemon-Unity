using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*草叢*/
public class MapArea : MonoBehaviour
{
    [SerializeField] List<Pokemon> wildPokemons;

    /*開始時為所有pokemon初始化狀態 初始化招式以及血量*/
    public Pokemon GetRandomWildPokemon()
    {
        var wildPokemon = wildPokemons[Random.Range(0, wildPokemons.Count)];
        wildPokemon.Init();
        return wildPokemon;
    }

}
