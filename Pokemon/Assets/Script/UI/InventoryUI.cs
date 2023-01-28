using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;
    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;
    int selectedItem = 0;
    Inventory inventory;
    List<ItemSlotUI> slotUIList;
    RectTransform itemListRect;
    /*包包軸限制轉動數*/
    const int itemInViewport = 4;
    /*由於包包UI跟玩家是分開的所以用findobject來查詢玩家的物品*/
    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdataItemList();
    }

    void UpdataItemList()
    {
        //清除所有Itemlist的子類別
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);
        /*初始化list用來存放所有子類別物件*/
        slotUIList = new List<ItemSlotUI>();
        /*把所有物品實例化並加進list*/
        foreach (var itemSlot in inventory.Slots)
        {
            /*實例化物件給父類傳遞*/
            var slotUIobj = Instantiate(itemSlotUI, itemList.transform);
            slotUIobj.SetData(itemSlot);
            slotUIList.Add(slotUIobj);
        }
        UpdateItemSelection();
    }
    public void HandleUpdate(Action onBack)
    {
        /*紀錄當前選擇*/
        int prevSelection = selectedItem;
        /*選擇部分*/
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;
        selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Slots.Count - 1);
        /*如果有跟新選擇才跟新選擇標籤顏色*/
        if (prevSelection != selectedItem)
            UpdateItemSelection();


        if (Input.GetKeyDown(KeyCode.X))
            onBack?.Invoke();
    }
    void UpdateItemSelection()
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.i.HighhlightColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }

        var item = inventory.Slots[selectedItem].Item;
        itemIcon.sprite = item.Icon;
        itemDescription.text = item.Description;

        HandleSrcolling();
    }

    void HandleSrcolling()
    {
        float scrollPos = Mathf.Clamp(selectedItem - itemInViewport, 0, selectedItem) * slotUIList[0].Height;
        /*包包選項時只有y軸移動*/
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
        /*包包箭頭介面*/
        bool showUpArrow = selectedItem > itemInViewport;
        upArrow.gameObject.SetActive(showUpArrow);
        bool shodownpArrow = selectedItem + itemInViewport < slotUIList.Count;
        downArrow.gameObject.SetActive(shodownpArrow);

    }


}
