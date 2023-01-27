using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDB
{
    static Dictionary<string, MoveBase> moves;

    public static void Init()
    {
        moves = new Dictionary<string, MoveBase>();

        var moveList = Resources.LoadAll<MoveBase>("");
        foreach (var move in moveList)
        {
            /*檢測是否陣列中有相同的pokemon,大寫N跟小寫n差別在Resource查找物件時
            大寫N->Script名稱
            小寫n->物件名稱
            */
            if (moves.ContainsKey(move.Name))
            {
                Debug.LogError($"There are two moves with the name {move.Name}");
                continue;
            }
            moves[move.Name] = move;
        }

    }
    public static MoveBase GetPokemonByName(string name)
    {
        if (!moves.ContainsKey(name))
        {
            Debug.LogError($"Move with the name {name} not found in the database");
            return null;
        }
        return moves[name];
    }
}

