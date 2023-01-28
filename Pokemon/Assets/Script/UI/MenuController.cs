using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject menu;
    List<Text> menuItem;
    /*選單選擇事件*/
    public event Action<int> onMenuSelected;
    public event Action closeMenuSelected;
    int selectedItem = 0;

    /*取得menu上所有的元素*/
    private void Awake()
    {
        menuItem = menu.GetComponentsInChildren<Text>().ToList();
    }

    public void OpenMenu()
    {
        menu.SetActive(true);
        UpdateItemSelection();
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
    }


    /*控制選單*/
    public void HandleUpdate()
    {
        /*紀錄當前選擇*/
        int prevSelection = selectedItem;
        /*選擇部分*/
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;
        selectedItem = Mathf.Clamp(selectedItem, 0, menuItem.Count - 1);
        /*如果有跟新選擇才跟新選擇標籤顏色*/
        if (prevSelection != selectedItem)
            UpdateItemSelection();
        /*Z Key選擇時觸發事件*/
        if (Input.GetKeyDown(KeyCode.Z))
        {
            onMenuSelected?.Invoke(selectedItem);
            CloseMenu();
        }

        /*X key關閉菜單*/
        else if (Input.GetKeyDown(KeyCode.X))
        {
            closeMenuSelected?.Invoke();
            CloseMenu();
        }

    }

    void UpdateItemSelection()
    {
        for (int i = 0; i < menuItem.Count; i++)
        {
            if (i == selectedItem)
                menuItem[i].color = GlobalSettings.i.HighhlightColor;
            else
                menuItem[i].color = Color.black;
        }
    }
}
