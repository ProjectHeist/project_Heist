using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlState
{
    Selected,
    PlayerMove,
    PlayerAttack,
    PlayerInteract
}
public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    private static PlayerController instance = null;
    public ControlState currentState;

    // variables
    public GameObject currentPlayer;
    private PlayerMove playerMove;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    public static PlayerController Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }


    void Start()
    {

    }

    // Update is called once per frame
    public void OnMouseClick(Vector2Int GridPos)
    {
        Astar astar = new Astar(MapManager.Instance.spots, MapManager.Instance.width, MapManager.Instance.height);
        switch (currentState) // 조작 상태에 따라
        {
            case ControlState.Selected:
                break;
            case ControlState.PlayerMove: // 해당 그리드로 플레이어가 갈지
                playerMove = currentPlayer.GetComponent<PlayerMove>();
                playerMove.SetTargetPosition(GridPos);
                currentState = ControlState.Selected;
                break;
            case ControlState.PlayerAttack: // 해당 그리드에 있는 적을 공격할지
                break;
            case ControlState.PlayerInteract: // 해당 그리드에 있는 물체와 상호작용할지
                break;
        }
    }




}
