using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] QuestBase questToStart;
    [SerializeField] List<Vector2> movementPattern;//要移動的向量
    [SerializeField] float timeBetweenPattern;   //間格時間
    Character character;
    int currentPattern = 0;
    float idleTimer = 0f;
    public enum NpcState { Idle, Walking, Dialog }
    NpcState state;
    ItemGiver itemGiver;

    Quest activeQuest;

    private void Awake()
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (state == NpcState.Idle)
        {
            state = NpcState.Dialog;
            character.LookTowards(initiator.position);
            /*NPC的物品不為null*/
            if (itemGiver != null && itemGiver.CanBeGiven())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else if (questToStart != null)
            {
                activeQuest = new Quest(questToStart);
                yield return activeQuest.StartQuest();
                questToStart =null; /*避免重複接取任務*/
            }
            else if(activeQuest!=null)
            {
                if(activeQuest.CanBeCompleted())
                {
                    /*initiator代表觸發人這邊觸發者是指player所以簽名帶入的也只會是player*/
                    yield return activeQuest.CompleteQuest(initiator);
                    activeQuest=null;
                }
                else
                {
                     yield return DialogManger.Instance.ShowDialog(activeQuest.Base.InProgressDialogue);
                }
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
