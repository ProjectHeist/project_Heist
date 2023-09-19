using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CritUp", fileName = "CRITUpEX")]
public class CRITUpEX : PlayerEX
{
    public float originalRate;
    public override void UseEX(PlayerState ps)
    {
        if (duration != 0)
        {
            originalRate = ps.critRate;
            ps.critRate = 1.0f;
        }
        else
        {
            ps.critRate = originalRate;
        }
    }
}
