using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logics;

namespace Ingame
{
    public class PlayerMove : MonoBehaviour
    {
        // Start is called before the first frame update
        private int currentPathIndex;
        private bool arrived;
        private Vector2Int currentPos;
        public Vector2Int destination;
        public PlayerState playerState;
        public PlayerAnim playerAnim;
        public List<Spot> path = null;
        public float moveSpeed = 100.0f;
        public bool rotated;

        // Update is called once per frame

        public void MoveToDest(Vector2Int targetPosition)
        {
            arrived = false;
            playerAnim = gameObject.GetComponent<PlayerAnim>();
            playerState = gameObject.GetComponent<PlayerState>();
            currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(gameObject.transform.position);
            currentPathIndex = 0;
            Astar astar = new Astar(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height);
            path = astar.CreatePath(IngameManager.Instance.mapManager.spots, currentPos, targetPosition, 1000, true);
            if (path != null)
            {
                path.Reverse();
                StartCoroutine(Move());
            }
            else
            {
                Debug.Log("Cannot go to Position");
                IngameManager.Instance.ingameUI.range.Delete(new Vector2Int(-1, -1));
                IngameManager.Instance.ingameUI.range.SelectedState(currentPos);
            }
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
                        playerState.faceDir = 0;
                        StartCoroutine(Rotate(90.0f));
                        break;
                    case 2: // -x to +x
                        playerState.faceDir = 0;
                        StartCoroutine(Rotate(180.0f));
                        break;
                    case 3: // -y to +x
                        playerState.faceDir = 0;
                        StartCoroutine(Rotate(-90.0f));
                        break;
                }
            }
            else if (pathdir.Equals(new Vector2Int(-1, 0)))
            {
                switch (currdir)
                {
                    case 0: // +x to -x
                        playerState.faceDir = 2;
                        StartCoroutine(Rotate(180.0f));
                        break;
                    case 1: // +y to -x
                        playerState.faceDir = 2;
                        StartCoroutine(Rotate(-90.0f));
                        break;
                    case 2: // -x to -x
                        rotated = true;
                        break;
                    case 3: // -y to -x
                        playerState.faceDir = 2;
                        StartCoroutine(Rotate(90.0f));
                        break;
                }
            }
            else if (pathdir.Equals(new Vector2Int(0, 1)))
            {
                switch (currdir)
                {
                    case 0: // +x to +y
                        playerState.faceDir = 1;
                        StartCoroutine(Rotate(-90.0f));
                        break;
                    case 1: // +y to +y
                        rotated = true;
                        break;
                    case 2: // -x to +y
                        playerState.faceDir = 1;
                        StartCoroutine(Rotate(90.0f));
                        break;
                    case 3: // -y to +y
                        playerState.faceDir = 1;
                        StartCoroutine(Rotate(180.0f));
                        break;
                }
            }
            else if (pathdir.Equals(new Vector2Int(0, -1)))
            {
                switch (currdir)
                {
                    case 0: // +x to -y
                        playerState.faceDir = 3;
                        StartCoroutine(Rotate(90.0f));
                        break;
                    case 1: // +y to -y
                        playerState.faceDir = 3;
                        StartCoroutine(Rotate(180.0f));
                        break;
                    case 2: // -x to -y
                        playerState.faceDir = 3;
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
            playerAnim.SetRunning(false);
            GameObject model = playerState.playerModel;
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

        IEnumerator Move()
        {
            Debug.Log(path.Count);
            Debug.Log(playerState.remainMoveRange);
            if (path.Count - 1 <= playerState.remainMoveRange)
            {
                int temp = playerState.remainMoveRange;
                IngameManager.Instance.mapManager.spots[currentPos.x, currentPos.y].z = 0;
                if (playerState.remainMoveRange == path.Count - 1)
                {
                    gameObject.GetComponent<PlayerState>().canMove--;
                    IngameManager.Instance.ingameUI.IsSelected(PanelType.Move, false);
                }
                playerState.remainMoveRange -= path.Count - 1;
                while (!arrived)
                {
                    if (path.Count - 1 <= temp)
                    {
                        rotated = false;
                        Vector2Int target = path[currentPathIndex].position;
                        Vector3 targetPosition = IngameManager.Instance.mapManager.GetWorldPositionFromGridPosition(target);
                        Vector3 moveDir = (targetPosition - transform.position).normalized; // 타겟을 설정하고
                        Vector2Int mD = amplify(moveDir);

                        SetDir(playerState.faceDir, mD); // 먼저 회전하고
                        yield return new WaitUntil(() => rotated); // 회전할 때까지 기다린다

                        playerAnim.SetRunning(true);
                        if (Vector3.Distance(transform.position, targetPosition) > 0.05f) // 타일 1개당 이동하는 알고리즘
                        {
                            float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                            transform.position = transform.position + moveDir * moveSpeed * Time.deltaTime;
                        }
                        else
                        {
                            currentPathIndex++;
                            if (currentPathIndex >= path.Count)
                            {
                                StopMoving();
                            }
                        }
                    }
                    yield return null;
                }

            }
            else
            {
                Debug.Log("Cannot go to Position");
                IngameManager.Instance.ingameUI.range.Delete(new Vector2Int(-1, -1));
                IngameManager.Instance.ingameUI.range.SelectedState(currentPos);
            }

        }

        public Vector2Int amplify(Vector3 dir)
        {
            if (dir.x > 0.5f)
            {
                return new Vector2Int(1, 0);
            }
            else if (dir.z > 0.5f)
            {
                return new Vector2Int(0, 1);
            }
            else if (dir.x < -0.5f)
            {
                return new Vector2Int(-1, 0);
            }
            else if (dir.z < -0.5f)
            {
                return new Vector2Int(0, -1);
            }
            else
            {
                return new Vector2Int(0, 0);
            }
        }


        public void StopMoving()
        {
            path = null;
            currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(transform.position);
            IngameManager.Instance.mapManager.spots[currentPos.x, currentPos.y].z = 1;
            IngameManager.Instance.ingameUI.range.Delete(new Vector2Int(-1, -1));
            IngameManager.Instance.ingameUI.range.SelectedState(currentPos);
            playerAnim.SetRunning(false);
            arrived = true;
        }

    }
}