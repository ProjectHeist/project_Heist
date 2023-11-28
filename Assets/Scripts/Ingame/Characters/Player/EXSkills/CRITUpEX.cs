using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CritUp", fileName = "CRITUpEX")]
public class CRITUpEX : PlayerEX
{
    public override void UseEX(PlayerState ps)
    {
        float diff = 1.0f - ps.critRate;
        ps.critRate = 1.0f;
        ps.buffs.Add(new BuffInfo(stats.critRate, duration, diff));
    }
}
