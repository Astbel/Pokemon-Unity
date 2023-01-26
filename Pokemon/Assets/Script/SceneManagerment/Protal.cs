using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protal : MonoBehaviour, IPlayerTriggerAble
{
    public void OnPlayerTriggered(PlayerController player)
    {
        Debug.Log("Sence Change");
    }
}
