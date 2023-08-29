using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager
{
    public void SelectedState(Vector2Int position)
    {
        MapManager.Instance.tile[position.x, position.y].GetComponentInChildren<Renderer>().material.color = new Color(100, 0, 0);
    }

    public void DisplayRange(List<Vector2Int> range, Color color)
    {
        for (int i = 0; i < range.Count; i++)
        {
            MapManager.Instance.tile[range[i].x, range[i].y].GetComponentInChildren<Renderer>().material.color = color;
        }
    }

    public void DeleteRange()
    {
        for (int i = 0; i < MapManager.Instance.width; i++)
        {
            for (int j = 0; j < MapManager.Instance.height; j++)
            {
                if (MapManager.Instance.tile[i, j] != null)
                    MapManager.Instance.tile[i, j].GetComponentInChildren<Renderer>().material.color = new Color(256, 256, 256);
            }
        }
    }
}
