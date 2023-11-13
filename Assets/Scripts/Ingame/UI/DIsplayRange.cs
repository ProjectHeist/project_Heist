using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DisplayRange
{
    public void SelectedState(Vector2Int position)
    {
        IngameManager.Instance.mapManager.tile[position.x, position.y].GetComponentInChildren<Renderer>().material.color = new Color(100, 0, 0);
    }

    public void Display(List<Vector2Int> range, Color color)
    {
        for (int i = 0; i < range.Count; i++)
        {
            IngameManager.Instance.mapManager.tile[range[i].x, range[i].y].GetComponentInChildren<Renderer>().material.color = color;
        }
    }

    public void Delete(Vector2Int except)
    {
        for (int i = 0; i < IngameManager.Instance.mapManager.width; i++)
        {
            for (int j = 0; j < IngameManager.Instance.mapManager.height; j++)
            {
                if (IngameManager.Instance.mapManager.tile[i, j] != null || (i != except.x && i != except.y))
                    IngameManager.Instance.mapManager.tile[i, j].GetComponentInChildren<Renderer>().material.color = new Color(256, 256, 256);
            }
        }
    }
}
