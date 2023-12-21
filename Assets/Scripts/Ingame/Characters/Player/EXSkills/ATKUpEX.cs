using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ingame
{
    [CreateAssetMenu(menuName = "AttackUp", fileName = "ATKUpEX")]
    public class ATKUpEX : PlayerEX
    {
        public override void UseEX(PlayerState ps)
        {
            float diff = 30.0f;
            ps.damage += 30;
            ps.StateChangeList.Add(new BuffInfo(stats.damage, duration, diff));
        }
    }
}

