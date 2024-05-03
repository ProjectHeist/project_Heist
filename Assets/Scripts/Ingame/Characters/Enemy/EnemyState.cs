using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Logics;
using UnityEngine;

namespace Ingame
{
    public class EnemyState : CharacterState
    {
        public int detectRange = 8;
        public int routeNum; // 순찰 경로를 나타냄
        public int faceDir; //0 is +x, 1 is +y, 2 is -x, 3 is -y
        public int alertRange = 6;
        public float moveSpeed = 6.0f;
        public void GetEnemyInfo()
        {
            HP = 100;
            accuracy = 0.5f;
            moveRange = 8;
            damage = 20;
            maxAttackRange = 6;
            minAttackRange = 3;
            critRate = 0.2f;
            canAttack = 1;
            canMove = 1;
            faceDir = 2;
        }
        public void OnEnemyHit(int damage)
        {
            OnCharacterHit(damage);
        }
    }
}