using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movementPattern;//要移動的向量
    [SerializeField] float timeBetweenPattern;   //間格時間
    Character character;
    int currentPattern = 0;
    float idleTimer = 0f;
    public enum NpcState { Idle, Walking }
    NpcState state;
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Interact()
    {
        if (state == NpcState.Idle)
            StartCoroutine(DialogManger.Instance.ShowDialog(dialog));
    }

    private void Update()
    {
        /*如果對話正在執行,NPC停下走動*/
        if (DialogManger.Instance.IsShowing) return;

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
        yield return character.Move(movementPattern[currentPattern]);
        currentPattern = (currentPattern + 1) % movementPattern.Count;

        state = NpcState.Idle;
    }


}
