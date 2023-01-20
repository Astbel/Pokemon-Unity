using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;

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

}
