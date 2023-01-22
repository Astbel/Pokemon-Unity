using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    /*狀態異常ID*/
    public ConditionID Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }
    public Action<Pokemon> OnStart { get; set; }
    /*用於中毒 燒傷每回合扣血使用*/
    public Action<Pokemon> OnAfterTurn { get; set; }
    /*用於凍結 麻痺 睡眠*/
    public Func<Pokemon, bool> OnBeforeTurn { get; set; }
}
