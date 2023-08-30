using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterState : MonoBehaviour
{
    public int HP;
    public int moveRange;
    public float critRate;
    public float accuracy;
    public int damage;
    public int maxAttackRange;
    public int minAttackRange;
    public int canAttack;
    public int canMove;

    public void OnCharacterHit(int damage)
    {
        if (HP - damage <= 0)
        {
            Destroy(gameObject, 0.5f);
        }
        else
        {
            HP -= damage;
        }
    }
}
