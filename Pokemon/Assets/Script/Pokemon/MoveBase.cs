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
    [SerializeField] int accuracy;     //命中率
    [SerializeField] bool alwaysHits;  //必中技能
    [SerializeField] int pp;
    [SerializeField] int priority;
    [SerializeField] MoveCategory category;
    [SerializeField] MoveEffects effects;
    [SerializeField] MoveTarget target;
    [SerializeField] List<SecondaryEffects> secondaryEffects;
    [SerializeField] AudioClip sound;
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
    public bool AlwaysHits
    {
        get { return alwaysHits; }
    }
    public int PP
    {
        get { return pp; }
    }
    public int Priority
    {
        get { return priority; }
    }

    public List<SecondaryEffects> SecondaryEffects { get { return secondaryEffects; } }
    public MoveEffects Effects { get { return effects; } }
    public MoveTarget Target { get { return target; } }
    public MoveCategory Category { get { return category; } }

    public AudioClip Sound => sound;
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
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;

    public List<StatBoost> Boosts { get { return boosts; } }
    public ConditionID Status { get { return status; } }
    public ConditionID VolatileStatus { get { return volatileStatus; } }

}

[System.Serializable]
public class StatBoost
{
    public Stat stat;
    public int boost;
}

/*招式第二效果應用在十萬伏特  極凍光線  噴射火焰 有機率造成異常狀態*/
[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance { get { return chance; } }
    public MoveTarget Target { get { return target; } }

}








