using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logics;
using Ingame;

public class EnemyAlert : MonoBehaviour
{
    public bool rotated = false;
    public EnemyAnim enemyAnim;
    public EnemyState es;
    public EnemyBehaviour eb;
    public void AlertAndAttack(Vector2Int targetPos)
    {
        es = gameObject.GetComponent<EnemyState>();
        enemyAnim = gameObject.GetComponent<EnemyAnim>();
        eb = gameObject.GetComponent<EnemyBehaviour>();
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


    IEnumerator Move(List<Spot> path)
    {
        EnemyState es = gameObject.GetComponent<EnemyState>();
        MapManager map = IngameManager.Instance.mapManager;
        int currentPathIndex = 0;
        float moveSpeed = es.moveSpeed;
        GameObject suspect = gameObject.GetComponent<EnemyBehaviour>().suspect;

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

        if (suspect != null)
        {
            bool InRange = gameObject.GetComponent<EnemyMovement>().InRange(map.GetGridPositionFromWorld(gameObject.transform.position), map.GetGridPositionFromWorld(suspect.transform.position), es.maxAttackRange);
            if (InRange)
            {
                suspect.GetComponent<PlayerState>().OnPlayerHit(es.damage); //Attack
                Debug.Log("Attack");
            }
        }
        gameObject.GetComponent<EnemyBehaviour>().actFinished = true;
    }

}
