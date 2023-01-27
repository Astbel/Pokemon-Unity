using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFov : MonoBehaviour, IPlayerTriggerAble
{
    public void OnPlayerTriggered(PlayerController player)
    {
        /*停止腳色移動動畫*/
        player.Character.Animator.IsMoving = false;
        GameController.Instance.OnEnterTrainerView(GetComponentInParent<TrainerController>());
    }
}
