using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    public PokemonBase Base { get { return _base; } }
    public int Exp { get; set; }
    public int Level { get { return level; } }
    public int HP { get; set; }
    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    //預設狀態
    public Dictionary<Stat, int> Stats { get; private set; }
    //狀態提升
    public Dictionary<Stat, int> StatsBoost { get; private set; }
    //用來顯示特殊異常狀態
    public Condition Status { get; private set; }
    //用於混亂狀態
    public Condition VolatileStatus { get; private set; }
    //用於顯示腳色Buff
    public Queue<string> StatusChanges { get; private set; } = new Queue<string>();
    //睡眠異常狀態計數
    public int StatusTime { get; set; }
    //混亂回合計數
    public int VolatileStatusTime { get; set; }
    public event System.Action OnStatusChanged;
    public event System.Action OnHpChanged;
    public Pokemon(PokemonBase pBase, int plevel)
    {
        _base = pBase;
        level = plevel;

        Init();
    }


    public void Init()
    {
        /*學習技能檢查是否有在該腳色List中*/
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.MoveBase));
            }
            /*當技能超過4則break*/
            if (Moves.Count >= PokemonBase.MaxNumOfMoves)
            {
                break;
            }
        }
        CalculateStat();
        HP = MaxHp;
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Exp = Base.GetExpForLevel(Level);

        /*清除所有異常狀態*/
        Status = null;
        VolatileStatus = null;
    }
    /*inital 初始狀態IV數值計算*/
    void CalculateStat()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10 + Level;

    }
    /*清除狀態*/
    void ResetStatBoost()
    {
        /*開場清除提升狀態*/
        StatsBoost = new Dictionary<Stat, int>()
        {
            {Stat.Attack,0},
            {Stat.Defense,0},
            {Stat.SpAttack,0},
            {Stat.SpDefense,0},
            {Stat.Speed,0},
            {Stat.Accuracy,0},
            {Stat.Evasion,0},
        };
    }
    /*確認是否升等*/
    public bool CheckForLevelUp()
    {
        if (Exp > Base.GetExpForLevel(level + 1))
        {
            ++level;
            return true;
        }
        return false;
    }
    /*確認升等要學招式*/
    public LearnableMove GetLearnableMoveAtCurLevel()
    {
        return Base.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
    }
    /*學習招式*/
    public void LearnMove(LearnableMove moveToLearn)
    {
        /*當超過4個時候*/
        if (Moves.Count > PokemonBase.MaxNumOfMoves)
            return;

        Moves.Add(new Move(moveToLearn.MoveBase));
    }

    /*計算提升數值技能*/
    int GetStat(Stat stat)
    {
        int statVal = Stats[stat];
        //取得目前提升狀態
        int boost = StatsBoost[stat];
        var boostValue = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        //確認是提升還是遞減
        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValue[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValue[-boost]);

        return statVal;
    }

    public void ApplyBoost(List<StatBoost> statBoosts)
    {
        foreach (var value in statBoosts)
        {
            var stat = value.stat;
            var boost = value.boost;

            /*提升狀態 clamp狀態最多上下是6倍*/
            StatsBoost[stat] = Mathf.Clamp(StatsBoost[stat] + boost, -6, 6);

            /*顯示腳色提升或降低狀態Buff訊息*/
            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat}  rose!");
            else
                StatusChanges.Enqueue($"{Base.Name}'s {stat}  fell!");
            /*Debug 訊息來確認是否有提升跟降低狀態*/
            Debug.Log($"{stat} has been boosted to {StatsBoost[stat]}");
        }
    }


    public int Attack
    {
        get { return GetStat(Stat.Attack); }
    }

    public int Defense
    {
        get { return GetStat(Stat.Defense); }
    }

    public int SpAttack
    {
        get { return GetStat(Stat.SpAttack); }
    }

    public int SpDefense
    {
        get { return GetStat(Stat.SpDefense); }
    }

    public int Speed
    {
        get { return GetStat(Stat.Speed); }
    }

    public int MaxHp
    {
        get;
        private set;
    }
    /*pokemon 傷害公式*/
    public DamageDetails TakeDamage(Move move, Pokemon attacker)
    {
        /*Crit hit*/
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;
        /*Effect type method*/
        float type = TypeChart.GetEffectivenss(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectivenss(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };
        /*判斷是否為特攻或是物攻,如果是true就用SP attack*/
        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        /*判斷是否為特防*/
        float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;
        /*damage formula*/
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);
        Debug.Log(damage);
        DecreaseHP(damage);

        return damageDetails;
    }
    /*跟新HP*/
    public void DecreaseHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        OnHpChanged?.Invoke();
    }
    public void InecreaseHP(int amount)
    {
        HP = Mathf.Clamp(HP + amount, 0, MaxHp);
        OnHpChanged?.Invoke();
    }

    /*中毒燒傷呼叫使用語法需要再查明*/
    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }
    /*狀態異常回復所以清除狀態*/
    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public bool OnBeforeMove()
    {
        bool canPerformMove = true;
        /*麻痺 冰凍 睡眠*/
        if (Status?.OnBeforeTurn != null)
        {
            /*確認是否為false,由於多一個混亂型別所以直接賦值*/
            if (!Status.OnBeforeTurn(this))
                canPerformMove = false;
        }
        /*混亂型別*/
        if (VolatileStatus?.OnBeforeTurn != null)
        {
            if (!VolatileStatus.OnBeforeTurn(this))
                canPerformMove = false;
        }

        return canPerformMove;
    }

    /*隨機技能*/
    public Move GetRandomMove()
    {
        /*紀錄pokemon剩餘技能,避免敵人PP用完還使用該技能*/
        var moveWithPP = Moves.Where(x => x.PP > 0).ToList();

        int r = Random.Range(0, moveWithPP.Count);
        return moveWithPP[r];
    }

    /*戰鬥結束後清除buff提升或降低效果*/
    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }

    /*狀態異常顯示,修正當中了異常狀態不能再中其他異常*/
    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null) return;
        Status = ConditionDB.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name}{Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }
    /*混亂狀態*/
    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (VolatileStatus != null) return;
        VolatileStatus = ConditionDB.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name}{VolatileStatus.StartMessage}");
    }
    /*解除混亂狀態*/
    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }
    /*讀取資料*/
    public Pokemon(PokemonSaveData saveData)
    {
        _base = PokemonDB.GetPokemonByName(saveData.name);
        HP = saveData.hp;
        level = saveData.level;
        Exp = saveData.exp;
        /*狀態部分回到ConditionDB的Dictionary去查找對應狀態*/
        if (saveData.statusId != null)
            Status = ConditionDB.Conditions[saveData.statusId.Value];
        else
            Status = null;
        /*重新在陣列找move資料*/
        Moves = saveData.moves.Select(s => new Move(s)).ToList();

        CalculateStat();
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        VolatileStatus = null;
    }
    /*將pokemon類轉成pokemonSaveData類別*/
    public PokemonSaveData GetSaveData()
    {
        var saveData = new PokemonSaveData()
        {
            name = Base.Name,
            hp = HP,
            level = Level,
            exp = Exp,
            statusId = Status?.Id, //null判定如果是null回傳null
            moves = Moves.Select(m => m.GetSaveData()).ToList()
        };

        return saveData;
    }

}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }

}
/*Pokemon 序列化要儲存資料只有Hp Exp Lv status*/
[System.Serializable]
public class PokemonSaveData
{
    public string name;
    public int hp;
    public int level;
    public int exp;
    public ConditionID? statusId;
    public List<MoveSaveData> moves;
}

