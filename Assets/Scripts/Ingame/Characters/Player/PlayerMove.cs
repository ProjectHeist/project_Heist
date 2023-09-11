using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMove : MonoBehaviour
{
    // Start is called before the first frame update
    private int currentPathIndex;
    private bool arrived;
    private Vector2Int currentPos;
    public Vector2Int destination;
    public PlayerState playerState;
    public List<Spot> path = null;
    public float moveSpeed = 100.0f;

    // Update is called once per frame

    public void MoveToDest(Vector2Int targetPosition)
    {
        arrived = false;
        playerState = gameObject.GetComponent<PlayerState>();
        currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(gameObject.transform.position);
        currentPathIndex = 0;
        Astar astar = new Astar(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height);
        path = astar.CreatePath(IngameManager.Instance.mapManager.spots, currentPos, targetPosition, 1000);
        path.Reverse();
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        if (path.Count <= playerState.moveRange)
        {
            gameObject.GetComponent<PlayerState>().canMove--;
            IngameManager.Instance.controlPanel.DisplayMove(true, false);
            if (gameObject.GetComponent<PlayerState>().canMove == 0)
            {
                IngameManager.Instance.controlPanel.DisplayMove(false, false);
            }
        }
        while (!arrived)
        {
            if (path.Count <= playerState.moveRange)
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
                    if (currentPathIndex >= path.Count)
                    {
                        StopMoving();
                    }

                }
            }
            else
            {
                Debug.Log("Can't go to position");
                StopMoving();
            }
            yield return null;
        }


    }

    public void StopMoving()
    {
        path = null;
        currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(transform.position);
        GameManager.Instance._ui.DeleteRange();
        GameManager.Instance._ui.SelectedState(currentPos);
        arrived = true;
    }

}
