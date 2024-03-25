using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "DataElements/MapData", order = 2)]
[System.Serializable]
public class MapData : ScriptableObject
{
    public int maxPlayerNum; // 최대 동료 수 
    public int EnemyNum; //배치할 적의 수 
    public Vector2Int[] enemyPos; //적의 위치 
    public int width;
    public int height;
    public Inwalkable[] inwalkable;
    public PatrolRoute[] patrolRoutes; // 적의 순찰 경로 
    public Forbidden[] forbiddenSpots;

    public List<Vector2Int> getInwalkables()
    {
        List<Vector2Int> inwalkables = new List<Vector2Int>();
        for (int i = 0; i < inwalkable.Length; i++)
        {
            Vector2Int startpoint = new Vector2Int(inwalkable[i].x, inwalkable[i].y);
            if (!inwalkables.Contains(startpoint))
            {
                inwalkables.Add(startpoint);
            }
            if (inwalkable[i].direction == "+x")
            {
                for (int j = 1; j < inwalkable[i].length; j++)
                {
                    Vector2Int newPoint = new Vector2Int(startpoint.x + j, startpoint.y);
                    if (!inwalkables.Contains(newPoint))
                    {
                        inwalkables.Add(newPoint);
                    }
                }
            }
            else if (inwalkable[i].direction == "-x")
            {
                for (int j = 1; j < inwalkable[i].length; j++)
                {
                    Vector2Int newPoint = new Vector2Int(startpoint.x - j, startpoint.y);
                    if (!inwalkables.Contains(newPoint))
                    {
                        inwalkables.Add(newPoint);
                    }
                }
            }
            else if (inwalkable[i].direction == "+y")
            {
                for (int j = 1; j < inwalkable[i].length; j++)
                {
                    Vector2Int newPoint = new Vector2Int(startpoint.x, startpoint.y + j);
                    if (!inwalkables.Contains(newPoint))
                    {
                        inwalkables.Add(newPoint);
                    }
                }
            }
            else if (inwalkable[i].direction == "-y")
            {
                for (int j = 1; j < inwalkable[i].length; j++)
                {
                    Vector2Int newPoint = new Vector2Int(startpoint.x, startpoint.y - j);
                    if (!inwalkables.Contains(newPoint))
                    {
                        inwalkables.Add(newPoint);
                    }
                }
            }
        }
        return inwalkables;
    }

    public List<Vector2Int> getForbiddens()
    {
        List<Vector2Int> forbidden = new List<Vector2Int>();
        for (int i = 0; i < forbiddenSpots.Length; i++)
        {
            Vector2Int startpoint = new Vector2Int(forbiddenSpots[i].x, forbiddenSpots[i].y);
            if (!forbidden.Contains(startpoint))
            {
                forbidden.Add(startpoint);
            }
            if (forbiddenSpots[i].direction == "+x")
            {
                for (int j = 1; j < forbiddenSpots[i].length; j++)
                {
                    Vector2Int newPoint = new Vector2Int(startpoint.x + j, startpoint.y);
                    if (!forbidden.Contains(newPoint))
                    {
                        forbidden.Add(newPoint);
                    }
                }
            }
            else if (forbiddenSpots[i].direction == "-x")
            {
                for (int j = 1; j < forbiddenSpots[i].length; j++)
                {
                    Vector2Int newPoint = new Vector2Int(startpoint.x - j, startpoint.y);
                    if (!forbidden.Contains(newPoint))
                    {
                        forbidden.Add(newPoint);
                    }
                }
            }
            else if (forbiddenSpots[i].direction == "+y")
            {
                for (int j = 1; j < forbiddenSpots[i].length; j++)
                {
                    Vector2Int newPoint = new Vector2Int(startpoint.x, startpoint.y + j);
                    if (!forbidden.Contains(newPoint))
                    {
                        forbidden.Add(newPoint);
                    }
                }
            }
            else if (forbiddenSpots[i].direction == "-y")
            {
                for (int j = 1; j < forbiddenSpots[i].length; j++)
                {
                    Vector2Int newPoint = new Vector2Int(startpoint.x, startpoint.y - j);
                    if (!forbidden.Contains(newPoint))
                    {
                        forbidden.Add(newPoint);
                    }
                }
            }
        }
        return forbidden;
    }
}

[System.Serializable]
public class Inwalkable
{
    public int x;
    public int y;
    public int length;
    public string direction;
}
[System.Serializable]
public class Forbidden
{
    public int x;
    public int y;
    public int length;
    public string direction;
    public int suspicion;
}