using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDatabase
{
    public WeaponStatList weaponStatList;
    public void Load()
    {
        var LoadedJson = Resources.Load<TextAsset>("Data/Weapon/TestWeapon");
        weaponStatList = JsonUtility.FromJson<WeaponStatList>(LoadedJson.text);
    }
}

[System.Serializable]
public class WeaponStatList
{
    public WeaponStat[] weaponStats;
}
