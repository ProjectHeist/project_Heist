using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Ingame
{
    public class PlayerAttack : MonoBehaviour
    {
        // Start is called before the first frame update
        public void AttackTarget(GameObject target, int dist)
        {
            PlayerState playerState = gameObject.GetComponent<PlayerState>();
            float finalaccuracy = 0.0f;
            if (dist <= playerState.minAttackRange)
            {
                finalaccuracy = 1.0f;
            }
            else
            {
                finalaccuracy = playerState.accuracy + (float)(playerState.maxAttackRange - dist) / (playerState.maxAttackRange - playerState.minAttackRange) * (1.0f - playerState.accuracy);
            }
            float rand = UnityEngine.Random.Range(0.0f, 1.0f);
            float _rand = UnityEngine.Random.Range(0.0f, 1.0f);
            if (rand <= finalaccuracy)
            {
                if (_rand <= playerState.critRate)
                {
                    Debug.Log("Critical Hit");
                    target.GetComponent<EnemyState>().OnEnemyHit(playerState.damage * 2);
                }
                else
                {
                    Debug.Log("Attack Hit");
                    target.GetComponent<EnemyState>().OnEnemyHit(playerState.damage);
                }
            }
            else
            {
                Debug.Log("Attack Missed");
            }

        }
    }
}

