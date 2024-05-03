using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logics;
using Ingame;

public class EnemyAlert : MonoBehaviour
{
    public void AlertAndAttack(Vector2Int targetPos)
    {
        EnemyState es = gameObject.GetComponent<EnemyState>();
        MapManager map = IngameManager.Instance.mapManager;
        Vector2Int currentpos = map.GetGridPositionFromWorld(gameObject.transform.position);
        map.spots[targetPos.x, targetPos.y].z = 0;

        Astar astar = new Astar(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height);
        List<Spot> path = astar.CreatePath(map.spots, map.GetGridPositionFromWorld(gameObject.transform.position), targetPos, 1000);
        map.spots[targetPos.x, targetPos.y].z = 1;
        List<Spot> newPath = new List<Spot>();

        path.Reverse();
        map.spots[currentpos.x, currentpos.y].z = 0;
        if (path.Count < es.moveRange)
        {
            for (int i = 0; i < path.Count - 4; i++)
            {
                newPath.Add(path[i]);
            }
        }
        else
        {
            for (int i = 0; i < es.moveRange - 3; i++)
            {
                newPath.Add(path[i]);
            }
        }
        StartCoroutine(Move(newPath));
    }

    IEnumerator Move(List<Spot> path)
    {
        EnemyState es = gameObject.GetComponent<EnemyState>();
        MapManager map = IngameManager.Instance.mapManager;
        int currentPathIndex = 0;
        float moveSpeed = es.moveSpeed;
        GameObject suspect = gameObject.GetComponent<EnemyBehaviour>().suspect;

        while (currentPathIndex < path.Count)
        {
            Vector2Int target = path[currentPathIndex].position;
            Vector3 targetPosition = IngameManager.Instance.mapManager.GetWorldPositionFromGridPosition(target);
            if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;
                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                transform.position = transform.position + moveDir * moveSpeed * Time.deltaTime;
            }
            else
            {
                currentPathIndex++;
            }
            yield return null;
        }
        Vector2Int currentpos = map.GetGridPositionFromWorld(gameObject.transform.position);
        map.spots[currentpos.x, currentpos.y].z = 1;

        if (suspect != null)
        {
            bool InRange = gameObject.GetComponent<EnemyMovement>().InRange(map.GetGridPositionFromWorld(gameObject.transform.position), map.GetGridPositionFromWorld(suspect.transform.position), es.maxAttackRange);
            if (InRange)
            {
                suspect.GetComponent<PlayerState>().OnPlayerHit(es.damage); //Attack
                Debug.Log("Attack");
            }
        }
    }

}
