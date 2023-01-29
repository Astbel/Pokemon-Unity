using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new recovert item")]
public class RecoveryItem : itemBase
{
    [Header("HP")]
    [SerializeField] int hpAmount;
    [SerializeField] bool restoreMaxHp;
    [Header("PP")]
    [SerializeField] int ppAmount;
    [SerializeField] bool restoreMaxPP;
    [Header("Status Conditions")]
    [SerializeField] ConditionID status;
    [SerializeField] bool recoverAllStatus;
    [Header("Revive")]
    [SerializeField] bool reive;
    [SerializeField] bool maxRevive;


    public override bool Use(Pokemon pokemon)
    {
        /*檢查是否為昏厥pokemon,且不能為滿血狀態*/
        if (hpAmount > 0)
        {
            if(pokemon.HP==pokemon.MaxHp)
                return false;

            pokemon.InecreaseHP(hpAmount);    
        }

        return true;
    }


}
