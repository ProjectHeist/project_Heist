using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    // Start is called before the first frame update
    public void Interact(Vector2Int gridPos)
    {
        GameObject selectedGrid = IngameManager.Instance.mapManager.GetGridCellFromPosition(gridPos);
        GridCell gridCell = selectedGrid.GetComponent<GridCell>();
        gridCell.CheckObject();
        switch (gridCell.objectInThisGrid.tag)
        {
            case "Door":
                gridCell.objectInThisGrid.GetComponent<Door>().OnDoorInteracted();
                break;
        }
    }

}
