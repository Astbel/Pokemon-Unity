using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> slots;
    /*事件用來跟新包包道具數量*/
    public event Action OnUpdated;
    public List<ItemSlot> Slots => slots;
    /*使用道具*/
    public itemBase UseItem(int ItemIndex, Pokemon selectedPokemon)
    {
        var item = slots[ItemIndex].Item;
        bool itemUsed = item.Use(selectedPokemon);
        if (itemUsed)
        {
            RemoveItem(item);
            return item;
        }

        return null;
    }

    public void RemoveItem(itemBase item)
    {
        /*用linq去檢測是否為該道具*/
        var itemSlot = slots.First(slots => slots.Item == item);
        itemSlot.Count--;
        /*如果道具使用數為0移除該項目*/
        if (itemSlot.Count == 0)
            slots.Remove(itemSlot);

        OnUpdated?.Invoke();
    }

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }
}


[Serializable]
public class ItemSlot
{
    [SerializeField] itemBase item;
    [SerializeField] int count;

    public itemBase Item => item;
    public int Count
    {
        get => count;
        set => count = value;
    }

}
