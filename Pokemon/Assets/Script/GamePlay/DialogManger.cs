using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManger : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int letterPerSecond;
    public static DialogManger Instance { get; private set; }
    public event Action OnShowDialog;
    public event Action OnCloseDialog;
    bool isTyping;
    public bool IsShowing { get; private set; }
    int currentLine = 0; //計算對話比數
    Dialog dialog;
    Action onDialogFinish;
    private void Awake()
    {
        Instance = this;
    }
    public IEnumerator ShowDialogText(string text, bool waitForInput = true)
    {
        IsShowing = true;
        dialogBox.SetActive(true);

        yield return TypeDialog(text);

        /*等待玩家按Z才關閉對話框*/
        if (waitForInput)
        {
            yield return new WaitUntil(()=>Input.GetKeyDown(KeyCode.Z));
        }
        dialogBox.SetActive(false);
        IsShowing = false;
    }


    public IEnumerator ShowDialog(Dialog dialog, Action onFinished = null)
    {
        /*等待對話框結束*/
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();

        IsShowing = true;
        this.dialog = dialog;

        onDialogFinish = onFinished;/*判斷對話是否結束*/

        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

    /*檢查對話框是否有第二行有的畫則顯示下一行*/
    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isTyping)
        {
            ++currentLine;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            //對話結束時
            else
            {
                currentLine = 0;
                IsShowing = false;
                dialogBox.SetActive(false);
                onDialogFinish?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }
    }

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
        isTyping = false;
    }

}
