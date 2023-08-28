using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
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
        Spot End = null;
        Spot Start = null;
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

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Spots[i, j].AddNeighbours(Spots, i, j);
                if (Spots[i, j].X == start.x && Spots[i, j].Y == start.y)
                    Start = Spots[i, j];
                else if (Spots[i, j].X == end.x && Spots[i, j].Y == end.y)
                    End = Spots[i, j];
            }
        }

        if (!IsValidPath(grid, Start, End))
            return null;
        List<Spot> openSet = new List<Spot>();
        List<Spot> closedSet = new List<Spot>();
        openSet.Add(Start);

        while (openSet.Count > 0)
        {
            int winner = 0;
            for (int i = 0; i < openSet.Count; i++)
                if (openSet[i].F < openSet[winner].F)
                    winner = i;
                else if (openSet[i].F == openSet[winner].F)
                    if (openSet[i].H < openSet[winner].H)
                        winner = i;

            var current = openSet[winner];
            if (End != null && openSet[winner] == End)
            {
                List<Spot> Path = new List<Spot>();
                var temp = current;
                Path.Add(temp);
                while (temp.previous != null)
                {
                    Path.Add(temp.previous);
                    temp = temp.previous;
                }
                if (length - (Path.Count - 1) < 0)
                {
                    Path.RemoveRange(0, (Path.Count - 1) - length);
                }
                return Path;
            }
            openSet.Remove(current);
            closedSet.Add(current);

            var neighbors = current.Neighbors;
            for (int i = 0; i < neighbors.Count; i++)
            {
                var n = neighbors[i];
                if (!closedSet.Contains(n) && n.Height < 1)
                {
                    var tempG = current.G + 1;
                    bool newPath = false;
                    if (openSet.Contains(n))
                    {
                        if (tempG < n.G)
                        {
                            n.G = tempG;
                            newPath = true;
                        }
                    }
                    else
                    {
                        n.G = tempG;
                        newPath = true;
                        openSet.Add(n);
                    }
                    if (newPath)
                    {
                        n.H = Heuristic(n, End);
                        n.F = n.G + n.H;
                        n.previous = current;
                    }
                }
            }

        }
        return null;
    }


    private int Heuristic(Spot a, Spot b)
    {
        var dx = Mathf.Abs(a.X - b.X);
        var dy = Mathf.Abs(a.Y - b.Y);
        return 1 * (dx + dy);
    }
}

public class Spot
{
    public int X;
    public int Y;
    public int F;
    public int G;
    public int H;
    public int dist;
    public int Height = 0;
    public Vector2Int position;
    public List<Spot> Neighbors;
    public Spot previous = null;
    public Spot(int x, int y, int height)
    {
        X = x;
        Y = y;
        F = 0;
        G = 0;
        H = 0;
        dist = 0;
        Neighbors = new List<Spot>();
        position = new Vector2Int(x, y);
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
        if (y < grid.GetUpperBound(1))
        {
            Neighbors.Add(grid[x, y + 1]);
        }
        if (y > 0)
        {
            Neighbors.Add(grid[x, y - 1]);
        }
    }
}
