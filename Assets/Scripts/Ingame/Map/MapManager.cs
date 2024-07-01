using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    public List<List<Spot>> getPatrolRoutes(Vector2Int startPos, int patrolIdx)
    {
        PatrolRoute patrolRoute = patrolRoutes[patrolIdx];
        Vector2Int currPos = startPos;
        List<List<Spot>> patrolroutes = new List<List<Spot>>();
        for (int i = 0; i < patrolRoute.routes.Count; i++) // 전체 루트
        {
            List<Spot> routes = new List<Spot>();
            for (int j = 0; j < patrolRoute.routes[i].movedist.Count; j++) // 턴마다 루트
            {
                List<Spot> p = addSpotbyRoute(patrolRoute.routes[i].movedist[j], patrolRoute.routes[i].direction[j], currPos);
                Spot Last = p[p.Count - 1];
                currPos = new Vector2Int(Last.X, Last.Y);
                for (int k = 0; k < p.Count; k++)
                {
                    routes.Add(p[k]);
                }
            }
            patrolroutes.Add(routes);
        }
        return patrolroutes;
    }

    private List<Spot> addSpotbyRoute(int dist, string dir, Vector2Int pos)
    {
        List<Spot> path = new List<Spot>();
        switch (dir)
        {
            case "+x":
                for (int i = 0; i < dist; i++)
                {
                    Vector3Int curr = spots[pos.x + 1 + i, pos.y];
                    Spot sp = new Spot(curr.x, curr.y, curr.z);
                    path.Add(sp);
                }
                break;
            case "-x":
                for (int i = 0; i < dist; i++)
                {
                    Vector3Int curr = spots[pos.x - 1 - i, pos.y];
                    Spot sp = new Spot(curr.x, curr.y, curr.z);
                    path.Add(sp);
                }
                break;
            case "+y":
                for (int i = 0; i < dist; i++)
                {
                    Vector3Int curr = spots[pos.x, pos.y + 1 + i];
                    Spot sp = new Spot(curr.x, curr.y, curr.z);
                    path.Add(sp);
                }
                break;
            case "-y":
                for (int i = 0; i < dist; i++)
                {
                    Vector3Int curr = spots[pos.x, pos.y - 1 - i];
                    Spot sp = new Spot(curr.x, curr.y, curr.z);
                    path.Add(sp);
                }
                break;
        }
        return path;
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
