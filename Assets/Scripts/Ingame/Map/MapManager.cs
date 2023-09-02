using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    public int width;
    public int height;
    public float gridSpaceSize;
    public Vector3Int[,] spots;
    // 실제 타일들의 리스트
    public GameObject[,] tile;
    public int[,] map;


    // 실제 좌표를 격자 내 좌표로 표현
    public Vector2Int GetGridPositionFromWorld(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / gridSpaceSize);
        int y = Mathf.FloorToInt(worldPosition.z / gridSpaceSize);
        x = Mathf.Clamp(x, 0, width);
        y = Mathf.Clamp(y, 0, height);
        return new Vector2Int(x, y);
    }

    // 격자 내 좌표를 실제 좌표로 표현
    public Vector3 GetWorldPositionFromGridPosition(Vector2Int gridPosition)
    {
        float x = gridPosition.x * gridSpaceSize + 0.5f;
        float y = gridPosition.y * gridSpaceSize + 0.5f;
        return new Vector3(x, 0.5f, y);
    }

    public GameObject GetGridCellFromPosition(Vector2Int gridPosition)
    {
        return tile[gridPosition.x, gridPosition.y];
    }

    public void GridIsWalkable(int x, int y, bool isWalkable)
    {
        if (isWalkable)
        {
            spots[x, y].z = 0;
        }
        else
        {
            spots[x, y].z = 1;
        }
    }
}
