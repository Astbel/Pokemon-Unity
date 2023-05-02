using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;
    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    int selectedItem;
    RectTransform itemListRect;
    List<itemBase> availableItems;
    List<ItemSlotUI> slotUIList;
    const int itemInViewport = 4;

    private void Awake()
    {
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    public void Show(List<itemBase> availableItems)
    {
        this.availableItems = availableItems;
        gameObject.SetActive(true);
        UpdataItemList();
    }

    public void HandleUpdate()
    {
        var prevSelection = selectedItem;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItem;
        else if ((Input.GetKeyDown(KeyCode.UpArrow)))
            --selectedItem;
        selectedItem = Mathf.Clamp(selectedItem, 0, availableItems.Count - 1);

        if (selectedItem != prevSelection)
            UpdateItemSelection();
    }

    void UpdataItemList()
    {
        //清除所有Itemlist的子類別
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);
        /*初始化list用來存放所有子類別物件*/
        slotUIList = new List<ItemSlotUI>();
        /*把所有物品實例化並加進list*/
        foreach (var item in availableItems)
        {
            /*實例化物件給父類傳遞*/
            var slotUIobj = Instantiate(itemSlotUI, itemList.transform);
            slotUIobj.SetNameAndPrice(item);
            slotUIList.Add(slotUIobj);
        }
        UpdateItemSelection();
    }

    void UpdateItemSelection()
    {
        selectedItem = Mathf.Clamp(selectedItem, 0, availableItems.Count - 1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.i.HighhlightColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }
        if (availableItems.Count > 0)
        {
            var item = availableItems[selectedItem];
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }

        HandleSrcolling();
    }

    void HandleSrcolling()
    {
        /*如果道具小於這個數量就不需要顯示滾動軸*/
        if (slotUIList.Count <= itemInViewport) return;

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
