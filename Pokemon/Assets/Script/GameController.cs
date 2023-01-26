using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscene }

/*
判斷遊戲狀態目前為誰控制避免同時腳色移動
*/
public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    TrainerController trainer;

    /*Audio Source */
    GameState state;
    /*Game start init*/
    private void Awake()
    {
        Instance = this;
        ConditionDB.Init();
    }

    private void Start()
    {
      
        battleSystem.OnBattleOver += EndBattle;

        /*Lamda function*/
        DialogManger.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManger.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };



    }

    public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();

        var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);

        battleSystem.StartBattle(playerParty, wildPokemonCopy);

    }
    /*Trainer Battle*/
    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        this.trainer = trainer;

        var playerParty = playerController.GetComponent<PokemonParty>();
        var trainerParty = trainer.GetComponent<PokemonParty>();

        battleSystem.StartTrainerBattle(playerParty, trainerParty);

    }

    public void OnEnterTrainerView(TrainerController trainer)
    {
        state = GameState.Cutscene;
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
    }

    void EndBattle(bool won)
    {
        if (trainer != null && won == true)
        {
            trainer.BattleLost();
            trainer = null;
        }

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    // Update is called once per frame
    private void Update()
    {
        //player on map
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        //player at battle
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManger.Instance.HandleUpdate();
        }

    }
}
