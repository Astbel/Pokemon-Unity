using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusUI : MonoBehaviour
{
    [SerializeField] StatusUIText  StatusUiChoicePerfab;

    List<ChoiceText> StatusUichoiceTexts;
    int currentChoice;

    int selection = 0;
    
   

    //開啟介面時上下選擇
    public void HandleUpdate(Action onSelected, Action onBack)
    {
         var prevSelection = selection;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentChoice;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentChoice;
        currentChoice = Mathf.Clamp(currentChoice, 0, StatusUichoiceTexts.Count - 1);
        /*顯示目前選項*/
        for (int i = 0; i < StatusUichoiceTexts.Count; i++)
        {
            StatusUichoiceTexts[i].SetSelected(i == currentChoice);
        }
        Debug.Log(currentChoice);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke();
        }
         /*返回鍵*/
        else if (Input.GetKeyDown(KeyCode.X))
        {
            gameObject.SetActive(false);
        }
   
    }


}
