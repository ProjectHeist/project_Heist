using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapDatabase
{
    public MapData testData;

    public void Load()
    {
        var loadedJson = Resources.Load<TextAsset>("Data/Map/TestMap1");
        testData = JsonUtility.FromJson<MapData>(loadedJson.text);
    }
}

[System.Serializable]
public class MapData
{
    public int maxPlayerNum;
    public int EnemyNum;
    public newVector[] enemyPos;
    public int width;
    public int height;
    public Inwalkable[] inwalkable;

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
public class newVector
{
    public int x;
    public int y;
}
