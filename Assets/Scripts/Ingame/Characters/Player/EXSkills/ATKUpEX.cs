using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AttackUp", fileName = "ATKUpEX")]
public class ATKUpEX : PlayerEX
{
    public override void UseEX(PlayerState ps)
    {
        if (duration != 0)
        {
            ps.damage += 30;
        }
        else
        {
            ps.damage -= 30;
        }
    }
}
