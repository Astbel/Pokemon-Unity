using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
1.CreateAssetMenu 在unity 上建立菜單,
2.fileName 文件名稱,menuName 在object 上面建立菜單名,order為按鈕順序
3.原式 [CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new Move"),order=1]
*/
[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new Move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;
    [SerializeField] PokemonType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;


    public string Name
    {
        get { return name; }
    }

    public PokemonType Type
    {
        get { return type; }
    }

    public int Power
    {
        get { return power; }
    }

    public int Accuracy
    {
        get { return accuracy; }
    }

    public int PP
    {
        get { return pp; }
    }
    /*Special Move*/
    public bool IsSpecial
    {
        get{
            if (type==PokemonType.FIre||type==PokemonType.Water||type==PokemonType.Grass||type==PokemonType.Ice||type==PokemonType.Electric
                ||type==PokemonType.Dragon)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


}
