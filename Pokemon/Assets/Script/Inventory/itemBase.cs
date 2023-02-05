using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;
    [SerializeField] Sprite icon;
    /*虛擬化讓他可以複寫*/
    public virtual string Name => name;
    public string Description => description;
    public Sprite Icon => icon;

    public virtual bool Use(Pokemon pokemon)
    {
        return false;
    }

    /*是否可重複使用Ex HM story item*/
    public virtual bool IsReuseable => false;
    public virtual bool CanUseInBattle => true;
    public virtual bool CanUseOutsideBattle => true;

}
