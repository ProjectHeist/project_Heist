using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logics;
using Ingame;

public class EnemyPatrol : MonoBehaviour
{
    public bool rotated = false;
    public EnemyAnim enemyAnim;
    public EnemyState es;
    public EnemyBehaviour eb;
    public int currentRoute = 0;
    public bool patrolling = false;
    Coroutine patrol;
    public void Patrol()
    {
        es = gameObject.GetComponent<EnemyState>();
        enemyAnim = gameObject.GetComponent<EnemyAnim>();
        eb = gameObject.GetComponent<EnemyBehaviour>();
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
            rotated = false;
            Vector2Int target = path[currentPathIndex];
            Vector3 targetPosition = IngameManager.Instance.mapManager.GetWorldPositionFromGridPosition(target);
            Vector3 moveDir = (targetPosition - transform.position).normalized; // 타겟을 설정하고
            Vector2Int mD = eb.em.amplify(moveDir);

            SetDir(es.faceDir, mD); // 먼저 회전하고
            yield return new WaitUntil(() => rotated); // 회전할 때까지 기다린다

            enemyAnim.SetRunning(true);
            if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = transform.position + moveDir * speed * Time.deltaTime;
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= path.Count)
                {
                    patrolling = false;
                    enemyAnim.SetRunning(false);
                    break;
                }
            }
            yield return null;
        }
        gameObject.GetComponent<EnemyBehaviour>().actFinished = true;
    }

    public void StopPatrol() //즉시 행동을 멈추는 함수. 상태 전환에 주로 사용
    {
        StopCoroutine(patrol);
        enemyAnim.SetRunning(false);
        patrolling = false;
        gameObject.GetComponent<EnemyBehaviour>().actFinished = true;
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

    public void SetDir(int currdir, Vector2Int pathdir)
    {
        if (pathdir.Equals(new Vector2Int(1, 0)))
        {
            switch (currdir)
            {
                case 0:  // +x to +x
                    rotated = true;
                    break;
                case 1:  // +y to +x
                    es.faceDir = 0;
                    StartCoroutine(Rotate(90.0f));
                    break;
                case 2: // -x to +x
                    es.faceDir = 0;
                    StartCoroutine(Rotate(180.0f));
                    break;
                case 3: // -y to +x
                    es.faceDir = 0;
                    StartCoroutine(Rotate(-90.0f));
                    break;
            }
        }
        else if (pathdir.Equals(new Vector2Int(-1, 0)))
        {
            switch (currdir)
            {
                case 0: // +x to -x
                    es.faceDir = 2;
                    StartCoroutine(Rotate(180.0f));
                    break;
                case 1: // +y to -x
                    es.faceDir = 2;
                    StartCoroutine(Rotate(-90.0f));
                    break;
                case 2: // -x to -x
                    rotated = true;
                    break;
                case 3: // -y to -x
                    es.faceDir = 2;
                    StartCoroutine(Rotate(90.0f));
                    break;
            }
        }
        else if (pathdir.Equals(new Vector2Int(0, 1)))
        {
            switch (currdir)
            {
                case 0: // +x to +y
                    es.faceDir = 1;
                    StartCoroutine(Rotate(-90.0f));
                    break;
                case 1: // +y to +y
                    rotated = true;
                    break;
                case 2: // -x to +y
                    es.faceDir = 1;
                    StartCoroutine(Rotate(90.0f));
                    break;
                case 3: // -y to +y
                    es.faceDir = 1;
                    StartCoroutine(Rotate(180.0f));
                    break;
            }
        }
        else if (pathdir.Equals(new Vector2Int(0, -1)))
        {
            switch (currdir)
            {
                case 0: // +x to -y
                    es.faceDir = 3;
                    StartCoroutine(Rotate(90.0f));
                    break;
                case 1: // +y to -y
                    es.faceDir = 3;
                    StartCoroutine(Rotate(180.0f));
                    break;
                case 2: // -x to -y
                    es.faceDir = 3;
                    StartCoroutine(Rotate(-90.0f));
                    break;
                case 3: // -y to -y
                    rotated = true;
                    break;
            }
        }
        else
        {
            rotated = true;
        }
    }

    IEnumerator Rotate(float endRotation)
    {
        enemyAnim.SetRunning(false);
        GameObject model = eb.enemyModel;
        float startRotation = model.transform.eulerAngles.y;
        float t = 0.0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, startRotation + endRotation, t / 0.5f) % 360.0f;
            model.transform.eulerAngles = new Vector3(model.transform.eulerAngles.x, yRotation, model.transform.eulerAngles.z);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        rotated = true;
    }


    IEnumerator Move(List<Spot> path, Vector2Int dest)
    {
        MapManager map = IngameManager.Instance.mapManager;
        int currentPathIndex = 0;
        float moveSpeed = gameObject.GetComponent<EnemyState>().moveSpeed;
        while (currentPathIndex < path.Count)
        {
            rotated = false;
            Vector2Int target = path[currentPathIndex].position;
            Vector3 targetPosition = IngameManager.Instance.mapManager.GetWorldPositionFromGridPosition(target);
            Vector3 moveDir = (targetPosition - transform.position).normalized; // 타겟을 설정하고
            Vector2Int mD = eb.em.amplify(moveDir);

            SetDir(es.faceDir, mD); // 먼저 회전하고
            yield return new WaitUntil(() => rotated); // 회전할 때까지 기다린다

            enemyAnim.SetRunning(true);
            if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                //Vector3 moveDir = (targetPosition - transform.position).normalized;
                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                transform.position = transform.position + moveDir * moveSpeed * Time.deltaTime;
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= path.Count)
                {
                    enemyAnim.SetRunning(false);
                }
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

        gameObject.GetComponent<EnemyBehaviour>().actFinished = true;
    }
}
