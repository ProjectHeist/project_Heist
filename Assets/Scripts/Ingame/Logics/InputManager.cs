using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine;
using Ingame;
using UnityEditor;

namespace Logics
{
    public class InputManager : MonoBehaviour //그리드에 들어가는 입력을 담당, 이를테면 공격 키를 누르고 그리드를 누를 시 해당 위치에 있는 적의 정보를 보여주고 공격
    {
        [SerializeField] private LayerMask gridLayer;

        private Vector2Int clickedGridPos;
        private GridCell clickedGridCell;
        public bool AllowInput = true;

        // 플레이어 선택과 관련된 변수
        public bool selected = false; // 플레이어가 선택되었는가? 

        bool tabclicked = false; // 금지구역 확인용
        public Vector2Int currentTilePos;
        private List<Vector2Int> currentRangeArea = new List<Vector2Int>();

        // Update is called once per frame
        void Update()
        {
            if (!IngameManager.Instance.isEnemyPhase)
            {
                if (selected && IngameManager.Instance.playerController.currentState != ControlState.Default)
                {
                    Vector2Int currentMousePos = MousePosToGridPos();
                    setMouseTile(currentMousePos, currentRangeArea);
                }
                // 마우스 컨트롤
                if (Input.GetMouseButtonDown(0))
                {
                    IngameManager.Instance.ingameUI.range.HideAllRange();
                    AllowInput = false;
                    Vector2Int mousePos = MousePosToGridPos();
                    if (IngameManager.Instance.mapManager.IsInMap(mousePos))
                    {
                        clickedGridCell = IngameManager.Instance.mapManager.GetGridCellFromPosition(mousePos).GetComponent<GridCell>();
                        clickedGridPos = new Vector2Int(clickedGridCell.posX, clickedGridCell.posY); // 마우스로 클릭한 그리드의 위치
                        if (!IngameManager.Instance.Deployed)
                        {
                            IngameManager.Instance.Deploy(clickedGridPos);
                        }
                        else if (!selected || IngameManager.Instance.playerController.currentState == ControlState.Default) // 플레이어가 선택되지 않은 상태에서, 혹은 선택된 플레이어가 아무것도 하지 않는 상태에서 그리드를 클릭했을 시
                        {
                            Select(); //해당 위치에 플레이어가 있는지 찾는다 
                        }
                        else if (selected) //플레이어가 있는 경우
                        {
                            if (IngameManager.Instance.playerController.currentState != ControlState.Default)
                                IngameManager.Instance.playerController.OnMouseClick(currentTilePos);
                            else
                                IngameManager.Instance.playerController.OnMouseClick(clickedGridPos); //해당 플레이어를 컨트롤하는 클래스를 이용해 컨트롤한다 
                        }
                    }
                    AllowInput = true;
                }
                else if (Input.GetKeyDown(KeyCode.Q) && AllowInput)
                {
                    EnterMode(ControlState.PlayerMove);
                }
                else if (Input.GetKeyDown(KeyCode.E) && AllowInput)
                {
                    EnterMode(ControlState.PlayerAttack);
                }
                else if (Input.GetKeyDown(KeyCode.R) && AllowInput)
                {
                    EnterMode(ControlState.PlayerEX);
                }
                else if (Input.GetKeyDown(KeyCode.F) && AllowInput)
                {
                    EnterMode(ControlState.PlayerInteract);
                }
                else if (Input.GetKeyDown(KeyCode.Tab) && AllowInput)
                {
                    if (!tabclicked)
                    {
                        List<Vector2Int> f = IngameManager.Instance.mapManager.forbiddens;
                        IngameManager.Instance.ingameUI.range.ShowRange(f, DisplayType.ForbiddenSpots);
                        tabclicked = true;
                    }
                    else
                    {
                        List<Vector2Int> f = IngameManager.Instance.mapManager.forbiddens;
                        Vector2Int currentPos;
                        if (IngameManager.Instance.playerController.currentPlayer != null)
                            currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(IngameManager.Instance.playerController.currentPlayer.transform.position);
                        else
                            currentPos = new Vector2Int(-1, -1);
                        IngameManager.Instance.ingameUI.range.HideRange(DisplayType.ForbiddenSpots);
                        tabclicked = false;
                    }
                }
            }
        }

        void setMouseTile(Vector2Int mousePos, List<Vector2Int> rangeArea)
        {
            Vector2Int indicatedTilePos = new Vector2Int();
            double minDist = double.MaxValue;
            foreach (Vector2Int tilePos in rangeArea)
            {
                double dist = (tilePos.x - mousePos.x) * (tilePos.x - mousePos.x) + (tilePos.y - mousePos.y) * (tilePos.y - mousePos.y);
                if (dist < minDist)
                {
                    minDist = dist;
                    indicatedTilePos = tilePos;
                }
            }
            if (indicatedTilePos != currentTilePos)
            {
                IngameManager.Instance.ingameUI.range.SetMouseTile(indicatedTilePos, currentTilePos);
                currentTilePos = indicatedTilePos;
            }
        }

        void EnterMode(ControlState newState) // 플레이어의 모드를 전환하는 함수 (PlayerMove, PlayerAttack 등)
        {
            GameObject player = IngameManager.Instance.playerController.currentPlayer; // 플레이어 가져오기
            if (player == null || !selected) return;
            if (IngameManager.Instance.playerController.currentState == newState)
            {
                ExitToDefault();
                return;
            }
            ClearCurrentMode();
            bool entered = false;
            switch (newState)
            {
                case ControlState.PlayerMove:
                    entered = TryEnterMove(player);
                    break;
                case ControlState.PlayerAttack:
                    entered = TryEnterAttack(player);
                    break;
                case ControlState.PlayerEX:
                    entered = TryEnterEX(player);
                    break;
                case ControlState.PlayerInteract:
                    entered = TryEnterInteract(player);
                    break;
            }

            if (entered)
            {
                IngameManager.Instance.playerController.currentState = newState;
                Debug.Log("State Changed to " + newState.ToString());
            }
        }

        void ClearCurrentMode()
        {
            var state = IngameManager.Instance.playerController.currentState;
            switch (state)
            {
                case ControlState.PlayerMove:
                    IngameManager.Instance.ingameUI.range.HideRange(DisplayType.MoveRange);
                    IngameManager.Instance.ingameUI.DeselectPanel(PanelType.Move);
                    break;
                case ControlState.PlayerAttack:
                    IngameManager.Instance.ingameUI.range.HideRange(DisplayType.minAttackRange);
                    IngameManager.Instance.ingameUI.range.HideRange(DisplayType.maxAttackRange);
                    IngameManager.Instance.ingameUI.DeselectPanel(PanelType.Attack);
                    break;
                case ControlState.PlayerEX:
                    IngameManager.Instance.ingameUI.range.HideRange(DisplayType.EXRange);
                    IngameManager.Instance.ingameUI.DeselectPanel(PanelType.EX);
                    break;
                case ControlState.PlayerInteract:
                    IngameManager.Instance.ingameUI.range.HideRange(DisplayType.InteractRange);
                    IngameManager.Instance.ingameUI.DeselectPanel(PanelType.Interact);
                    break;
            }
        }

        void ExitToDefault()
        {
            ClearCurrentMode();
            IngameManager.Instance.playerController.currentState = ControlState.Default;
            Escape();
        }

        bool TryEnterMove(GameObject player)
        {
            PlayerState state = player.GetComponent<PlayerState>();
            if (state.canMove <= 0) return false;
            GetRange getRange = new GetRange(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height); // 범위 구하기 
            List<Vector2Int> moveRange = getRange.getWalkableSpots(IngameManager.Instance.mapManager.GetGridPositionFromWorld(player.transform.position), state.remainMoveRange); // 이동 범위 담는 리스트 
            currentRangeArea.Clear();
            currentRangeArea.AddRange(moveRange);
            IngameManager.Instance.ingameUI.range.ShowRange(moveRange, DisplayType.MoveRange); // 플레이어의 이동 가능한 위치를 표현
            IngameManager.Instance.ingameUI.SelectPanel(PanelType.Move);
            return true;
        }

        bool TryEnterAttack(GameObject player)
        {
            PlayerState state = player.GetComponent<PlayerState>();
            Color maxRange = new Color(0, 256, 0);
            Color minRange = new Color(100, 256, 100);
            if (state.canAttack <= 0) return false;
            GetRange getRange = new GetRange(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height); // 범위 구하기 
            List<Vector2Int> maxAttackRange = getRange.getrg(IngameManager.Instance.mapManager.GetGridPositionFromWorld(player.transform.position), state.maxAttackRange); // 최대 공격 범위 담는 리스트 
            List<Vector2Int> minAttackRange = getRange.getrg(IngameManager.Instance.mapManager.GetGridPositionFromWorld(player.transform.position), state.minAttackRange); // 최소 공격 범위 담는 리스트 
            currentRangeArea.Clear();
            currentRangeArea.AddRange(maxAttackRange);
            IngameManager.Instance.ingameUI.range.ShowRange(maxAttackRange, DisplayType.maxAttackRange); // 플레이어의 최대 사거리를 표현
            IngameManager.Instance.ingameUI.range.ShowRange(minAttackRange, DisplayType.minAttackRange); // 플레이어의 필중 사거리를 표현
            IngameManager.Instance.ingameUI.SelectPanel(PanelType.Attack);
            return true;
        }

        bool TryEnterEX(GameObject player)
        {
            PlayerState state = player.GetComponent<PlayerState>();
            PlayerEX ex = GameManager.Instance.playerEXes[state.EXIndex];
            Color Range = Color.cyan;
            if (state.EXcooldown > 0) return false;
            IngameManager.Instance.ingameUI.SelectPanel(PanelType.EX);
            if (ex.range != -1)
            {
                GetRange getRange = new GetRange(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height); // 범위 구하기 
                //List<Vector2Int> skillRange = getRange.getrg(IngameManager.Instance.mapManager.GetGridPositionFromWorld(player.transform.position), ex.range - 1); // 최대 공격 범위 담는 리스트 
                //IngameManager.Instance.ingameUI.range.Display(skillRange, Range);
            }
            return true;
        }

        bool TryEnterInteract(GameObject player)
        {
            GetRange getRange = new GetRange(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height); // 범위 구하기 
            List<Vector2Int> maxInteractRange = getRange.getrg(IngameManager.Instance.mapManager.GetGridPositionFromWorld(player.transform.position), 2); // 최대 상호작용 범위 담는 리스트 \
            currentRangeArea.Clear();
            currentRangeArea.AddRange(maxInteractRange);
            IngameManager.Instance.ingameUI.range.ShowRange(maxInteractRange, DisplayType.InteractRange); // 플레이어의 최대 사거리를 표현
            IngameManager.Instance.ingameUI.SelectPanel(PanelType.Interact);
            return true;
        }


        private void Escape()
        {
            IngameManager.Instance.playerController.currentState = ControlState.Default;
            Vector2Int currentPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(IngameManager.Instance.playerController.currentPlayer.transform.position);
            IngameManager.Instance.ingameUI.range.HideAllRange();
            ShowSelectedPlayer();
            Debug.Log("State Changed to Select");
        }

        private void ShowEnemyInfo(GameObject enemy)  // 해당 그리드에 있는 물체의 정보를 보여주는 함수
        {
            //Debug.Log(clickedGridPos);
            Color detectRange = Color.magenta;

            MapManager map = IngameManager.Instance.mapManager;
            EnemyState es = enemy.GetComponent<EnemyState>();
            GetRange getRange = new GetRange(map.spots, map.width, map.height);
            /*List<Vector2Int> range = getRange.getrg(map.GetGridPositionFromWorld(enemy.transform.position), es.detectRange);
            List<Vector2Int> newRange = new List<Vector2Int>();
            for (int i = 0; i < range.Count; i++)
            {
                if (es.faceDir == 0) //+x
                {
                    if (range[i].x > map.GetGridPositionFromWorld(enemy.transform.position).x)
                    {
                        newRange.Add(range[i]);
                    }
                }
                else if (es.faceDir == 2) // -x
                {
                    if (range[i].x < map.GetGridPositionFromWorld(enemy.transform.position).x)
                    {
                        newRange.Add(range[i]);
                    }
                }
                else if (es.faceDir == 1) //+y
                {
                    if (range[i].y > map.GetGridPositionFromWorld(enemy.transform.position).y)
                    {
                        newRange.Add(range[i]);
                    }
                }
                else //-y
                {
                    if (range[i].y < map.GetGridPositionFromWorld(enemy.transform.position).y)
                    {
                        newRange.Add(range[i]);
                    }
                }
            }*/
            IngameManager.Instance.ingameUI.range.ShowRange(enemy.GetComponent<EnemyVision>().visionList, DisplayType.EnemyVision);
        }

        private void ShowObjectInfo(GameObject gameObject)  // 해당 그리드에 있는 물체의 정보를 보여주는 함수
        {
            //Debug.Log(clickedGridPos);

        }

        private void Select() // 플레이어를 선택하는 함수
        {
            if (clickedGridCell.playerInThisGrid != null) // 플레이어가 있는 타일을 클릭할 때
            {
                IngameManager.Instance.ingameUI.DisplayName(clickedGridCell.playerInThisGrid);
                IngameManager.Instance.playerController.currentPlayer = clickedGridCell.playerInThisGrid; // 타일 위에 있는 물체를 현재 플레이어로 삼고 
                IngameManager.Instance.playerController.currentState = ControlState.Default; // 플레이어의 상태를 선택됨으로 변경
                ShowSelectedPlayer();
                selected = true;
                Debug.Log("Player Selected");
                if (clickedGridCell.playerInThisGrid.GetComponent<PlayerState>().canMove > 0)
                {
                    IngameManager.Instance.ingameUI.IsSelected(PanelType.Move, true);
                }
                else
                {
                    IngameManager.Instance.ingameUI.IsSelected(PanelType.Move, false);
                }
                if (clickedGridCell.playerInThisGrid.GetComponent<PlayerState>().canAttack > 0)
                {
                    IngameManager.Instance.ingameUI.IsSelected(PanelType.Attack, true);
                }
                else
                {
                    IngameManager.Instance.ingameUI.IsSelected(PanelType.Attack, false);
                }
                if (clickedGridCell.playerInThisGrid.GetComponent<PlayerState>().EXcooldown == 0)
                {
                    IngameManager.Instance.ingameUI.IsSelected(PanelType.EX, true);
                }
                else
                {
                    IngameManager.Instance.ingameUI.IsSelected(PanelType.EX, false);
                }
            }
            else if (clickedGridCell.enemyInThisGrid != null)
            {
                IngameManager.Instance.ingameUI.DisplayName(null);
                IngameManager.Instance.playerController.currentPlayer = null;
                ShowEnemyInfo(clickedGridCell.enemyInThisGrid);
            }
            else if (clickedGridCell.objectInThisGrid != null)
            {
                IngameManager.Instance.ingameUI.DisplayName(null);
                IngameManager.Instance.playerController.currentPlayer = null;
                ShowObjectInfo(clickedGridCell.objectInThisGrid);
            }
            else // 아무것도 없을 때
            {
                IngameManager.Instance.ingameUI.DisplayName(null);
                IngameManager.Instance.playerController.currentPlayer = null;
                selected = false;
            }
        }

        private void ShowSelectedPlayer()
        {
            Vector2Int currentpos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(IngameManager.Instance.playerController.currentPlayer.transform.position);
            // ui를 이용하여 플레이어를 가리키는 기능 추가
        }

        private Vector2Int MousePosToGridPos()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane gridPlane = new Plane(Vector3.up, new Vector3(0f, 0.05f, 0f));
            if (gridPlane.Raycast(ray, out float dist))
            {
                Vector3 worldPoint = ray.GetPoint(dist);
                Vector2Int mousePos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(worldPoint);
                return mousePos;
            }
            else
            {
                return Vector2Int.zero;
            }
            /*if (Physics.Raycast(ray, out RaycastHit hit, 1000f, gridLayer))
            {
                return hit.transform.GetComponent<GridCell>();
            }
            else
            {
                return null;
            }*/
        }
    }
}