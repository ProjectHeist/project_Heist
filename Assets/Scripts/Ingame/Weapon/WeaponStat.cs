[System.Serializable]
public class WeaponStat
{
    public int weaponCost;
    public int weaponNumber;

    public string weaponName;

    public int weaponMinAttackRange;

    public int weaponMaxAttackRange;

    public int weaponDamage;

    public float weaponCritRate;

    public WeaponStat()
    {
        weaponMinAttackRange = 0;
        weaponMaxAttackRange = 0;
        weaponDamage = 0;
        weaponCritRate = 0;
    }

}
