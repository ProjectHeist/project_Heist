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

    IEnumerator Move()
    {
        if (path.Count <= playerState.moveRange)
        {
            IngameManager.Instance.mapManager.spots[currentPos.x, currentPos.y].z = 0;
            gameObject.GetComponent<PlayerState>().canMove--;

            if (gameObject.GetComponent<PlayerState>().canMove == 0)
            {
                IngameManager.Instance.ingameUI.IsSelected(PanelType.Move, false);
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
    public void StopMoving()
    {
        path = null;
        currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(transform.position);
        IngameManager.Instance.mapManager.spots[currentPos.x, currentPos.y].z = 1;
        IngameManager.Instance.ingameUI.range.Delete(new Vector2Int(-1, -1));
        IngameManager.Instance.ingameUI.range.SelectedState(currentPos);
        arrived = true;
    }

}
