using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerAble
{

// 用來避免遇敵過於頻繁的時間間隔
    private const float EncounterCooldown = 5.0f;
    private float lastEncounterTime;

    public void OnPlayerTriggered(PlayerController player)
    {
        if (Time.time - lastEncounterTime >= EncounterCooldown && UnityEngine.Random.Range(1, 101) <= 10)
        {
            player.Character.Animator.IsMoving = false;
            GameController.Instance.StartBattle();

            lastEncounterTime = Time.time;
        }
    }

     public bool TriggerRepeatedly => true;
}
