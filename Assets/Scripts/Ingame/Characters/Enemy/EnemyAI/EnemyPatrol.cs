using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logics;
using Ingame;

public class EnemyPatrol : MonoBehaviour
{
    public int currentRoute = 0;
    public bool patrolling = false;
    Coroutine patrol;
    public void Patrol()
    {
        EnemyState es = gameObject.GetComponent<EnemyState>();
        if (es.routeNum != -1)
        {
            PatrolRoute route = GameManager.Instance._data.totalDB.mapDatabase.MapDataList[GameManager.Instance.mapIndex].patrolRoutes[es.routeNum]; // 루트 가져오고
            var currRoute = route.routes[currentRoute];

            List<Vector2Int> path = new List<Vector2Int>();
            Vector2Int currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(transform.position);
            for (int i = 0; i < currRoute.direction.Count; i++)
            {
                Vector2Int moveTo = gameObject.GetComponent<EnemyMovement>().GetModifiedDist(currRoute.direction[i], currRoute.movedist[i]);
                path.Add(currentPos + moveTo);
                currentPos += moveTo;
            }
            patrol = StartCoroutine(Patrolling(path));
            if (currentRoute == route.routes.Count - 1)
            {
                currentRoute = 0;
            }
            else
            {
                currentRoute++;
            }
        }
    }

    IEnumerator Patrolling(List<Vector2Int> path)
    {
        bool arrived = false;
        int currentPathIndex = 0;
        float speed = gameObject.GetComponent<EnemyState>().moveSpeed;
        patrolling = true;
        while (!arrived)
        {
            Vector2Int targetPos = path[currentPathIndex];
            Vector3 targetPosition = IngameManager.Instance.mapManager.GetWorldPositionFromGridPosition(targetPos);
            if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;
                transform.position = transform.position + moveDir * speed * Time.deltaTime;
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= path.Count)
                {
                    patrolling = false;
                    break;
                }
            }
            yield return null;
        }
    }

    public void StopPatrol() //즉시 행동을 멈추는 함수. 상태 전환에 주로 사용
    {
        StopCoroutine(patrol);
        patrolling = false;
    }

    public void ReturnToPatrol()
    {
        EnemyBehaviour eb = gameObject.GetComponent<EnemyBehaviour>();
        EnemyState es = gameObject.GetComponent<EnemyState>();
        Vector2Int returnPos = GameManager.Instance._data.totalDB.mapDatabase.MapDataList[GameManager.Instance.mapIndex].enemyPos[eb.enemyIndex];

        MapManager map = IngameManager.Instance.mapManager;
        Astar astar = new Astar(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height);
        List<Spot> path = astar.CreatePath(map.spots, map.GetGridPositionFromWorld(gameObject.transform.position), returnPos, 1000);

        List<Spot> newPath = new List<Spot>();
        path.Reverse();
        if (path.Count < es.moveRange)
        {
            for (int i = 0; i < path.Count; i++)
            {
                newPath.Add(path[i]);
            }
        }
        else
        {
            for (int i = 0; i < es.moveRange; i++)
            {
                newPath.Add(path[i]);
            }
        }
        StartCoroutine(Move(newPath, returnPos));
    }

    IEnumerator Move(List<Spot> path, Vector2Int dest)
    {
        MapManager map = IngameManager.Instance.mapManager;
        int currentPathIndex = 0;
        float moveSpeed = gameObject.GetComponent<EnemyState>().moveSpeed;
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

        if (currentpos.Equals(dest))
        {
            currentRoute = 0;
            gameObject.GetComponent<EnemyBehaviour>().enemyPattern = EnemyPattern.Patrol;
            Debug.Log("엄준식은 살아있다");
        }
    }
}
