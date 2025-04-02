using System.Collections;
using System.Collections.Generic;
using Logics;
using UnityEngine;
using Ingame;
using TMPro;
using Unity.VisualScripting;

public class EnemyMove : MonoBehaviour
{
    public bool rotated = false;
    public EnemyState es;
    public EnemyBehaviour eb;
    public EnemyPatternType enemyPatternType;
    public EnemyAnim enemyAnim;
    public bool moving;
    Coroutine move;

    public void StopMove()
    {
        Debug.Log("MoveStop");
        StopCoroutine(move);
        enemyAnim.SetRunning(false);
        moving = false;
        gameObject.GetComponent<EnemyBehaviour>().actFinished = true;
    }
    public void MoveTo(EnemyPattern ep, Vector2Int targetPos)
    {
        GameObject current = gameObject;
        ep.EnemyAct(targetPos, current);
        List<Spot> path = ep.path;
        enemyPatternType = ep.PatternType;
        if (enemyPatternType == EnemyPatternType.Alert)
        {
            for (int i = 0; i < path.Count; i++)
            {
                Debug.Log("path: " + path[i].X + " " + path[i].Y);
            }
        }

        es = gameObject.GetComponent<EnemyState>();
        eb = gameObject.GetComponent<EnemyBehaviour>();
        enemyAnim = gameObject.GetComponent<EnemyAnim>();
        move = StartCoroutine(Move(path));
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
        while (t < 0.2f)
        {
            t += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, startRotation + endRotation, t / 0.2f) % 360.0f;
            model.transform.eulerAngles = new Vector3(model.transform.eulerAngles.x, yRotation, model.transform.eulerAngles.z);
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        rotated = true;
    }


    IEnumerator Move(List<Spot> path)
    {
        //if (enemyPatternType == EnemyPatternType.Alert)
        //Debug.Log("path: " + path.Count);
        EnemyState es = gameObject.GetComponent<EnemyState>();
        MapManager map = IngameManager.Instance.mapManager;
        int currentPathIndex = 0;
        float moveSpeed = es.moveSpeed;
        GameObject suspect = gameObject.GetComponent<EnemyBehaviour>().suspect;
        moving = true;

        while (currentPathIndex < path.Count)
        {
            if (enemyPatternType == EnemyPatternType.Alert)
                Debug.Log("idx: " + currentPathIndex);
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
                if (enemyPatternType == EnemyPatternType.Alert)
                    Debug.Log("엄 준 식");

                gameObject.GetComponent<EnemyVision>().visionMove(target);
                currentPathIndex++;
                if (currentPathIndex >= path.Count)
                {
                    enemyAnim.SetRunning(false);
                    moving = false;
                }
                else if (path[currentPathIndex].Height == 2) //Door
                {
                    GameObject grid = map.GetGridCellFromPosition(new Vector2Int(path[currentPathIndex].X, path[currentPathIndex].Y));
                    grid.GetComponent<GridCell>().CheckObject();
                    GameObject door = grid.GetComponent<GridCell>().objectInThisGrid;
                    StartCoroutine(door.GetComponent<Door>().SlideOpen());
                    yield return new WaitUntil(() => !door.GetComponent<Door>().DoorIsOpening);
                }
            }
            yield return null;
        }
        Vector2Int currentpos = map.GetGridPositionFromWorld(gameObject.transform.position);
        if (enemyPatternType == EnemyPatternType.Return)
        {
            eb.enemyPattern.currentPos = currentpos;
            eb.enemyPattern.UpdateState();
        }
        map.spots[currentpos.x, currentpos.y].z = 1;
        gameObject.GetComponent<EnemyBehaviour>().actFinished = true;
    }
}
