using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class EnemyState : CharacterState
{
    public void GetEnemyInfo()
    {
        HP = 100;
        accuracy = 0.5f;
        moveRange = 10;
        damage = 20;
        maxAttackRange = 12;
        minAttackRange = 6;
        critRate = 0.2f;
        canAttack = 1;
        canMove = 1;
    }
    public void OnEnemyHit(int damage)
    {
        OnCharacterHit(damage);
    }
    void Awake()
    {
        GetEnemyInfo();
    }
}
