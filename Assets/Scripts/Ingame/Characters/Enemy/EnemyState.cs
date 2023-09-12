using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class EnemyState : CharacterState
{
    public int detectRange = 8;
    public int faceDir; //0 is +x, 1 is -x, 2 is +y, 3 is -y
    public void GetEnemyInfo()
    {
        HP = 100;
        accuracy = 0.5f;
        moveRange = 5;
        damage = 20;
        maxAttackRange = 6;
        minAttackRange = 3;
        critRate = 0.2f;
        canAttack = 1;
        canMove = 1;
        faceDir = 1;
    }
    public void OnEnemyHit(int damage)
    {
        OnCharacterHit(damage);
    }
}
