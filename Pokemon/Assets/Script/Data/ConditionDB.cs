using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionDB
{
    /*初始化所有Dictionary 的變量*/
    public static void Init()
    {
        foreach (var StatusID in Conditions)
        {
            var conditionID = StatusID.Key;
            var condition = StatusID.Value;

            condition.Id = conditionID;
        }
    }
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn,
            new Condition()
            {
                Name ="Poison",
                StartMessage ="has been poisioned",
                //Lamda Function
                OnAfterTurn =(Pokemon pokemon)=>
                {
                    //中毒傷害
                    pokemon.UpdateHP(pokemon.MaxHp/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to poison");
                }
            }
        },
        {
            ConditionID.brn,
            new Condition()
            {
                Name ="Burn",
                StartMessage ="has been burned",
                //Lamda Function
                OnAfterTurn =(Pokemon pokemon)=>
                {
                    //燒傷傷害
                    pokemon.UpdateHP(pokemon.MaxHp/8);
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} hurt itself due to burn");
                }
            }
        },
        {
            ConditionID.par,
            new Condition()
            {
                Name ="Paralyzed",
                StartMessage ="has been paralyzed",
                OnBeforeTurn =(Pokemon pokemon)=>
                {
                    if(Random.Range(1,5)==1)
                    {
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'s paralyzed and can't move");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
                Name ="Freeze",
                StartMessage ="has been Frozen",
                OnBeforeTurn =(Pokemon pokemon)=>
                {
                    if(Random.Range(1,5)==1)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name}'s is not frozen anymore");
                        return true;
                    }
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} can't move because is frozen");
                    return false;
                }
            }
        },
        {
            ConditionID.slp,
            new Condition()
            {
                Name ="Sleep",
                StartMessage ="has fallen asleep",
                OnStart=(Pokemon pokemon)=>
                {
                        //Sleep turn set as 1-3 turns
                        pokemon.StatusTime=Random.Range(1,4);
                        Debug.Log($"Will be asleep for {pokemon.StatusTime} moves");
                },
                OnBeforeTurn =(Pokemon pokemon)=>
                {
                    if (pokemon.StatusTime <=0)
                    {
                        pokemon.CureStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} woke up!");
                        return true;
                    }   
                    /*睡眠前間減少回合被在Text窗列出*/
                    pokemon.StatusTime--;
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is sleeping ");
                    return false;
                }
            }
        },
        //Volatile Status Conditions
         {
            ConditionID.confusion,
            new Condition()
            {
                Name ="Confusion",
                StartMessage ="has been confusion",
                OnStart=(Pokemon pokemon)=>
                {
                    //Sleep turn set as 1-4 turns
                    pokemon.VolatileStatusTime=Random.Range(1,5);
                    Debug.Log($"Will be confusion for {pokemon.StatusTime} moves");
                },
                OnBeforeTurn =(Pokemon pokemon)=>
                {
                    if (pokemon.VolatileStatusTime <=0)
                    {
                        pokemon.CureVolatileStatus();
                        pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} kicked out of confusion!");
                        return true;
                    }   
                    pokemon.VolatileStatusTime--;
                    /*混亂狀態有機會使出技能 Rate 50%*/
                    if(Random.Range(1,3)==1)
                        return true;

                    //混亂中自己受到傷害
                    pokemon.StatusChanges.Enqueue($"{pokemon.Base.Name} is confusion ");
                    pokemon.UpdateHP(pokemon.MaxHp/8);
                    pokemon.StatusChanges.Enqueue($"It hurt itself due to  confusion ");
                    return false;
                }
            }
        }
    };
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz,confusion
}