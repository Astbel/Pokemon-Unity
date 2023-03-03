using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Menu, PartyScreen, Bag, Cutscene, Paused, Evolution }

/*
判斷遊戲狀態目前為誰控制避免同時腳色移動
*/
public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] InventoryUI inventoryUI;
    TrainerController trainer;
    MenuController menuController;
    GameState prevState;
    GameState state;
    GameState stateBeforeEvolution;

    public GameState State => state;

    public SceneDetail CurrentScene { get; private set; }

    public SceneDetail PrevScene { get; private set; }

    /*Game start init*/
    private void Awake()
    {
        /*取消使用滑鼠*/
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        /**/
        Instance = this;
        menuController = GetComponent<MenuController>();
        QuestDB.Init();
        PokemonDB.Init();
        MoveDB.Init();
        ConditionDB.Init();
        ItemDB.Init();
    }

    private void Start()
    {

        battleSystem.OnBattleOver += EndBattle;
        partyScreen.Init();
        /*Lamda function*/
        DialogManger.Instance.OnShowDialog += () =>
        {
            prevState = state;
            state = GameState.Dialog;
        };

        DialogManger.Instance.OnDialogFinished += () =>
        {
            if (state == GameState.Dialog)
                state = prevState;
        };

        menuController.closeMenuSelected += () =>
        {
            state = GameState.FreeRoam;
        };
        menuController.onMenuSelected += OnMenuSelect;
        /*進化事件新增儲存狀態因為在使用進化石會有卡住狀態的問題*/
        EvolutionManager.i.OnStartEvolution += () =>
         {
            stateBeforeEvolution =state;
            state = GameState.Evolution;
         };
        EvolutionManager.i.OnCompleteEvolution += () => 
        {
            partyScreen.SetPartyData();
            state = stateBeforeEvolution;
        };
    }
    /*切換場景停止遊戲控制*/
    public void PausedGame(bool pause)
    {
        if (pause)
        {
            prevState = state;
            state = GameState.Paused;
        }
        else
        {
            state = prevState;

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

        partyScreen.SetPartyData();

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
        /*結束戰鬥切換場景BGM*/
        AudioManager.i.PlayMusic(CurrentScene.SceneMusic, fade: true);

        /*確認是否進化*/
        var playerParty = playerController.GetComponent<PokemonParty>();
        StartCoroutine(playerParty.CheckForEvolutions());
    }

    // Update is called once per frame
    private void Update()
    {
        //player on map
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
            /*開啟選單*/
            if (Input.GetKeyDown(KeyCode.Return))
            {
                menuController.OpenMenu();
                state = GameState.Menu;
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
        else if (state == GameState.Menu)
        {
            menuController.HandleUpdate();
        }
        else if (state == GameState.PartyScreen)
        {
            Action onSelected = () =>
            {

            };
            Action onBack = () =>
            {
                partyScreen.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };

            partyScreen.HandleUpdate(onSelected, onBack);
        }
        /*開啟包包選單*/
        else if (state == GameState.Bag)
        {
            Action onBack = () =>
            {
                inventoryUI.gameObject.SetActive(false);
                state = GameState.FreeRoam;
            };

            inventoryUI.HandleUpdate(onBack);
        }
    }

    public void SetCurrentScene(SceneDetail currScene)
    {
        PrevScene = CurrentScene;
        CurrentScene = currScene;
    }

    void OnMenuSelect(int selectedItem)
    {
        if (selectedItem == 0)
        {
            //Pokemon
            partyScreen.gameObject.SetActive(true);
            state = GameState.PartyScreen;
        }
        else if (selectedItem == 1)
        {
            //Bag
            inventoryUI.gameObject.SetActive(true);
            state = GameState.Bag;
        }
        else if (selectedItem == 2)
        {
            //Save
            SavingSystem.i.Save("SavingSolt1");
            state = GameState.FreeRoam;
        }
        else if (selectedItem == 3)
        {
            //Load
            SavingSystem.i.Load("SavingSolt1");
            state = GameState.FreeRoam;
        }

    }

}
