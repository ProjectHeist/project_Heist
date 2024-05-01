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
        int idx = enemyBehaviour.enemyIndex;
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!IngameManager.Instance.walldetection.IsWallBetween(transform.position, collision.gameObject.transform.position)) // 사이에 벽이 없을 경우
            {
                Debug.Log("엄준식");
                enemyBehaviour.detectedplayers.Add(collision.gameObject);
                Vector2Int currPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(collision.gameObject.transform.position);
                PlayerState ps = collision.gameObject.GetComponent<PlayerState>();
                int sus = IngameManager.Instance.mapManager.GetSuspicion(currPos); //의심도 체크
                if (!ps.detected)
                {
                    if (sus != 0) // 금지구역에 있을 때
                    {
                        collision.gameObject.GetComponent<PlayerState>().suspicion[idx] += sus;
                        ps.detected = true;
                    }
                    else if (enemyBehaviour.enemyPattern == EnemyPattern.Lured)
                    {
                        collision.gameObject.GetComponent<PlayerState>().suspicion[idx] += 50;
                        ps.detected = true;
                    }
                }
                if (collision.gameObject.GetComponent<PlayerState>().suspicion[idx] >= 100) // 의심가는 인물이 포착될 경우
                {
                    GameObject max = enemyBehaviour.GetMaxSuspicion();
                    if (collision.gameObject == max)
                    {
                        enemyBehaviour.suspect = collision.gameObject;
                        enemyBehaviour.memoryturn = 2;
                        if (enemyBehaviour.patrolling)
                            enemyBehaviour.StopPatrol();
                        enemyBehaviour.enemyPattern = EnemyPattern.Alert;
                        enemyBehaviour.AlertOthers();
                    }
                }
                else if (collision.gameObject.GetComponent<PlayerState>().suspicion[idx] >= 50)
                {
                    GameObject max = enemyBehaviour.GetMaxSuspicion();
                    if (collision.gameObject == max)
                    {
                        enemyBehaviour.suspect = collision.gameObject;
                        enemyBehaviour.memoryturn = 2;
                        if (enemyBehaviour.enemyPattern == EnemyPattern.Patrol || enemyBehaviour.enemyPattern == EnemyPattern.Guard)
                        {
                            if (enemyBehaviour.patrolling)
                                enemyBehaviour.StopPatrol();
                            enemyBehaviour.enemyPattern = EnemyPattern.Chase;
                            enemyBehaviour.AlertOthers();
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
            Debug.Log("김찬호");
            enemyBehaviour.detectedplayers.Remove(collision.gameObject);
        }
    }
}
