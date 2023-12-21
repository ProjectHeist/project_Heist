using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ingame
{
    [CreateAssetMenu(menuName = "ThrowKnife", fileName = "ThrowKnifeEX")]
    public class ThrowKnifeEX : PlayerEX
    {
        // Start is called before the first frame update
        public int damage = 50;
        public override void UseEX(PlayerState ps)
        {
            target.GetComponent<EnemyState>().OnEnemyHit(damage);
        }
    }
}

