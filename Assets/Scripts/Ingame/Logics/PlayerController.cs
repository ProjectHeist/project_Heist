using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ingame;

namespace Logics
{
    public enum ControlState
    {
        Default,
        PlayerMove,
        PlayerAttack,
        PlayerInteract,
        PlayerEX
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
                    decideMove(GridPos);
                    IngameManager.Instance.ingameUI.DeselectPanel(PanelType.Move);
                    break;
                case ControlState.PlayerAttack: // 해당 그리드에 있는 적을 공격할지
                    Vector2Int playerPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(currentPlayer.transform.position);
                    int dist = Math.Abs(GridPos.x - playerPos.x) + Math.Abs(GridPos.y - playerPos.y);

                    GridCell gridcell = IngameManager.Instance.mapManager.GetGridCellFromPosition(GridPos).GetComponent<GridCell>();
                    gridcell.CheckEnemy();
                    GameObject enemy = gridcell.enemyInThisGrid;

                    decideAttack(enemy, dist, playerPos);
                    IngameManager.Instance.ingameUI.DeselectPanel(PanelType.Attack);
                    break;
                case ControlState.PlayerInteract: // 해당 그리드에 있는 물체와 상호작용할지
                    Vector2Int _playerPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(currentPlayer.transform.position);
                    int distance = Math.Abs(GridPos.x - _playerPos.x) + Math.Abs(GridPos.y - _playerPos.y);

                    GridCell _gridcell = IngameManager.Instance.mapManager.GetGridCellFromPosition(GridPos).GetComponent<GridCell>();
                    _gridcell.CheckObject();
                    GameObject _object = _gridcell.objectInThisGrid;

                    decideInteract(_object, distance);
                    IngameManager.Instance.ingameUI.DeselectPanel(PanelType.Interact);
                    break;
                case ControlState.PlayerEX:
                    PlayerState ps = currentPlayer.GetComponent<PlayerState>();
                    PlayerEX ex = GameManager.Instance.playerEXes[ps.EXIndex];
                    decideEX(GridPos, ex);
                    break;
            }
        }

        public void decideMove(Vector2Int position)
        {
            if (IngameManager.Instance.mapManager.GetGridPositionFromWorld(currentPlayer.transform.position) != position && IngameManager.Instance.mapManager.spots[position.x, position.y].z == 0)
            {
                playerMove.MoveToDest(position);
                currentState = ControlState.Default;
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
                    IngameManager.Instance.ingameUI.range.Delete(new Vector2Int(-1, -1));

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

        public void decideAttack(GameObject enemy, int dist, Vector2Int playerPos)
        {
            if (dist <= currentPlayer.GetComponent<PlayerState>().maxAttackRange)
            {
                if (enemy != null)
                {
                    PlayerAttack playerAttack = currentPlayer.GetComponent<PlayerAttack>();
                    playerAttack.AttackTarget(enemy, dist);
                    currentPlayer.GetComponent<PlayerState>().canAttack--;
                    currentState = ControlState.Default;
                    IngameManager.Instance.ingameUI.range.Delete(new Vector2Int(-1, -1));
                    IngameManager.Instance.ingameUI.DeselectPanel(PanelType.Attack);
                    if (currentPlayer.GetComponent<PlayerState>().canAttack == 0)
                    {
                        IngameManager.Instance.ingameUI.IsSelected(PanelType.Attack, false);
                    }
                }
                else
                {
                    Debug.Log("Please Select enemy to attack");
                }
                currentPlayer.GetComponent<PlayerSound>().Notify(playerPos, currentPlayer.GetComponent<PlayerState>().soundRange);
            }
            else
            {
                Debug.Log("Out of range");
            }

        }

        public void decideEX(Vector2Int gridpos, PlayerEX ex)
        {
            if (ex.range != -1) // 특수 스킬
            {
                Vector2Int playerPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(currentPlayer.transform.position);
                int dist = Math.Abs(gridpos.x - playerPos.x) + Math.Abs(gridpos.y - playerPos.y);
                if (ex.subject == 1 && dist <= ex.range) //적 대상이고, 범위 내 그리드를 클릭했을 때
                {
                    GridCell gridcell = IngameManager.Instance.mapManager.GetGridCellFromPosition(gridpos).GetComponent<GridCell>();
                    gridcell.CheckEnemy();
                    if (gridcell.enemyInThisGrid != null) //그리드 내 실제로 적이 있을 때
                    {
                        ex.target = gridcell.enemyInThisGrid;
                        ex.UseEX(currentPlayer.GetComponent<PlayerState>());
                        currentPlayer.GetComponent<PlayerState>().EXcooldown = ex.coolTime;
                        currentState = ControlState.Default;
                        if (currentPlayer.GetComponent<PlayerState>().EXcooldown > 0)
                        {
                            IngameManager.Instance.ingameUI.IsSelected(PanelType.EX, false);
                        }
                        IngameManager.Instance.ingameUI.DeselectPanel(PanelType.EX);
                    }
                    else
                    {
                        Debug.Log("Please Select enemy to EX");
                    }
                }
                else if (ex.subject == 0 && dist <= ex.range) //플레이어 대상이고, 범위 내 그리드를 클릭했을 때
                {
                    GridCell gridcell = IngameManager.Instance.mapManager.GetGridCellFromPosition(gridpos).GetComponent<GridCell>();
                    gridcell.CheckPlayer();
                    if (gridcell.playerInThisGrid != null) //그리드 내 실제로 플레이어가 있을 때
                    {
                        ex.target = gridcell.playerInThisGrid;
                        ex.UseEX(currentPlayer.GetComponent<PlayerState>());
                        currentPlayer.GetComponent<PlayerState>().EXcooldown = ex.coolTime;
                        currentState = ControlState.Default;
                        if (currentPlayer.GetComponent<PlayerState>().EXcooldown > 0)
                        {
                            IngameManager.Instance.ingameUI.IsSelected(PanelType.EX, false);
                        }
                        IngameManager.Instance.ingameUI.DeselectPanel(PanelType.EX);
                    }
                    else
                    {
                        Debug.Log("Please Select player to EX");
                    }
                }
                else if (ex.subject == -1 && dist <= ex.range)
                {
                    ex.targetPosition = gridpos;
                    ex.UseEX(currentPlayer.GetComponent<PlayerState>());
                    currentPlayer.GetComponent<PlayerState>().EXcooldown = ex.coolTime;
                    currentState = ControlState.Default;
                    if (currentPlayer.GetComponent<PlayerState>().EXcooldown > 0)
                    {
                        IngameManager.Instance.ingameUI.IsSelected(PanelType.EX, false);
                    }
                    IngameManager.Instance.ingameUI.DeselectPanel(PanelType.EX);
                }
                IngameManager.Instance.ingameUI.range.Delete(new Vector2Int(-1, -1));
            }
            else // 자버프형 스킬
            {
                ex.UseEX(currentPlayer.GetComponent<PlayerState>()); //무조건 시전 
                currentPlayer.GetComponent<PlayerState>().EXcooldown = ex.coolTime;
                currentState = ControlState.Default;
                if (currentPlayer.GetComponent<PlayerState>().EXcooldown > 0)
                {
                    IngameManager.Instance.ingameUI.IsSelected(PanelType.EX, false);
                }
                IngameManager.Instance.ingameUI.DeselectPanel(PanelType.EX);
            }

        }

    }
}