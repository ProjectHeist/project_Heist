using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPath
{
    Astar astar;
    public Vector3Int[,] spots;
    public int width;
    public int height;
    private static FindPath instance;
    public static FindPath Instance
    {
        get
        {
            if (null == instance)
            {
                instance = new FindPath();
            }
            return instance;
        }
    }

    public List<Spot> makePath(Vector2Int currentPos, Vector2Int targetPos, int distance)
    {
        astar = new Astar(spots, width, height);
        List<Spot> newPath = astar.CreatePath(spots, currentPos, targetPos, distance);
        return newPath;
    }
}
