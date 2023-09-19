using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour //그리드에 들어가는 입력을 담당, 이를테면 공격 키를 누르고 그리드를 누를 시 해당 위치에 있는 적의 정보를 보여주고 공격
{
    [SerializeField] private LayerMask gridLayer;
    private Vector2Int clickedGridPos;
    private GridCell clickedGridCell;
    public bool AllowInput = true;

    // 플레이어 선택과 관련된 변수
    public bool selected = false; // 플레이어가 선택되었는가? 

    // Update is called once per frame
    void Update()
    {
        // 마우스 컨트롤
        if (Input.GetMouseButtonDown(0))
        {
            GameManager.Instance._ui.DeleteRange();
            AllowInput = false;
            clickedGridCell = IsMouseOverAGridSpace();
            if (clickedGridCell != null)
            {
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
                    IngameManager.Instance.playerController.OnMouseClick(clickedGridPos); //해당 플레이어를 컨트롤하는 클래스를 이용해 컨트롤한다 
                }
            }
            AllowInput = true;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && AllowInput)
        {
            if (IngameManager.Instance.playerController.currentState == ControlState.Default && selected) // 플레이어가 선택되었으며 아무 행동도 취하지 않았을 때
            {
                GameObject player = IngameManager.Instance.playerController.currentPlayer; // 플레이어 가져오기
                PlayerState state = player.GetComponent<PlayerState>(); // 플레이어 스탯 가져오기
                if (state.canMove > 0)
                {
                    GetRange getRange = new GetRange(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height); // 범위 구하기 
                    List<Vector2Int> moveRange = getRange.getWalkableSpots(IngameManager.Instance.mapManager.GetGridPositionFromWorld(player.transform.position), state.moveRange - 1); // 이동 범위 담는 리스트 
                    GameManager.Instance._ui.DisplayRange(moveRange, Color.blue); // 플레이어의 이동 가능한 위치를 표현
                    IngameManager.Instance.playerController.currentState = ControlState.PlayerMove; // 플레이어 상태를 이동 상태로 전환 (클릭 시 이동)
                    Debug.Log("State Changed to Move");
                    IngameManager.Instance.controlPanel.DisplayMove(true, true);
                }
            }
            else if (IngameManager.Instance.playerController.currentState == ControlState.PlayerMove && selected) // 이미 눌러놨을 경우
            {
                Escape();
                GameObject player = IngameManager.Instance.playerController.currentPlayer; // 플레이어 가져오기
                PlayerState state = player.GetComponent<PlayerState>(); // 플레이어 스탯 가져오기
                IngameManager.Instance.controlPanel.DisplayMove(true, false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.E) && AllowInput)
        {
            if (IngameManager.Instance.playerController.currentState == ControlState.Default && selected)
            {
                GameObject player = IngameManager.Instance.playerController.currentPlayer;
                CharacterState state = player.GetComponent<CharacterState>();
                Color maxRange = new Color(0, 256, 0);
                Color minRange = new Color(100, 256, 100);

                if (state.canAttack > 0)
                {
                    GetRange getRange = new GetRange(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height); // 범위 구하기 
                    List<Vector2Int> maxAttackRange = getRange.getrg(IngameManager.Instance.mapManager.GetGridPositionFromWorld(player.transform.position), state.maxAttackRange - 1); // 최대 공격 범위 담는 리스트 
                    List<Vector2Int> minAttackRange = getRange.getrg(IngameManager.Instance.mapManager.GetGridPositionFromWorld(player.transform.position), state.minAttackRange - 1); // 최소 공격 범위 담는 리스트 
                    GameManager.Instance._ui.DisplayRange(maxAttackRange, maxRange); // 플레이어의 최대 사거리를 표현
                    GameManager.Instance._ui.DisplayRange(minAttackRange, minRange); // 플레이어의 필중 사거리를 표현

                    IngameManager.Instance.playerController.currentState = ControlState.PlayerAttack; // 플레이어 상태를 공격 상태로 전환 (클릭 시 공격)
                    Debug.Log("State Changed to Attack");
                    IngameManager.Instance.controlPanel.DisplayAttack(true, true);
                }
            }
            else if (IngameManager.Instance.playerController.currentState == ControlState.PlayerAttack && selected)
            {
                GameObject player = IngameManager.Instance.playerController.currentPlayer; // 플레이어 가져오기
                PlayerState state = player.GetComponent<PlayerState>(); // 플레이어 스탯 가져오기
                IngameManager.Instance.controlPanel.DisplayAttack(true, false);
                Escape();
            }
        }
        /*else if (Input.GetKeyDown(KeyCode.R) && AllowInput)
        {
            if (IngameManager.Instance.playerController.currentState == ControlState.Default && selected)
            {
                GameObject player = IngameManager.Instance.playerController.currentPlayer;
                PlayerState state = player.GetComponent<PlayerState>();
                //PlayerEX ex = player.GetComponent<PlayerEX>();
                Color Range = new Color(0, 100, 200);
                if (state.EXcooldown == 0)
                {
                    IngameManager.Instance.controlPanel.DisplayEX(true, true);
                    /*if (ex.range != -1)
                    {
                        GetRange getRange = new GetRange(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height); // 범위 구하기 
                        List<Vector2Int> skillRange = getRange.getrg(IngameManager.Instance.mapManager.GetGridPositionFromWorld(player.transform.position), ex.range - 1); // 최대 공격 범위 담는 리스트 
                        GameManager.Instance._ui.DisplayRange(skillRange, Range);
                        Debug.Log("State Changed to EX");
                    }
                }
            }
            else if (IngameManager.Instance.playerController.currentState == ControlState.PlayerEX && selected)
            {
                IngameManager.Instance.controlPanel.DisplayEX(true, false);
            }
        }*/
        else if (Input.GetKeyDown(KeyCode.F) && AllowInput)
        {
            if (IngameManager.Instance.playerController.currentState == ControlState.Default && selected)
            {
                GameObject player = IngameManager.Instance.playerController.currentPlayer;
                Color InteractRange = Color.yellow;

                GetRange getRange = new GetRange(IngameManager.Instance.mapManager.spots, IngameManager.Instance.mapManager.width, IngameManager.Instance.mapManager.height); // 범위 구하기 
                List<Vector2Int> maxInteractRange = getRange.getWalkableSpots(IngameManager.Instance.mapManager.GetGridPositionFromWorld(player.transform.position), 2); // 최대 상호작용 범위 담는 리스트 \
                GameManager.Instance._ui.DisplayRange(maxInteractRange, InteractRange); // 플레이어의 최대 사거리를 표현

                IngameManager.Instance.playerController.currentState = ControlState.PlayerInteract; // 플레이어 상태를 상호작용 상태로 전환 (클릭 시 상호작용)
                Debug.Log("State Changed to Interact");
                IngameManager.Instance.controlPanel.DisplayInteract(true);
            }
            else if (IngameManager.Instance.playerController.currentState == ControlState.PlayerInteract && selected)
            {
                IngameManager.Instance.controlPanel.DisplayInteract(false);
                Escape();
            }
        }

    }

    private void Escape()
    {
        IngameManager.Instance.playerController.currentState = ControlState.Default;
        GameManager.Instance._ui.DeleteRange();
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
        List<Vector2Int> range = getRange.getrg(map.GetGridPositionFromWorld(enemy.transform.position), es.detectRange);
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
            else if (es.faceDir == 1) // -x
            {
                if (range[i].x < map.GetGridPositionFromWorld(enemy.transform.position).x)
                {
                    newRange.Add(range[i]);
                }
            }
            else if (es.faceDir == 2) //+y
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
        }
        GameManager.Instance._ui.DisplayRange(newRange, detectRange);
    }

    private void ShowObjectInfo(GameObject gameObject)  // 해당 그리드에 있는 물체의 정보를 보여주는 함수
    {
        //Debug.Log(clickedGridPos);

    }

    private void Select() // 플레이어를 선택하는 함수
    {
        clickedGridCell.CheckPlayer();
        clickedGridCell.CheckEnemy();
        if (clickedGridCell.playerInThisGrid != null) //무언가가 있는 타일을 클릭했을 때
        {
            IngameManager.Instance.playerController.currentPlayer = clickedGridCell.playerInThisGrid; // 타일 위에 있는 물체를 현재 플레이어로 삼고 
            IngameManager.Instance.playerController.currentState = ControlState.Default; // 플레이어의 상태를 선택됨으로 변경
            GameManager.Instance._ui.DeleteRange(); // 기존 선택된 상태를 해제하고
            ShowSelectedPlayer();
            selected = true;
            Debug.Log("Player Selected");
            if (clickedGridCell.playerInThisGrid.GetComponent<PlayerState>().canMove > 0)
            {
                IngameManager.Instance.controlPanel.Move.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
            }
            else
            {
                IngameManager.Instance.controlPanel.Move.GetComponent<Image>().color = new Color(255, 0, 0, 0.5f);
            }
            if (clickedGridCell.playerInThisGrid.GetComponent<PlayerState>().canAttack > 0)
            {
                IngameManager.Instance.controlPanel.Attack.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
            }
            else
            {
                IngameManager.Instance.controlPanel.Attack.GetComponent<Image>().color = new Color(255, 0, 0, 0.5f);
            }
        }
        else if (clickedGridCell.enemyInThisGrid != null)
        {
            ShowEnemyInfo(clickedGridCell.enemyInThisGrid);
        }
        else if (clickedGridCell.objectInThisGrid != null)
        {
            ShowObjectInfo(clickedGridCell.objectInThisGrid);
        }
        else // 아무것도 없을 때
        {
            selected = false;
            GameManager.Instance._ui.DeleteRange();
        }
    }

    private void ShowSelectedPlayer()
    {
        Vector2Int currentpos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(IngameManager.Instance.playerController.currentPlayer.transform.position);
        GameManager.Instance._ui.SelectedState(currentpos);
    }

    private GridCell IsMouseOverAGridSpace()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, gridLayer))
        {
            return hit.transform.GetComponent<GridCell>();
        }
        else
        {
            return null;
        }
    }
}
