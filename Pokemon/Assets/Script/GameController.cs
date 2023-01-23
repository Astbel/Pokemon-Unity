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
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        /*Trainer Battle*/
        playerController.OnEnterTrainerView += (Collider2D trainerCollider) =>
        {
            var tranier = trainerCollider.GetComponentInParent<TrainerController>();
            if (tranier != null)
            {
                state = GameState.Cutscene;
                StartCoroutine(tranier.TriggerTrainerBattle(playerController));
            }
        };

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

    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();

        battleSystem.StartBattle(playerParty, wildPokemon);

    }
    /*Trainer Battle*/
    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        var trainerParty = trainer.GetComponent<PokemonParty>();

        battleSystem.StartTrainerBattle(playerParty, trainerParty);

    }


    void EndBattle(bool won)
    {
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
