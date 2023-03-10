using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{

    public MoveBase Base { get; set; }

    public int PP { get; set; }

    public Move(MoveBase pBase)
    {
        Base = pBase;
        PP = pBase.PP;
    }
    /*讀取*/
    public Move(MoveSaveData saveData)
    {
       Base= MoveDB.GetPokemonByName(saveData.name);
        PP=saveData.pp;
    }
    /*存取*/
    public MoveSaveData GetSaveData()
    {
        var saveData =new MoveSaveData()
        {
            name=Base.Name,
            pp=PP

        };
        return saveData;
    }
}

[Serializable]
public class MoveSaveData
{
    public string name;
    public int pp;
}
