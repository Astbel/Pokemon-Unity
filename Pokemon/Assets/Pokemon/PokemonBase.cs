using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] PokemonType type1;
    [SerializeField] PokemonType type2;

    /*Base Stats*/
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;
    [SerializeField] List<LearnableMove> learnableMoves;

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public Sprite FrontSprite
    {
        get { return frontSprite; }
    }

    public Sprite BackSprite
    {
        get { return backSprite; }
    }

    public PokemonType Type1
    {
        get { return type1; }
    }

    public PokemonType Type2
    {
        get { return type2; }
    }


    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }

    public int SpAttack
    {
        get { return spAttack; }
    }

    public int SpDefense
    {
        get { return spDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }


}
/*
為了在inspectior上配置幾等學習技能所以要新增System.Serializable
*/
[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;
    public MoveBase MoveBase
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }

}


public enum PokemonType
{
    None,
    Normal,
    FIre,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Posion,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon
}
/*屬性克制*/
public class TypeChart
{
    static float[][] chart =
     {
        //                  nor   Fir  Wat  Ele   Gra    Ice  Fig   Poi
        /*Nor*/ new float[] {1f,  1f,   1f,  1f,  1f,    1f,  1f,   1f},
        /*Fir*/ new float[] {1f, 0.5f,0.5f,  1f,  2f,    2f,  1f,   1f},
        /*Wat*/ new float[] {1f,  2f, 0.5f,  2f,  0.5f,   1f,  1f,  1f},
        /*Ele*/ new float[] {1f,  1f,  2f,  0.5f, 0.5f,  1f,  1f,   1f},
        /*Gra*/ new float[] {1f, 0.5f, 2f,  2f,   0.5f,  1f,  1f,  0.5f},
        /*Poi*/ new float[] {1f,  1f,  1f,  1f,   2f,    1f,  1f,   1f},

    };

    public static float GetEffectivenss(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.None || defenseType == PokemonType.None)
            return 1;
        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }

}
