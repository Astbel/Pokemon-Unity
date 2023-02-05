using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ItemCategory { Items, Pokeballs, Tms }

public class Inventory : MonoBehaviour
{
    [SerializeField] List<ItemSlot> slots;
    [SerializeField] List<ItemSlot> pokeballslots;
    [SerializeField] List<ItemSlot> tmSlots;

    /*用來回傳List的型別*/
    List<List<ItemSlot>> allSlots;

    /*事件用來跟新包包道具數量*/
    public event Action OnUpdated;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { slots, pokeballslots, tmSlots };
    }

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "ITEMS"," POKEBALLS ","TMs & HMs"
    };
    /*回傳其中一種鞋別的slot*/
    public List<ItemSlot> GetSlotByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }
    /*獲取道具種類以及位置*/
    public itemBase GetItem(int itemIndex, int categoryIndex)
    {
        var currentSlots = GetSlotByCategory(categoryIndex);
        return currentSlots[itemIndex].Item;
    }
    /*使用道具*/
    public itemBase UseItem(int ItemIndex, Pokemon selectedPokemon, int selectedCategory)
    {
        var item = GetItem(ItemIndex, selectedCategory);
        bool itemUsed = item.Use(selectedPokemon);
        if (itemUsed)
        {
            if (!item.IsReuseable)
                RemoveItem(item, selectedCategory);

            return item;
        }

        return null;
    }
    public void AddItem(itemBase item, int count = 1)
    {
        /*回傳enum索引確認撿起道具類別*/
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotByCategory(category);
        /*確認插槽當沒有匯回傳null*/
        var itemSlot = currentSlots.FirstOrDefault(slots => slots.Item == item);
        /*如果本身有則增加數量就好*/
        if (itemSlot != null)
        {
            itemSlot.Count += count;
        }
        /*撿起時要擴充格子創建物件以及數量*/
        else
        {
            currentSlots.Add(new ItemSlot()
            {
                Item = item,
                Count = count
            });
        }
        OnUpdated?.Invoke();
    }

    public void RemoveItem(itemBase item, int selectedCategory)
    {
        var currentSlots = GetSlotByCategory(selectedCategory);
        /*用linq去檢測是否為該道具*/
        var itemSlot = currentSlots.First(slots => slots.Item == item);
        itemSlot.Count--;
        /*如果道具使用數為0移除該項目*/
        if (itemSlot.Count == 0)
            currentSlots.Remove(itemSlot);

        OnUpdated?.Invoke();
    }

    /*確認道具類別*/
    ItemCategory GetCategoryFromItem(itemBase item)
    {
        if (item is RecoveryItem)
            return ItemCategory.Items;
        else if (item is PokeBallItem)
            return ItemCategory.Pokeballs;
        else
            return ItemCategory.Tms;
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

    public itemBase Item
    {
        get => item;
        set => item = value;
    }
    public int Count
    {
        get => count;
        set => count = value;
    }

}
