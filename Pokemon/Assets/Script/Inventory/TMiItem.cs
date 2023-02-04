using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new TM or HM")]
public class TMiItem : itemBase
{
    [SerializeField] MoveBase move;

    public override bool Use(Pokemon pokemon)
    {
        /*從invertoryUI 確認是否學系招式如果是澤回傳true*/
        return pokemon.HasMove(move);
    }

    public MoveBase Move => move;

}
