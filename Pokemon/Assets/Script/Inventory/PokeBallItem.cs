using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Create new Pokeball")]
public class PokeBallItem : itemBase
{
    [SerializeField] float catchRateModfier = 1;

    public override bool Use(Pokemon pokemon)
    {
        if(GameController.Instance.State==GameState.Battle)
            return true;
        else
            return false;
    }

    public float CatchRateModfier => catchRateModfier;
}
