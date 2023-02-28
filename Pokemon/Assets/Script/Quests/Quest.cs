using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public QuestBase Base { get; private set; }
    public QuestStatus Status { get; private set; }

    public Quest(QuestBase _base)
    {
        Base = _base;
    }
    /*從quest類別來保存任務*/
    public QuestSaveData GetSaveData()
    {
        var saveData = new QuestSaveData()
        {
            name = Base.Name,
            status = Status

        };
        return saveData;
    }
    /*讀取用*/
    public Quest(QuestSaveData saveData)
    {
        Base = QuestDB.GetObjectByName(saveData.name);
        Status = saveData.status;
    }

    /*開始完成都放入鍊表*/
    public IEnumerator StartQuest()
    {
        Status = QuestStatus.Started;

        yield return DialogManger.Instance.ShowDialog(Base.StartDialogue);

        var questList = QuestList.GetQuestList();

        questList.AddQuest(this);
    }

    public IEnumerator CompleteQuest(Transform player)
    {
        Status = QuestStatus.Completed;

        yield return DialogManger.Instance.ShowDialog(Base.CompleteDialogue);
        /*確認道具類別*/
        var inventory = Inventory.GetInventory();
        if (Base.RequiredItem != null)
        {
            inventory.RemoveItem(Base.RequiredItem);
        }

        if (Base.RewardItem != null)
        {
            inventory.AddItem(Base.RewardItem);
            string playerName = player.GetComponent<PlayerController>().Name;
            yield return DialogManger.Instance.ShowDialogText($"{playerName} received {Base.RewardItem.Name}");
        }

        var questList = QuestList.GetQuestList();

        questList.AddQuest(this);
    }

    /*用來確認任務是否完成*/
    public bool CanBeCompleted()
    {
        var inventory = Inventory.GetInventory();
        if (Base.RequiredItem != null)
        {
            if (!inventory.HasItem(Base.RequiredItem))
                return false;
        }
        return true;
    }
}
[System.Serializable]
public class QuestSaveData
{
    public string name;
    public QuestStatus status;
}


public enum QuestStatus { None, Started, Completed }
