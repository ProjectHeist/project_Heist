using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelect : MonoBehaviour
{
    public int index;
    public void SelectWeapon()
    {
        PrepareManager.Instance.addWeaponTo = index;
        PrepareManager.Instance.addPlayerTo = -1;
    }
}
