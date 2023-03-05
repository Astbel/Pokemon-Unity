using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, Interactable, ISavable
{
    [SerializeField] Dialog dialog;

    [Header("Quest")]
    [SerializeField] QuestBase questToStart;
    [SerializeField] QuestBase questToComplete;

    [Header("Movement")]
    [SerializeField] List<Vector2> movementPattern;//要移動的向量
    [SerializeField] float timeBetweenPattern;   //間格時間
    Character character;
    int currentPattern = 0;
    float idleTimer = 0f;
    public enum NpcState { Idle, Walking, Dialog }
    NpcState state;
    ItemGiver itemGiver;
    PokemonGiver pokemonGiver;
    Quest activeQuest;
    Healer healer;


    private void Awake()
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
        pokemonGiver = GetComponent<PokemonGiver>();
        healer = GetComponent<Healer>();
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (state == NpcState.Idle)
        {
            state = NpcState.Dialog;
            character.LookTowards(initiator.position);
            /*任務完成*/
            if (questToComplete != null)
            {
                var quest = new Quest(questToComplete);
                yield return quest.CompleteQuest(initiator);
                questToComplete = null;

                Debug.Log($"{quest.Base.Name} completed");
            }

            /*NPC的物品不為null*/
            if (itemGiver != null && itemGiver.CanBeGiven())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else if (pokemonGiver != null && pokemonGiver.CanBeGiven())
            {
                yield return pokemonGiver.GivePokemon(initiator.GetComponent<PlayerController>());
            }
            else if (questToStart != null)
            {
                activeQuest = new Quest(questToStart);
                yield return activeQuest.StartQuest();
                questToStart = null; /*避免重複接取任務*/
                /*玩家本身持有該道具*/
                if (activeQuest.CanBeCompleted())
                {
                    /*initiator代表觸發人這邊觸發者是指player所以簽名帶入的也只會是player*/
                    yield return activeQuest.CompleteQuest(initiator);
                    activeQuest = null;
                }
            }
            else if (activeQuest != null)
            {
                if (activeQuest.CanBeCompleted())
                {
                    /*initiator代表觸發人這邊觸發者是指player所以簽名帶入的也只會是player*/
                    yield return activeQuest.CompleteQuest(initiator);
                    activeQuest = null;
                }
                else
                {
                    yield return DialogManger.Instance.ShowDialog(activeQuest.Base.InProgressDialogue);
                }
            }
            else if (healer != null)
            {
                yield return healer.Heal(initiator, dialog);
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

    public object CaptureState()
    {
        var saveData = new NpcQuestSaveData();
        saveData.activeQuest = activeQuest?.GetSaveData();

        if (questToStart = null)
            saveData.questToStart = (new Quest(questToStart)).GetSaveData();
        if (questToComplete = null)
            saveData.questToComplete = (new Quest(questToComplete)).GetSaveData();

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as NpcQuestSaveData;
        if (saveData != null)
        {
            activeQuest = (saveData.activeQuest != null) ? new Quest(saveData.activeQuest) : null;

            questToStart = (saveData.questToStart != null) ? new Quest(saveData.questToStart).Base : null;

            questToComplete = (saveData.questToComplete != null) ? new Quest(saveData.questToComplete).Base : null;
        }


    }
}

[System.Serializable]
public class NpcQuestSaveData
{
    public QuestSaveData activeQuest;
    public QuestSaveData questToStart;
    public QuestSaveData questToComplete;
}