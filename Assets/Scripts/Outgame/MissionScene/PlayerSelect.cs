using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelect : MonoBehaviour
{
    // Start is called before the first frame update
    public int index;
    public void SelectPlayer()
    {
        PrepareManager.Instance.addPlayerTo = index;
        PrepareManager.Instance.addWeaponTo = -1;
    }
}
