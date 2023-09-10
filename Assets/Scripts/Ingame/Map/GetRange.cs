using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;

public class GetRange
{
    private Spot[,] currentmap;
    private Vector3Int[,] grid;
    private int columns;
    private int rows;

    private Queue<Spot> OpenSet = new Queue<Spot>();
    private Dictionary<Vector2Int, int> reachable = new Dictionary<Vector2Int, int>();

    public GetRange(Vector3Int[,] grid, int columns, int rows)
    {
        currentmap = new Spot[columns, rows];
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                currentmap[i, j] = new Spot(grid[i, j].x, grid[i, j].y, grid[i, j].z);
            }
        }
    }

    public List<Vector2Int> getWalkableSpots(Vector2Int start, int moveRange)
    {
        OpenSet.Clear();
        reachable.Clear();
        reachable.Add(start, 0);
        OpenSet.Enqueue(new Spot(start.x, start.y, 0));
        while (OpenSet.Count > 0)
        {
            Spot parent = OpenSet.Dequeue();
            int childDist = parent.dist + 1;
            CheckChild(parent.X - 1, parent.Y, childDist, moveRange, reachable);
            CheckChild(parent.X + 1, parent.Y, childDist, moveRange, reachable);
            CheckChild(parent.X, parent.Y - 1, childDist, moveRange, reachable);
            CheckChild(parent.X, parent.Y + 1, childDist, moveRange, reachable);
        }
        List<Vector2Int> walkableSpots = reachable.Keys.ToList();

        return walkableSpots;
    }

    public List<Vector2Int> getrg(Vector2Int start, int Range)
    {
        OpenSet.Clear();
        reachable.Clear();
        reachable.Add(start, 0);
        OpenSet.Enqueue(new Spot(start.x, start.y, 0));
        while (OpenSet.Count > 0)
        {
            Spot parent = OpenSet.Dequeue();
            int childDist = parent.dist + 1;
            newCheckChild(parent.X - 1, parent.Y, childDist, Range, reachable);
            newCheckChild(parent.X + 1, parent.Y, childDist, Range, reachable);
            newCheckChild(parent.X, parent.Y - 1, childDist, Range, reachable);
            newCheckChild(parent.X, parent.Y + 1, childDist, Range, reachable);
        }
        List<Vector2Int> walkableSpots = reachable.Keys.ToList();

        return walkableSpots;
    }

    void CheckChild(int x, int y, int distance, int maxDistance, Dictionary<Vector2Int, int> reachable)
    {
        var position = new Vector2Int(x, y);
        bool isInBounds = x >= 0 && x < currentmap.GetUpperBound(0) && y >= 0 && y < currentmap.GetUpperBound(1);
        if (!isInBounds)
        {
            return;
        }
        else if (currentmap[x, y].Height != 0)
        {
            return;
        }

        if (reachable.ContainsKey(position))
        {
            return;
        }

        reachable.Add(position, distance);

        if (distance < maxDistance)
        {
            Spot newSpot = new Spot(x, y, 0);
            newSpot.dist = distance;
            OpenSet.Enqueue(newSpot);
        }
    }

    void newCheckChild(int x, int y, int distance, int maxDistance, Dictionary<Vector2Int, int> reachable)
    {
        var position = new Vector2Int(x, y);
        bool isInBounds = x >= 0 && x < currentmap.GetUpperBound(0) && y >= 0 && y < currentmap.GetUpperBound(1);
        if (!isInBounds)
        {
            return;
        }

        if (reachable.ContainsKey(position))
        {
            return;
        }

        reachable.Add(position, distance);

        if (distance < maxDistance)
        {
            Spot newSpot = new Spot(x, y, 0);
            newSpot.dist = distance;
            OpenSet.Enqueue(newSpot);
        }
    }



}
