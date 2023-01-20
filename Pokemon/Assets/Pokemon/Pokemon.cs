using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

    public PokemonBase Base { get{return _base;} }
    public int Level { get{return level;} }

    public int HP { get; set; }

    public List<Move> Moves { get; set; }

    public void Init()
    {
        HP = MaxHp;

        /*學習技能檢查是否有在該腳色List中*/
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
            {
                Moves.Add(new Move(move.MoveBase));
            }
            /*當技能超過4則break*/
            if (Moves.Count >= 4)
            {
                break;
            }
        }

    }

    public int Attack
    {
        get { return Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5; }
    }

    public int MaxHp
    {
        get { return Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 5; }
    }

    public int Defense
    {
        get { return Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5; }
    }

    public int SpAttack
    {
        get { return Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5; }
    }

    public int SpDefense
    {
        get { return Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5; }
    }

    public int Speed
    {
        get { return Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5; }
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
        float attack = (move.Base.IsSpecial) ? attacker.SpAttack : attacker.Attack;
        /*判斷是否為特防*/
        float defense = (move.Base.IsSpecial) ? SpDefense : Defense;
        /*damage formula*/
        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack / defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        if (HP < 0)
        {
            HP = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    }

    /*隨機技能*/
    public Move GetRandomMove()
    {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}


public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }

}



