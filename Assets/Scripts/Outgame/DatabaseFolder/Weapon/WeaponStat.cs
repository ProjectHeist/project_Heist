using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "WeaponStat", menuName = "DataElements/WeaponStat", order = 1)]
public class WeaponStat : ScriptableObject
{
    public int weaponCost;
    public int weaponNumber;

    public string weaponName;

    public int weaponMinAttackRange;

    public int weaponMaxAttackRange;

    public int weaponDamage;

    public float weaponCritRate;
}
