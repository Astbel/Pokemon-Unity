using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;//要移動的向量
    [SerializeField] float timeBetweenPattern;   //間格時間
    Character character;
    ItemGiver itemGiver;
    int currentPattern = 0;
    float idleTimer = 0f;
    public enum NpcState { Idle, Walking, Dialog }
    NpcState state;
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (state == NpcState.Idle)
        {
            state = NpcState.Dialog;
            character.LookTowards(initiator.position);

            if (itemGiver != null && itemGiver.CanBeGiven())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else
            {
                yield return DialogManger.Instance.ShowDialog(dialog);
            }

            idleTimer = 0f;
            state = NpcState.Idle;

        }

    }

    private void Update()
    {
        if (state == NpcState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }
        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NpcState.Walking;

        var oldPos = transform.position;

        yield return character.Move(movementPattern[currentPattern]);
        if (transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPattern.Count;

        state = NpcState.Idle;
    }


}
