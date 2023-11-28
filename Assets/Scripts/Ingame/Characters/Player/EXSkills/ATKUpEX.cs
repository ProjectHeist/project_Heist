using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackUp", fileName = "ATKUpEX")]
public class ATKUpEX : PlayerEX
{
    public override void UseEX(PlayerState ps)
    {
        float diff = 30.0f;
        ps.damage += 30;
        ps.buffs.Add(new BuffInfo(stats.damage, duration, diff));
    }
}
