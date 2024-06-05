using System.Collections;
using System.Collections.Generic;
using Ingame;
using UnityEngine;
using Logics;

public class EnemyVision : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision) // 시야에 들어왔을 때
    {
        EnemyBehaviour enemyBehaviour = gameObject.GetComponentInParent<EnemyBehaviour>();
        EnemyPatrol enemyPatrol = gameObject.GetComponentInParent<EnemyPatrol>();
        EnemyState es = gameObject.GetComponentInParent<EnemyState>();
        int idx = enemyBehaviour.enemyIndex;
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!IngameManager.Instance.walldetection.IsWallBetween(transform.position, collision.gameObject.transform.position)) // 사이에 벽이 없을 경우
            {
                enemyBehaviour.detectedplayers.Add(collision.gameObject);
                collision.gameObject.GetComponent<PlayerState>().enemyDetectedPlayer.Add(gameObject.transform.parent.gameObject);

                Vector2Int currPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(collision.gameObject.transform.position);
                PlayerState ps = collision.gameObject.GetComponent<PlayerState>();
                int sus = IngameManager.Instance.mapManager.GetSuspicion(currPos); //의심도 체크

                if (!es.wasDetected[ps.playerIndex])
                {
                    if (sus != 0) // 금지구역에 있을 때
                    {
                        es.AddSuspicion(ps.playerIndex, sus);
                    }
                    else if (enemyBehaviour.enemyPattern == EnemyPatternType.Lured)
                    {
                        es.AddSuspicion(ps.playerIndex, 10);
                    }
                }

                if (es.suspicion[ps.playerIndex] >= 100) // 의심가는 인물이 포착될 경우
                {
                    GameObject max = enemyBehaviour.GetMaxSuspicion();
                    if (collision.gameObject == max)
                    {
                        enemyBehaviour.suspect = collision.gameObject;
                        enemyBehaviour.memoryturn = 2;
                        if (enemyPatrol.patrolling)
                            enemyPatrol.StopPatrol();
                        enemyBehaviour.enemyPattern = EnemyPatternType.Alert;
                        enemyBehaviour.AlertOthers();
                    }
                    if (!IngameManager.Instance.spawner.policeSpawn)
                    {
                        IngameManager.Instance.spawner.startspawnTimer(enemyBehaviour.suspect);
                    }
                }
                else if (es.suspicion[ps.playerIndex] >= 50)
                {
                    GameObject max = enemyBehaviour.GetMaxSuspicion();
                    if (collision.gameObject == max)
                    {
                        enemyBehaviour.suspect = collision.gameObject;
                        enemyBehaviour.memoryturn = 2;
                        if (enemyBehaviour.enemyPattern == EnemyPatternType.Patrol || enemyBehaviour.enemyPattern == EnemyPatternType.Guard)
                        {
                            if (enemyPatrol.patrolling)
                                enemyPatrol.StopPatrol();
                            enemyBehaviour.enemyPattern = EnemyPatternType.Chase;
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Wall is between");
            }
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        EnemyBehaviour enemyBehaviour = gameObject.GetComponentInParent<EnemyBehaviour>();
        if (collision.gameObject.CompareTag("Player"))
        {
            enemyBehaviour.detectedplayers.Remove(collision.gameObject);
            collision.gameObject.GetComponent<PlayerState>().enemyDetectedPlayer.Remove(gameObject);
        }
    }
}
