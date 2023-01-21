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
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;

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

    public MoveEffects Effects { get { return effects; } }

    public MoveTarget Targetget { get { return target; } }


    public MoveCategory Category { get { return category; } }
}

public enum MoveTarget
{
    Enemy, Self
}


public enum MoveCategory
{
    Physical, Special, Status
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoost> boosts;

    public List<StatBoost> Boosts { get { return boosts; } }

}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}







