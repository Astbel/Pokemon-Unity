using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionManager : MonoBehaviour
{
    [SerializeField] GameObject evolutionUI;
    [SerializeField] Image pokemonImage;
    /*為了讓控制器得知狀態這邊需要製作一個委派通知*/
    public event Action OnStartEvolution;
    public event Action OnCompleteEvolution;
    public static EvolutionManager i { get; private set; }

    /*這邊awake只是為了讓他靜態能在其他地方被呼叫*/
    private void Awake()
    {
        i = this;
    }

    public IEnumerator Evolve(Pokemon pokemon, Evolution evolution)
    {
        OnStartEvolution?.Invoke();
        evolutionUI.SetActive(true);
        /*這邊的圖像會是要進化的寶可夢*/
        pokemonImage.sprite = pokemon.Base.FrontSprite;
        yield return DialogManger.Instance.ShowDialogText($"{pokemon.Base.Name} is evolving ");
        /*進化後因原本的class消失為了讓字串顯示在這儲存要進化的pokemon明子*/
        var oldPokemon = pokemon.Base;
        pokemon.Evolve(evolution);
        /*這邊則是要進化的對象圖像*/
        pokemonImage.sprite = pokemon.Base.FrontSprite;
        yield return DialogManger.Instance.ShowDialogText($"{oldPokemon.Name} evolvedinto {pokemon.Base.Name} ");

        evolutionUI.SetActive(false); 
        OnCompleteEvolution?.Invoke();

    }




}
