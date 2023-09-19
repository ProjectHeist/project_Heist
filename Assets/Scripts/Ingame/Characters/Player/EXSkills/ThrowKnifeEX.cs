using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ThrowKnife", fileName = "ThrowKnifeEX")]
public class ThrowKnifeEX : PlayerEX
{
    // Start is called before the first frame update
    public GameObject target;
    public int damage = 50;
    public override void UseEX(PlayerState ps)
    {
        target.GetComponent<EnemyState>().HP -= damage;
    }
}
