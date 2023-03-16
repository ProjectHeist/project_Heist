using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInputManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GridMapManager gridMapManager;
    [SerializeField] private LayerMask gridLayer;
    public Vector2Int clickedGridPos;
    private GridCell clickedGridCell;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            clickedGridCell = IsMouseOverAGridSpace();
            if (clickedGridCell != null)
            {
                clickedGridPos = new Vector2Int(clickedGridCell.posX, clickedGridCell.posY);
            }
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
