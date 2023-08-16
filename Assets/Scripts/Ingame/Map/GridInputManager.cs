using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ControlState
{
    Select,
    PlayerMove,
    PlayerAttack,
    PlayerInteract
}

public class GridInputManager : MonoBehaviour //그리드에 들어가는 입력을 담당, 이를테면 공격 키를 누르고 그리드를 누를 시 해당 위치에 있는 적의 정보를 보여주고 공격
{
    // Start is called before the first frame update
    [SerializeField] private GridMapManager gridMapManager;
    [SerializeField] private LayerMask gridLayer;
    public Vector2Int clickedGridPos;
    private GridCell clickedGridCell;
    private GridCell selectedGridCell;
    public GameObject currentPlayer;
    public PlayerMove playerMove;
    public bool selected = false;

    ControlState currentState = ControlState.Select;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Astar astar = new Astar(GridMapManager.Instance.spots, GridMapManager.Instance.width, GridMapManager.Instance.height);
            clickedGridCell = IsMouseOverAGridSpace();
            if (clickedGridCell != null)
            {
                clickedGridPos = new Vector2Int(clickedGridCell.posX, clickedGridCell.posY); // 마우스로 클릭한 그리드의 위치
                ShowInfo();
                switch (currentState) // 조작 상태에 따라
                {
                    case ControlState.Select:
                        Select();
                        break;
                    case ControlState.PlayerMove: // 해당 그리드로 플레이어가 갈지
                        playerMove = currentPlayer.GetComponent<PlayerMove>();
                        playerMove.SetTargetPosition(clickedGridPos);
                        currentState = ControlState.Select;
                        break;
                    case ControlState.PlayerAttack: // 해당 그리드에 있는 적을 공격할지
                        break;
                    case ControlState.PlayerInteract: // 해당 그리드에 있는 물체와 상호작용할지
                        break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentState == ControlState.Select && selected)
            {
                currentState = ControlState.PlayerMove;
                Debug.Log("State Changed to Move");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {

        }
        else if (Input.GetKeyDown(KeyCode.E))
        {

        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState != ControlState.Select)
            {
                currentState = ControlState.Select;
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
        selectedGridCell = clickedGridCell;
        clickedGridCell.CheckObject();
        if (clickedGridCell.objectInThisGrid != null) //무언가가 있는 타일을 클릭했을 때
        {
            if (clickedGridCell.objectInThisGrid.tag == "Player") // 플레이어가 있는 타일일 때
            {
                currentPlayer = clickedGridCell.objectInThisGrid;
                selected = true;
                Debug.Log("Player Selected");
            }
            else //적이나 물체가 있는 타일일 때
            {
                selected = false;
            }
        }
        else
        {
            selected = false;
        }
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
