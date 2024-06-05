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
        public float moveSpeed = 20.0f;

        public List<int> suspicion = new List<int>(); // 플레이어별 의심도
        public List<bool> susIncreased = new List<bool>(); // 의심도가 증가하였는가?
        public List<bool> isSuspect = new List<bool>(); // 플레이어별 의심되는 상태 
        public List<bool> wasDetected = new List<bool>(); // 해당 플레이어를 이미 감지한 적이 있는가?

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

        public void suspicionInit()
        {
            for (int i = 0; i < IngameManager.Instance.players.Count; i++)
            {
                suspicion.Add(0);
                susIncreased.Add(false);
                isSuspect.Add(false);
                wasDetected.Add(false);
            }
        }

        public void AddSuspicion(int index, int sus)
        {
            for (int i = 0; i < suspicion.Count; i++)
            {
                if (index == i)
                {
                    suspicion[i] += sus;
                    susIncreased[i] = true;
                    wasDetected[i] = true;
                }
            }
        }
        public void SetSuspicion(bool isSus, int index, int sus)
        {
            for (int i = 0; i < suspicion.Count; i++)
            {
                if (index == i)
                {
                    suspicion[i] = sus;
                    susIncreased[i] = true;
                    if (isSus)
                        isSuspect[i] = true;
                    else
                        isSuspect[i] = false;
                    wasDetected[i] = true;
                }
            }
        }

        public void OnEnemyHit(int damage)
        {
            OnCharacterHit(damage);
        }
    }
}