using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFov : MonoBehaviour, IPlayerTriggerAble
{
    public void OnPlayerTriggered(PlayerController player)
    {
        GameController.Instance.OnEnterTrainerView(GetComponentInParent<TrainerController>());
    }
}