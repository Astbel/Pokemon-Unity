using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayAction, PlayerMove, EnemyMove, Busy }
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    public event Action<bool> OnBattleOver;

    BattleState state;  //回合制狀態機
    int currentAction; //選單變數偵測
    int currentMove; //技能選單變數偵測
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    public void StartBattle()
    {
        StartCoroutine(SetUpBattle());
    }
    /*
    修正為 IEnumerator經過顯示遇敵資訊後,再載入選單界面窗
    */
    public IEnumerator SetUpBattle()
    {
        playerUnit.Setup();
        playerHud.SetData(playerUnit.Pokemon);

        enemyUnit.Setup();
        enemyHud.SetData(enemyUnit.Pokemon);

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name}  appeared.");

        PlayAction(); /*初始狀態*/

    }
    /*玩家狀態 回合制設定*/
    void PlayAction()
    {
        state = BattleState.PlayAction;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
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
            OnBattleOver(false);
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
    }
    /*Player 選擇動作*/
    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
                --currentAction;
        }

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
            if (currentMove < playerUnit.Pokemon.Moves.Count - 1)
                ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }
        //Debug.Log(currentMove);
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);
        /*當選擇技能時*/
        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }

    }



}
