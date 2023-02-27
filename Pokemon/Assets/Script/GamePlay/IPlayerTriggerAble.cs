using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerTriggerAble 
{
    void OnPlayerTriggered(PlayerController player);

    bool TriggerRepeatedly {get;}
}
