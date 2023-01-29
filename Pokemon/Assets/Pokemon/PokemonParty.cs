using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;
    //回傳至給Party UI 來顯示

    public event Action OnUpdated;

    public List<Pokemon> Pokemons
    {
        get { return pokemons; }
        set { pokemons = value; OnUpdated?.Invoke(); }
    }

    /*開始時為所有pokemon初始化狀態 初始化招式以及血量*/
    private void Start()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }

    /*確認玩家的pokemon是否有昏厥*/
    public Pokemon GetHealthyPokemon()
    {
        /*檢查玩家第一支pokemon血量是否大於0*/
        return pokemons.Where(x => x.HP > 0).FirstOrDefault();
    }

    /*加入寶可夢*/
    public void AddPokemon(Pokemon newPokemon)
    {
        if (pokemons.Count < 6)
        {
            pokemons.Add(newPokemon);
            OnUpdated?.Invoke();
        }
        else
        {
            /*Add a pc for implemented*/
        }
    }
    /*宣告一個靜態method來獲得腳色隊伍*/
    public static PokemonParty GetPlayerParty()
    {
        return FindObjectOfType<PlayerController>().GetComponent<PokemonParty>();
    }


}
