using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMove : MonoBehaviour
{
    // Start is called before the first frame update
    private int currentPathIndex;
    private Vector2Int currentPos;
    public Vector2Int destination;
    public PlayerState playerState;
    public List<Spot> path = null;
    public float moveSpeed = 100.0f;

    void Start()
    {
        playerState = gameObject.GetComponent<PlayerState>();
        Debug.Log(currentPos);
    }

    // Update is called once per frame
    void Update()
    {
        MoveToDest();
    }

    public void SetTargetPosition(Vector2Int targetPosition)
    {
        currentPos = MapManager.Instance.GetGridPositionFromWorld(gameObject.transform.position);
        if (currentPos != targetPosition)
        {
            currentPathIndex = 0;
            Astar astar = new Astar(MapManager.Instance.spots, MapManager.Instance.width, MapManager.Instance.height);
            path = astar.CreatePath(MapManager.Instance.spots, currentPos, targetPosition, 1000);
            path.Reverse();
        }
    }

    public void MoveToDest()
    {
        if (path != null)
        {
            if (path.Count <= playerState.moveRange)
            {
                Vector2Int target = path[currentPathIndex].position;
                Vector3 targetPosition = MapManager.Instance.GetWorldPositionFromGridPosition(target);
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
        }
    }

    public void StopMoving()
    {
        path = null;
        currentPos = MapManager.Instance.GetGridPositionFromWorld(transform.position);
        GameManager.Instance._ui.DeleteRange();
        GameManager.Instance._ui.SelectedState(currentPos);
    }

}
