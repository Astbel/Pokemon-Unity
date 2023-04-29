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
        /*復活道具當執行後,清除debuff,當使用後break*/
        if (reive || maxRevive)
        {
            if (pokemon.HP > 0)
                return false;
            /*復活*/
            if (reive)
                pokemon.InecreaseHP(pokemon.MaxHp / 4);
            else if (maxRevive)
                pokemon.InecreaseHP(pokemon.MaxHp);

            pokemon.CureStatus();

            return true;
        }
        /*陣亡寶可夢不可以使用以下道具*/
        if (pokemon.HP == 0)
            return false;
        /*檢查是否為昏厥pokemon,且不能為滿血狀態*/
        if (restoreMaxHp || hpAmount > 0)
        {
            /*Pokemon滿血不回血*/
            if (pokemon.HP == pokemon.MaxHp)
                return false;
            /*Pokemonm回復超級HP*/
            if (restoreMaxHp)
                pokemon.InecreaseHP(pokemon.MaxHp);
            else
                pokemon.InecreaseHP(hpAmount);
        }

        /*回復異常狀態*/
        if (recoverAllStatus || status != ConditionID.none)
        {
            /*如果寶可夢正常則不使用道具*/
            if (pokemon.Status == null && pokemon.VolatileStatus != null)
                return false;

            if (recoverAllStatus)
            {
                pokemon.CureStatus();
                pokemon.CureVolatileStatus();
            }
            else
            {
                if (pokemon.Status.Id == status)
                    pokemon.CureStatus();
                else if (pokemon.VolatileStatus.Id == status)
                    pokemon.CureVolatileStatus();
                else
                    return false;
            }
        }

        /*回復招式*/
        if (restoreMaxPP)
        {
            pokemon.Moves.ForEach(m => m.InecreasePP(m.Base.PP));
        }
        else if (ppAmount > 0)
        {
            pokemon.Moves.ForEach(m => m.InecreasePP(ppAmount));
        }
        return true;
    }


}
