using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat
{
    private int Number;
    private int Class;
    private string Name;
    private int HP;
    private float accuracy;
    private int moveRange;

    public int playerNumber
    {
        get
        {
            return Number;
        }
    }
    public int playerClass
    {
        get
        {
            return Class;
        }
    }
    public string playerName
    {
        get
        {
            return Name;
        }
    }
    public int playerHP
    {
        get
        {
            return HP;
        }
    }
    public float playerAccuracy
    {
        get
        {
            return accuracy;
        }
    }
    public int playerMoveRange
    {
        get
        {
            return moveRange;
        }
    }
}
