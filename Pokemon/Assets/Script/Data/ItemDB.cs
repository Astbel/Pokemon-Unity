using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDB
{
    static Dictionary<string, itemBase> items;

    public static void Init()
    {
        items = new Dictionary<string, itemBase>();

        var itemList = Resources.LoadAll<itemBase>("");
        foreach (var item in itemList)
        {
            /*檢測是否陣列中有相同的pokemon,大寫N跟小寫n差別在Resource查找物件時
            大寫N->Script名稱
            小寫n->物件名稱
            */
            if (items.ContainsKey(item.Name))
            {
                Debug.LogError($"There are two moves with the name {item.Name}");
                continue;
            }
            items[item.Name] = item;
        }

    }
    public static itemBase GetItemByName(string name)
    {
        if (!items.ContainsKey(name))
        {
            Debug.LogError($"Move with the name {name} not found in the database");
            return null;
        }
        return items[name];
    }
}
