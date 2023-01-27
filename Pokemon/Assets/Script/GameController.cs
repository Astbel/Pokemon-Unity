using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscene, Paused }

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

    GameState stateBeforePause;
    GameState state;

    public SceneDetail CurrentScene { get; private set; }

    public SceneDetail PrevScene { get; private set; }

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
    /*切換場景停止遊戲控制*/
    public void PausedGame(bool pause)
    {
        if (pause)
        {
            stateBeforePause = state;
            state = GameState.Paused;
        }
        else
        {
            state = stateBeforePause;

        }
    }

    public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PokemonParty>();
        /*由於切換場景的關西FindObjectOfType只會load第一章地圖,用CurrentScene來切換對應地圖遇到的怪物*/
        var wildPokemon = CurrentScene.GetComponent<MapArea>().GetRandomWildPokemon();

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
            /*測試存檔讀檔*/
            if (Input.GetKeyDown(KeyCode.S))
            {
                SavingSystem.i.Save("SavingSolt1");
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                SavingSystem.i.Load("SavingSolt1");
            }
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

    public void SetCurrentScene(SceneDetail currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }

}
