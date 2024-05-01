using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Logics;

public class DisplayRange
{
    public void SelectedState(Vector2Int position)
    {
        IngameManager.Instance.mapManager.tile[position.x, position.y].GetComponentInChildren<SetTileColor>().SetColor(new Color(100, 0, 0));
    }

    public void Display(List<Vector2Int> range, Color color)
    {
        for (int i = 0; i < range.Count; i++)
        {
            IngameManager.Instance.mapManager.tile[range[i].x, range[i].y].GetComponentInChildren<SetTileColor>().SetColor(color);
        }
    }

    public void Delete(Vector2Int except)
    {
        for (int i = 0; i < IngameManager.Instance.mapManager.width; i++)
        {
            for (int j = 0; j < IngameManager.Instance.mapManager.height; j++)
            {
                if (IngameManager.Instance.mapManager.tile[i, j] != null || (i != except.x && i != except.y))
                    IngameManager.Instance.mapManager.tile[i, j].GetComponentInChildren<SetTileColor>().SetColor(new Color(256, 256, 256));
            }
        }
    }
    public void Delete(List<Vector2Int> deletelist, Vector2Int except)
    {
        for (int i = 0; i < deletelist.Count; i++)
        {
            if (IngameManager.Instance.mapManager.tile[deletelist[i].x, deletelist[i].y] != null && !deletelist[i].Equals(except))
                IngameManager.Instance.mapManager.tile[deletelist[i].x, deletelist[i].y].GetComponentInChildren<SetTileColor>().SetColor(new Color(256, 256, 256));
        }
    }
}
