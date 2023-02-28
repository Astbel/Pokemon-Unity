using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObject : MonoBehaviour
{
    [SerializeField] QuestBase questToCheck;
    [SerializeField] ObjectAction OnStart;
    [SerializeField] ObjectAction OnComplete;

    QuestList questList;

    private void Start()
    {
        questList = QuestList.GetQuestList();
        /*當訂閱觸發後調用update跟新*/
        questList.OnUpdated +=UpdateObjectStatus;

        UpdateObjectStatus();
    }


    private void OnDestroy()
    {
       questList.OnUpdated -=UpdateObjectStatus;
    }

    /*檢測所有子類別是否enable*/
    public void UpdateObjectStatus()
    {
        if (OnStart != ObjectAction.DoNothing && questList.IsStarted(questToCheck.Name))
        {
            foreach (Transform child in transform)
            {
                if (OnStart == ObjectAction.Enable)
                    child.gameObject.SetActive(true);
                else if (OnStart == ObjectAction.Disable)
                    child.gameObject.SetActive(false);
            }
        }
        if (OnComplete != ObjectAction.DoNothing && questList.IsCompleted(questToCheck.Name))
        {
            foreach (Transform child in transform)
            {
                if (OnComplete == ObjectAction.Enable)
                    child.gameObject.SetActive(true);
                else if (OnComplete == ObjectAction.Disable)
                    child.gameObject.SetActive(false);
            }
        }
    }





}

public enum ObjectAction { DoNothing, Enable, Disable }