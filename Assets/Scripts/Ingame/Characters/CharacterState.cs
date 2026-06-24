using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logics;

public enum stats
{
    HP,
    accuracy,
    moveRange,
    damage,
    attackRange,
    critRate
}

namespace Ingame
{
    public class BuffInfo
    {
        public stats stat;
        public int duration;
        public float value;
        public BuffInfo(stats s, int d, float v)
        {
            stat = s;
            duration = d;
            value = v;
        }
    }
    public class CharacterState : MonoBehaviour
    {
        public int maxHP;
        public int HP;
        public int moveRange;
        public float critRate;
        public float accuracy;
        public int damage;
        public int maxAttackRange;
        public int minAttackRange;
        public int canAttack;
        public int canMove;

        public List<BuffInfo> StateChangeList = new();

        public void OnCharacterHit(int damage)
        {
            if (HP - damage <= 0)
            {
                OnCharacterDeath();
            }
            else
            {
                HP -= damage;
                gameObject.GetComponentInChildren<HPBar>().SetValue(HP);
            }
        }

        public virtual void OnCharacterDeath()
        {
            gameObject.SetActive(false);
        }
    }
}

