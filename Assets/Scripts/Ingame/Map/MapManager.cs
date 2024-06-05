using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    public int width;
    public int height;
    public float gridSpaceSize;
    public int EnemyNum; //배치할 적의 수
    public int spawnTime;
    public Vector2Int[] spawnPos;
    public Vector2Int[] enemyPos;
    public Vector3Int[,] spots;
    public PatrolRoute[] patrolRoutes; // 적의 순찰 경로 
    // 실제 타일들의 리스트
    public GameObject[,] tile;
    public int[,] map;
    public List<Vector2Int> forbiddens;

    public MapManager(MapData mapData)
    {
        width = mapData.width;
        height = mapData.height;
        EnemyNum = mapData.EnemyNum;
        spawnPos = mapData.spawnPos;
        enemyPos = mapData.enemyPos;
        patrolRoutes = mapData.patrolRoutes;
        spawnTime = mapData.spawntime;
    }

    public int GetSuspicion(Vector2Int pos)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (i == pos.x && j == pos.y)
                {
                    return map[i, j];
                }
            }
        }
        return 0;
    }
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

    public void GridIsWalkable(int x, int y, int zVal)
    {
        spots[x, y].z = zVal;
    }

    public int GetDist(Vector3 origin, Vector3 dest)
    {
        return Math.Abs(GetGridPositionFromWorld(origin).x - GetGridPositionFromWorld(dest).x) + Math.Abs(GetGridPositionFromWorld(origin).y - GetGridPositionFromWorld(dest).y);
    }
}
