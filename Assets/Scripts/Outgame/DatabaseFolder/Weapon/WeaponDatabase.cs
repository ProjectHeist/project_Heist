using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Database/WeaponDatabase", order = 1)]
public class WeaponDatabase : ScriptableObject
{
    public WeaponStat[] weaponStatList;
}


