using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManger : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] ChoiceBox choiceBox;
    [SerializeField] Text dialogText;
    [SerializeField] int letterPerSecond;
    public static DialogManger Instance { get; private set; }
    public event Action OnShowDialog;
    public event Action OnDialogFinished;
    public bool IsShowing { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    public IEnumerator ShowDialogText(string text, bool waitForInput = true,
    bool autoClose = true, List<string> choices = null, Action<int> onChoiceSelected = null)
    {
        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);

        AudioManager.i.PlaySfx(AudioID.UISelect);

        yield return TypeDialog(text);

        /*等待玩家按Z才關閉對話框*/
        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        if (autoClose)
        {
            CloseDialog();
        }
        OnDialogFinished?.Invoke();
    }

    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
    }

    public IEnumerator ShowDialog(Dialog dialog, List<string> choices = null,
        Action<int> onChoiceSelected = null)
    {
        /*等待對話框結束*/
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);

        /*修正顯示訊息,以及等待玩家按下Z才顯示下一段*/
        foreach (var line in dialog.Lines)
        {
            AudioManager.i.PlaySfx(AudioID.UISelect);
            yield return TypeDialog(line);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        dialogBox.SetActive(false);
        IsShowing = false;
        OnDialogFinished?.Invoke();
    }

    /*檢查對話框是否有第二行有的畫則顯示下一行*/
    public void HandleUpdate()
    {

    }

    public IEnumerator TypeDialog(string line)
    {
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
    }

}