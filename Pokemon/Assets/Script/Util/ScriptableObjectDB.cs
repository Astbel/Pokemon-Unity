using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*限制泛型使用型別只能用在可邊寫腳本*/
/*
name 這邊使指script的名稱
Name 這邊是指 物件的命名
*/
public class ScriptableObjectDB<T> : MonoBehaviour where T : ScriptableObject
{
    static Dictionary<string, T> objects;

    public static void Init()
    {
        objects = new Dictionary<string, T>();

        var objectArray = Resources.LoadAll<T>("");
        foreach (var obj in objectArray)
        {
            /*檢測是否陣列中有相同的pokemon,大寫N跟小寫n差別在Resource查找物件時
            大寫N->Script名稱
            小寫n->物件名稱
            */
            if (objects.ContainsKey(obj.name))
            {
                Debug.LogError($"There are two Objects with the name {obj.name}");
                continue;
            }
            objects[obj.name] = obj;
        }

    }

    public static T GetObjectByName(string name)
    {
        if (!objects.ContainsKey(name))
        {
            Debug.LogError($"Object with the name {name} not found in the database");
            return null;
        }
        return objects[name];
    }

}
