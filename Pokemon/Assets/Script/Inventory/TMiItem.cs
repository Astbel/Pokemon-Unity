using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new TM or HM")]
public class TMiItem : itemBase
{
    [SerializeField] MoveBase move;
    [SerializeField] bool isHM;

    public override string Name => base.Name + $": {move.Name} ";

    public override bool Use(Pokemon pokemon)
    {
        /*從invertoryUI 確認是否學系招式如果是澤回傳true*/
        return pokemon.HasMove(move);
    }

    public bool CanBeTaught(Pokemon pokemon)
    {
      return  pokemon.Base.LearnAbleByItems.Contains(Move);
    }

    /*判斷是是不是HM招式*/
    public override bool IsReuseable => isHM;

    public override bool CanUseInBattle => false;

    public MoveBase Move => move;
    public bool IsHM => isHM;
}
