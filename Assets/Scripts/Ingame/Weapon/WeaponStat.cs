using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStat
{
    private int Number;
    private float Name;
    private int minAttackRange;
    private int maxAttackRange;
    private int damage;
    private float critRate;

    public int weaponNumber
    {
        get
        {
            return Number;
        }
    }
    public float weaponName
    {
        get
        {
            return Name;
        }
    }
    public int weaponMinAttackRange
    {
        get
        {
            return minAttackRange;
        }
    }
    public int weaponMaxAttackRange
    {
        get
        {
            return maxAttackRange;
        }
    }
    public int weaponDamage
    {
        get
        {
            return damage;
        }
    }
    public float weaponCritRate
    {
        get
        {
            return critRate;
        }
    }
}
