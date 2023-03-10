using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int letterPerSecond;
    [SerializeField] Text dialogText;
    /*選耽物件*/
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] GameObject choiceBox;
    /*動作以及招式*/
    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;
    /*屬性以及次數*/
    [SerializeField] Text ppText;
    [SerializeField] Text typeText;
    [SerializeField] Text yesText;
    [SerializeField] Text noText;

    Color highlightedColor;

    private void Start()
    {
        highlightedColor = GlobalSettings.i.HighhlightColor;
    }

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }

        yield return new WaitForSeconds(1f); //wait fot 1sec
    }
    /*智能選單介面*/
    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableChoiceBox(bool enabled)
    {
        choiceBox.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void UpdateChoiceBox(bool yesSelect)
    {
        if (yesSelect)
        {
            yesText.color = highlightedColor;
            noText.color = Color.black;
        }
        else
        {
            yesText.color = Color.black;
            noText.color = highlightedColor;
        }
    }

    /*偵測選單選擇的項目,如果有選擇則顯示選項顏色否則皆為黑色*/
    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; ++i)
        {
            if (i == selectedAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }

    /*技能選擇選單顏色提示*/
    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i == selectedMove)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }
        /*顯示技能PP / Type*/
        ppText.text = $"PP {move.PP}/{move.Base.PP} ";
        typeText.text = move.Base.Type.ToString();

        /*如果PP為0,如果是MAX的一半變為黃色*/
        if (move.PP == 0)
        {
            ppText.color = Color.red;
        }
        else if (move.PP <= move.Base.PP / 2)
        {
            ppText.color = Color.yellow;
        }
        else
        {
            ppText.color = Color.black;
        }
    }

    /*Pokemon 技能顯示如果檢測技能是否少於4個如果比較少則顯示"-"*/
    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
                moveTexts[i].text = moves[i].Base.Name;
            else
                moveTexts[i].text = "-";
        }
    }



}
