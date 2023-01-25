using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartySelection, AboutToUse, BattleOver }
public enum BattleAction { Move, SwitchPokemon, UseItem, Run }
public class BattleSystem : MonoBehaviour
{
    /*Trainer Player */
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    /*player pokemon & wild pokemon*/
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] GameObject pokeballSprite;

    public event Action<bool> OnBattleOver;
    BattleState state;  //回合制狀態機
    BattleState? prevState;  //回合制狀態機
    int currentAction; //選單變數偵測
    int currentMove; //技能選單變數偵測
    int currentMember;//party 選單中變數
    /*確認是否為戰鬥狀態*/
    bool isTrainerBattle = false;
    bool aboutToUseChoice = true;
    /*物件畫 玩家以及野生*/
    PokemonParty playerParty;
    PokemonParty trainerParty;
    Pokemon wildPokemon;

    PlayerController player;
    TrainerController trainer;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerController>();

        StartCoroutine(SetUpBattle());
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;

        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetUpBattle());
    }

    /*
    修正為 IEnumerator經過顯示遇敵資訊後,再載入選單界面窗
    player端檢測Pokemon是否HP還大於0
    */
    public IEnumerator SetUpBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();
        if (!isTrainerBattle)
        {
            /*Wild Pokemon Battle*/
            playerUnit.Setup(playerParty.GetHealthyPokemon());

            enemyUnit.Setup(wildPokemon);

            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

            yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name}  appeared.");
        }
        else
        {
            /*Tranier Battle*/
            //disable both pokemon image
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);

            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;

            yield return dialogBox.TypeDialog($"{trainer.Name} wants to battle");
            //Send out first Pokemon of the trainer
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = trainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);
            yield return dialogBox.TypeDialog($"{trainer.Name} send out {enemyPokemon.Base.Name} ");
            //Send out first Pokemon of the player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPokemon = playerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPokemon);
            yield return dialogBox.TypeDialog($"Go  {playerPokemon.Base.Name} !");
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        }

        partyScreen.Init();
        /*確認誰先攻*/
        ActionSelection();

    }
    /*確認戰鬥是否結束*/
    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        //使用link來指向玩家所有pokemon來進行清除reset buff
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }

    /*玩家狀態 回合制設定*/
    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog($"What will {playerUnit.Pokemon.Base.Name} do now");
        dialogBox.EnableActionSelector(true);
    }
    //選單選擇戰鬥中其他Pokemon
    void OpenPartyScreen()
    {
        state = BattleState.PartySelection;
        /*回傳player的pokemon list*/
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    /*選技能階段*/
    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }
    /*是否替換pokemon*/
    IEnumerator AboutToUse(Pokemon newPokemon)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"{trainer.Name} is about to use {newPokemon.Base.Name}. Do you want to change Pokemon ? ");
        state = BattleState.AboutToUse;
        dialogBox.EnableChoiceBox(true);

    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();
            /*設置優先權*/
            int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;
            //確認哪隻先
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if (enemyMovePriority == playerMovePriority)
                playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;


            //回傳誰先
            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondaryPokemon = secondUnit.Pokemon;

            //第一回合先動的人
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            //確保整個回合結束當State整個結束才算是這個回合結束
            if (state == BattleState.BattleOver) yield break;

            if (secondaryPokemon.HP > 0)
            {
                //第一回合後面動作
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                //確保整個回合結束當State整個結束才算是這個回合結束
                if (state == BattleState.BattleOver) yield break;
            }
        }
        /*如果玩家切換*/
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedMember = playerParty.Pokemons[currentMember];
                state = BattleState.Busy;    /*狀態改為busy避免玩家一直A造成誤動作*/
                yield return SwitchPokemon(selectedMember);
            }
            //使用道具
            else if(playerAction==BattleAction.UseItem)
            {
                dialogBox.EnableActionSelector(false);
                yield return ThrowPokeBall();
            }
            //Enemy Turn
            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            //確保整個回合結束當State整個結束才算是這個回合結束
            if (state == BattleState.BattleOver) yield break;
        }
        if (state != BattleState.BattleOver)
            ActionSelection();
    }
    /*Move function for player and enemy*/
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        /*檢測是否種異常狀態 冰凍 麻痺 睡眠 如果回false則直接從協程break出來*/
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        Debug.Log(canRunMove);
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.UpdateHP();
            yield break;
        }
        /*狀態異常例如麻痺有可能可以放技能但還是要打印訊息出來*/
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        //當使用技能時要減少PP
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");
        //確認招式是否命中
        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            /*攻擊動畫以及敵人受傷動畫*/
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();
            /*判斷是否是status技能還是傷害技能*/
            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move.Base.Effects, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
            }
            else  /*傷害技能*/
            {
                /*計算傷害*/
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);

                /*跟新Hp*/
                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageDetails(damageDetails);
            }
            /*招式異常狀態檢測,確認招式屬性是否大於0*/
            if (move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0 && targetUnit.Pokemon.HP > 0)
            {
                foreach (var secondary in move.Base.SecondaryEffects)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                }
            }
            /*判定是否陣亡,如果陣亡等待兩秒執行結束戰鬥回合*/
            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} Fainted");
                targetUnit.PlayFaintAnimation();
                yield return new WaitForSeconds(2f);

                CheckForBattleOver(targetUnit);

            }
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}'s attack missed");
        }
    }
    /*狀態Buff*/
    IEnumerator RunMoveEffects(MoveEffects effects, Pokemon source, Pokemon target, MoveTarget moveTarget)
    {
        //確認是否有提升或降低能力
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                source.ApplyBoost(effects.Boosts);  //提升我方
            else
                target.ApplyBoost(effects.Boosts);  //降低敵方
        }
        //異常狀態
        if (effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }
        //混亂狀態
        if (effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }
        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }
    /*狀態異常在回合結束後才執行*/
    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        /*戰鬥結束後不執行*/
        if (state == BattleState.BattleOver) yield break;
        /*避免在選單過程中狀態機位切換回去敵人又動作,等待狀態機再切回RunningTurn*/
        yield return new WaitUntil(() => state == BattleState.RunningTurn);
        //中毒 燒傷會在每一回合觸發然後種異常狀態的pokemon有可能昏厥
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.UpdateHP();
        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} Fainted");
            sourceUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }




    /*招式命中率*/
    bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        /*必中技能直接回傳true不計算命中迴避*/
        if (move.Base.AlwaysHits)
            return true;
        /*計算命中率*/
        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatsBoost[Stat.Accuracy];
        int evasion = source.StatsBoost[Stat.Evasion];

        var boostValue = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };
        //提升或降低命中
        if (accuracy > 0)
            moveAccuracy *= boostValue[accuracy];
        else
            moveAccuracy /= boostValue[-accuracy];
        //提升或降低迴避
        if (evasion > 0)
            moveAccuracy /= boostValue[evasion];
        else
            moveAccuracy *= boostValue[-evasion];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    /*確認昏厥pokemon是否為玩家如果是玩家在檢測是否還有剩餘pokemon,如果有擇繼續沒有停止場景*/
    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
        {
            if (!isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                    StartCoroutine(AboutToUse(nextPokemon));
                else
                    BattleOver(true);
            }
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
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartySelection)
        {
            HandlePartySelection();
        }
        else if (state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
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
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                //Bag
                StartCoroutine(RunTurns(BattleAction.UseItem));
            }
            else if (currentAction == 2)
            {
                //Party
                prevState = state;
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
            //當pp等於0無法使用技能
            var move = playerUnit.Pokemon.Moves[currentMove];
            if (move.PP == 0) return;

            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        /*退出案件*/
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
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

            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                /*狀態改為busy避免玩家一直A造成誤動作*/
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }
        }
        /*返回鍵*/
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (playerUnit.Pokemon.HP <= 0)
            {
                partyScreen.SetMessageText("You have to choose a Pokemon to continue ");
                return;
            }
            partyScreen.gameObject.SetActive(false);

            if (prevState == BattleState.AboutToUse)
            {
                prevState = null;
                StartCoroutine(SendNextPokemon());
            }
            else
                ActionSelection();
        }
    }

    /*替換pokemon*/
    void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            aboutToUseChoice = !aboutToUseChoice;

        dialogBox.UpdateChoiceBox(aboutToUseChoice);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            /*Yes 進入選擇畫面*/
            if (aboutToUseChoice == true)
            {
                dialogBox.EnableChoiceBox(false);
                prevState = BattleState.AboutToUse;
                OpenPartyScreen();
            }
            /*NO 繼續戰鬥畫面*/
            else
            {
                dialogBox.EnableChoiceBox(false);
                StartCoroutine(SendNextPokemon());
            }

        }
        /*取消切換 Trainer直接送pkemon*/
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableChoiceBox(false);
            StartCoroutine(SendNextPokemon());
        }
    }



    /*切換部分
    1.只有HP>0才可以切換
    2.如果昏厥的話要再次比較哪知pokemon的基礎速度點高才決定誰先攻
    */
    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        /*確認玩家pokemon是否HP大於0才播放切換*/
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }
        /*替換寶可夢*/
        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"I Chose you {newPokemon.Base.Name}!.");
        /*把狀態在此轉回RunningTurn,讓RunningTurn來決定誰是這回合先動作*/
        if (prevState == null)
        {
            state = BattleState.RunningTurn;
        }
        else if (prevState == BattleState.AboutToUse)
        {
            prevState = null;
            StartCoroutine(SendNextPokemon());
        }

    }

    /*提升降低狀態Buff訊息*/
    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        //檢查queue裡面的訊息
        while (pokemon.StatusChanges.Count > 0)
        {
            /*Dequeue來顯示訊息*/
            var message = pokemon.StatusChanges.Dequeue();
            /*顯示該訊息*/
            yield return dialogBox.TypeDialog(message);

        }
    }
    /*Trainer send pokemon*/
    IEnumerator SendNextPokemon()
    {
        /*等待trainer替換*/
        state = BattleState.Busy;

        var nextPokemon = trainerParty.GetHealthyPokemon();
        enemyUnit.Setup(nextPokemon);

        yield return dialogBox.TypeDialog($"{trainer.Name} send out {nextPokemon.Base.Name} ");
        /*回到戰鬥階段*/
        state = BattleState.RunningTurn;
    }

    /*丟寶貝球*/
    IEnumerator ThrowPokeBall()
    {
        state = BattleState.Busy;
        /*Trainer Battle 不能捕捉對方pokemon*/
        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($" You can't steal the trainers pokemon ! ");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return dialogBox.TypeDialog($"{player.Name} used POKEBALL ! ");

        var pokeballObj = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();

        //Pokeball丟動畫
        yield return pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 5), 1f, 1, 1f).WaitForCompletion();
        //呼叫捕捉畫面
        yield return enemyUnit.PlayCaptureAnimation();
        //pokeball掉下
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 4f, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatchPokemon(enemyUnit.Pokemon);
        //pokeball搖動 z軸在2D只會有旋轉的效果運用條動Z軸來達到搖動效果
        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            /*每一次球晃動等待0.5Sec*/
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }
        /*當搖晃為四次捕捉成功*/
        if (shakeCount == 4)
        {
            //pkemon caught
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} was caught ! ");
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();

            playerParty.AddPokemon(enemyUnit.Pokemon);
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} has been add to your party ! ");

            Destroy(pokeball);
            BattleOver(true);

        }
        else
        {
            //pokemon broke
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            if (shakeCount < 2)
                yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} broke free ! ");
            else
                yield return dialogBox.TypeDialog($"Almost caught it ! ");
            Destroy(pokeball);
            state = BattleState.RunningTurn;
        }

    }
    /*捕捉率來源Wiki*/
    int TryToCatchPokemon(Pokemon pokemon)
    {
        float a = (3 * pokemon.MaxHp - 2 * pokemon.HP) * pokemon.Base.CatchRate * ConditionDB.GetStatusBouns(pokemon.Status) / (3 * pokemon.MaxHp);

        if (a >= 255)
            return 4;

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;

        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;

            ++shakeCount;

        }
        return shakeCount;
    }

}
