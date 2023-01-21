using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayAction, PlayerMove, EnemyMove, Busy, Party }
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    public event Action<bool> OnBattleOver;

    BattleState state;  //回合制狀態機
    int currentAction; //選單變數偵測
    int currentMove; //技能選單變數偵測
    int currentMember;//party 選單中變數

    /*物件畫 玩家以及野生*/
    PokemonParty playerParty;
    Pokemon wildPokemon;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        StartCoroutine(SetUpBattle());
    }
    /*
    修正為 IEnumerator經過顯示遇敵資訊後,再載入選單界面窗
    player端檢測Pokemon是否HP還大於0
    */
    public IEnumerator SetUpBattle()
    {
        playerUnit.Setup(playerParty.GetHealthyPokemon());
        playerHud.SetData(playerUnit.Pokemon);

        enemyUnit.Setup(wildPokemon);
        enemyHud.SetData(enemyUnit.Pokemon);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name}  appeared.");

        PlayAction(); /*初始狀態*/

    }
    /*玩家狀態 回合制設定*/
    void PlayAction()
    {
        state = BattleState.PlayAction;
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true);
    }
    //選單選擇戰鬥中其他Pokemon
    void OpenPartyScreen()
    {
        state = BattleState.Party;
        /*回傳player的pokemon list*/
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    /*選技能階段*/
    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    /*對話介面顯示玩家技能*/
    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy; //避免腳色在這時候可以選技能

        var move = playerUnit.Pokemon.Moves[currentMove];
        //當使用技能時要減少PP
        move.PP--;
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.Name}");
        /*攻擊動畫以及敵人受傷動畫*/
        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        enemyUnit.PlayHitAnimation();
        /*計算傷害*/
        var damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        /*跟新Hp*/
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);
        /*判定是否陣亡,如果陣亡等待兩秒執行結束戰鬥回合*/
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} Fainted");
            enemyUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }
    /*敵人動作*/
    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Pokemon.GetRandomMove();
        //當使用技能時要減少PP
        move.PP--;
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} used {move.Base.Name}");
        /*攻擊動畫以及玩家受傷動畫*/
        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        playerUnit.PlayHitAnimation();
        /*計算傷害*/
        var damageDetails = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
        /*跟新Hp*/
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);
        /*判定是否陣亡,如果陣亡等待兩秒執行結束戰鬥回合*/
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} Fainted");
            playerUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            /*檢查玩家是否還有其他能用的Pokemon,如果有責設置下一隻的資料並顯示在對話框上*/
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
            {
                OpenPartyScreen();
            }
            else
            {
                OnBattleOver(false);
            }
        }
        else
        {
            PlayAction();
        }

    }
    /*show damage detail*/
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A Critical hit!");
        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("It's not very effective...");

        // Debug.Log(damageDetails.Critical);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    public void HandleUpdate()
    {
        if (state == BattleState.PlayAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.Party)
        {
            HandlePartySelection();
        }

    }
    /*Player 選擇動作*/
    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentAction -= 2;
        }
        /*限制選單賦值0~3*/
        currentAction = Math.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //Fight
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                //Bag
            }
            else if (currentAction == 2)
            {
                //Party
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                //run
            }
        }

    }
    /*技能選單有一個可能是pokemon的技能萬一不足四個
    技能在陣列中的排序定義如下
    0 1
    2 3
    所以腳色在左和右時只會對陣列增加1
    當為上下移動時陣列只會增加為2
    */
    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMove -= 2;
        }
        /*限制選單賦值由於限制跟腳色技能樹有關所以參照玩家pokemon擁有技能上線做定義*/
        currentMove = Math.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);
        //Debug.Log(currentMove);
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);
        /*當選擇技能時*/
        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
        /*退出案件*/
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            PlayAction();
        }
    }

    /*隊伍選單*/
    /*陣列如下所以在上下Key則是增加為2
      0  1
      2  3
      4  5
    */
    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentMember;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentMember;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMember += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMember -= 2;
        }
        /*限制選單賦值由於限制跟腳色技能樹有關所以參照玩家pokemon擁有技能上線做定義*/
        currentMember = Math.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        /*選擇腳色*/
        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Pokemons[currentMember];
            //不能選擇昏厥pokemon
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("You can't send a fainted Pokemon");
                return;
            }
            //不能選擇已經出場的pokemon
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText("You can't switch with the same Pokemon");
                return;
            }
            /*當選單結束時關閉選單*/
            partyScreen.gameObject.SetActive(false);
            /*狀態改為busy避免玩家一直A造成誤動作*/
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));

        }
        /*返回鍵*/
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            PlayAction();
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        /*確認玩家pokemon是否HP大於0才播放切換*/
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back{playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        /*替換寶可夢*/
        playerUnit.Setup(newPokemon);
        playerHud.SetData(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"I Chose you {newPokemon.Base.Name}!.");
        /*換敵人的回合*/
        StartCoroutine(EnemyMove());
    }

}
