using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlState
{
    Default,
    PlayerMove,
    PlayerAttack,
    PlayerInteract
}
public class PlayerController
{
    // Start is called before the first frame update
    public ControlState currentState;

    // variables
    public GameObject currentPlayer;
    private PlayerMove playerMove;

    // Update is called once per frame
    public void OnMouseClick(Vector2Int GridPos)
    {
        Astar astar = new Astar(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height);
        switch (currentState) // 조작 상태에 따라
        {
            case ControlState.Default:
                break;
            case ControlState.PlayerMove: // 해당 그리드로 플레이어가 갈지
                playerMove = currentPlayer.GetComponent<PlayerMove>();
                playerMove.SetTargetPosition(GridPos);
                currentState = ControlState.Default;
                break;
            case ControlState.PlayerAttack: // 해당 그리드에 있는 적을 공격할지
                Vector2Int playerPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(currentPlayer.transform.position);
                int dist = Math.Abs(GridPos.x - playerPos.x) + Math.Abs(GridPos.y - playerPos.y);

                GridCell gridcell = IngameManager.Instance.mapManager.GetGridCellFromPosition(GridPos).GetComponent<GridCell>();
                gridcell.CheckEnemy();
                GameObject enemy = gridcell.enemyInThisGrid;

                decideAttack(enemy, dist);
                break;
            case ControlState.PlayerInteract: // 해당 그리드에 있는 물체와 상호작용할지
                Vector2Int _playerPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(currentPlayer.transform.position);
                int distance = Math.Abs(GridPos.x - _playerPos.x) + Math.Abs(GridPos.y - _playerPos.y);

                GridCell _gridcell = IngameManager.Instance.mapManager.GetGridCellFromPosition(GridPos).GetComponent<GridCell>();
                _gridcell.CheckObject();
                GameObject _object = _gridcell.objectInThisGrid;

                decideInteract(_object, distance);
                break;
        }
    }

    public void decideInteract(GameObject interactObject, int dist)
    {
        if (dist <= 2)
        {
            if (interactObject != null)
            {
                PlayerInteract playerInteract = currentPlayer.GetComponent<PlayerInteract>();
                playerInteract.Interact(interactObject, dist);
                currentState = ControlState.Default;
                GameManager.Instance._ui.DeleteRange();
            }
            else
            {
                Debug.Log("Please Select object to interact");
            }
        }
        else
        {
            Debug.Log("Out of Range");
        }
    }

    public void decideAttack(GameObject enemy, int dist)
    {
        if (dist <= currentPlayer.GetComponent<PlayerState>().maxAttackRange)
        {
            if (enemy != null)
            {
                PlayerAttack playerAttack = currentPlayer.GetComponent<PlayerAttack>();
                playerAttack.AttackTarget(enemy, dist);
                currentState = ControlState.Default;
                GameManager.Instance._ui.DeleteRange();
            }
            else
            {
                Debug.Log("Please Select enemy to attack");
            }
        }
        else
        {
            Debug.Log("Out of range");
        }

    }




}
