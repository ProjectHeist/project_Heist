using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar
{
    public Spot[,] Spots;
    public Astar(Vector3Int[,] grid, int columns, int rows)
    {
        Spots = new Spot[columns, rows];
    }
    private bool IsValidPath(Vector3Int[,] grid, Spot start, Spot end)
    {
        if (end == null)
        {
            return false;
        }
        if (start == null)
        {
            return false;
        }
        if (end.Height >= 1)
        {
            return false;
        }
        return true;
    }
    public List<Spot> CreatePath(Vector3Int[,] grid, Vector2Int start, Vector2Int end, int length)
    {

    }
}

public class Spot
{
    public int X;
    public int Y;
    public int F;
    public int G;
    public int H;
    public int Height = 0;
    public List<Spot> Neighbors;
    public Spot previous = null;
    public Spot(int x, int y, int height)
    {
        X = x;
        Y = y;
        F = 0;
        G = 0;
        H = 0;
        Neighbors = new List<Spot>();
        Height = height;
    }
}
