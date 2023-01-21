using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string StartMessage { get; set; }
    /*用於中毒 燒傷每回合扣血使用*/
    public Action<Pokemon> OnAfterTurn { get; set; }

}
