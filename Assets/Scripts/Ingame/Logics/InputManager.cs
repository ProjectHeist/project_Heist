using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour //그리드에 들어가는 입력을 담당, 이를테면 공격 키를 누르고 그리드를 누를 시 해당 위치에 있는 적의 정보를 보여주고 공격
{
    [SerializeField] private LayerMask gridLayer;
    private Vector2Int clickedGridPos;
    private GridCell clickedGridCell;

    // 플레이어 선택과 관련된 변수
    public bool selected = false; // 플레이어가 선택되었는가? 

    // Update is called once per frame
    void Update()
    {
        // 마우스 컨트롤
        if (Input.GetMouseButtonDown(0))
        {
            clickedGridCell = IsMouseOverAGridSpace();
            if (clickedGridCell != null)
            {
                clickedGridPos = new Vector2Int(clickedGridCell.posX, clickedGridCell.posY); // 마우스로 클릭한 그리드의 위치
                ShowInfo();
                if (!selected || PlayerController.Instance.currentState == ControlState.Selected) // 플레이어가 선택되지 않은 상태에서, 혹은 선택된 플레이어가 아무것도 하지 않는 상태에서 그리드를 클릭했을 시
                {
                    Select(); //해당 위치에 플레이어가 있는지 찾는다 
                }
                if (selected) //플레이어가 있는 경우
                {
                    PlayerController.Instance.OnMouseClick(clickedGridPos); //해당 플레이어를 컨트롤하는 클래스를 이용해 컨트롤한다 
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (PlayerController.Instance.currentState == ControlState.Selected && selected) // 플레이어가 선택되었으며 아무 행동도 취하지 않았을 때
            {
                GameObject player = PlayerController.Instance.currentPlayer;
                PlayerState state = player.GetComponent<PlayerState>();
                Astar astar = new Astar(MapManager.Instance.spots, MapManager.Instance.width, MapManager.Instance.height);
                GetRange getRange = new GetRange(MapManager.Instance.spots, MapManager.Instance.width, MapManager.Instance.height);
                List<Vector2Int> moveRange = getRange.getWalkableSpots(MapManager.Instance.GetGridPositionFromWorld(player.transform.position), state.moveRange - 1);
                GameManager.Instance._ui.DisplayMoveRange(moveRange); // 플레이어의 이동 가능한 위치를 표현
                PlayerController.Instance.currentState = ControlState.PlayerMove; // 플레이어 상태를 이동 상태로 전환 (클릭 시 이동)
                Debug.Log("State Changed to Move");
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (PlayerController.Instance.currentState == ControlState.Selected && selected)
            {

            }
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (PlayerController.Instance.currentState == ControlState.Selected && selected)
            {

            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PlayerController.Instance.currentState != ControlState.Selected)
            {
                PlayerController.Instance.currentState = ControlState.Selected;
                GameManager.Instance._ui.DeleteRange();
                ShowSelectedPlayer();
                Debug.Log("State Changed to Select");
            }
        }

    }

    private void ShowInfo()  // 해당 그리드에 있는 물체의 정보를 보여주는 함수
    {
        //Debug.Log(clickedGridPos);
    }

    private void Select() // 플레이어를 선택하는 함수
    {
        clickedGridCell.CheckObject();
        if (clickedGridCell.objectInThisGrid != null) //무언가가 있는 타일을 클릭했을 때
        {
            if (clickedGridCell.objectInThisGrid.tag == "Player") // 플레이어가 있는 타일일 때
            {
                PlayerController.Instance.currentPlayer = clickedGridCell.objectInThisGrid; // 타일 위에 있는 물체를 현재 플레이어로 삼고 
                PlayerController.Instance.currentState = ControlState.Selected; // 플레이어의 상태를 선택됨으로 변경
                GameManager.Instance._ui.DeleteRange(); // 기존 선택된 상태를 해제하고
                ShowSelectedPlayer();
                selected = true;
                Debug.Log("Player Selected");
            }
            else //적이나 물체가 있는 타일일 때
            {
                selected = false;
                Vector2Int currentpos = MapManager.Instance.GetGridPositionFromWorld(PlayerController.Instance.currentPlayer.transform.position);
                GameManager.Instance._ui.DeleteRange();
            }
        }
        else // 아무것도 없을 때
        {
            selected = false;
            Vector2Int currentpos = MapManager.Instance.GetGridPositionFromWorld(PlayerController.Instance.currentPlayer.transform.position);
            GameManager.Instance._ui.DeleteRange();
        }
    }

    private void ShowSelectedPlayer()
    {
        Vector2Int currentpos = MapManager.Instance.GetGridPositionFromWorld(PlayerController.Instance.currentPlayer.transform.position);
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
