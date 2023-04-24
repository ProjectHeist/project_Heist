using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar
{
    public Spot[,] Spots;
    public List<Spot> openSet = new List<Spot>();
    public List<Spot> closedSet = new List<Spot>();
    public Spot End;
    public Spot Start;

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
        var columns = Spots.GetUpperBound(0) + 1;
        var rows = Spots.GetUpperBound(1) + 1;
        Spots = new Spot[columns, rows];

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Spots[i, j] = new Spot(grid[i, j].x, grid[i, j].y, grid[i, j].z);
            }
        }
        Start = new Spot(start.x, start.y, 0);
        End = new Spot(end.x, end.y, 0);
        openSet.Add(Start);
        while (openSet.Count > 0)
        {
            int winner = 0;
            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].F < openSet[winner].F)
                    winner = i;
                else if (openSet[i].F == openSet[winner].F)
                    if ()
            }
            Spot current = openSet[winner];
            if (current == End)
                Debug.Log("Complete");
            openSet.Remove(current);
            closedSet.Add(current);

            if (current.Neighbors.Count == 0)
            {
                current.AddNeighbours(Spots);
            }
        }
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
    public void AddNeighbours(Spot[,] grid, int x, int y)
    {
        if (x < grid.GetUpperBound(0))
        {
            Neighbors.Add(grid[x + 1, y]);
        }
        if (x > 0)
        {
            Neighbors.Add(grid[x - 1, y]);
        }
        if (y > grid.GetUpperBound(1))
        {
            Neighbors.Add(grid[x, y + 1]);
        }
        if (y > 0)
        {
            Neighbors.Add(grid[x, y - 1]);
        }
    }
}
