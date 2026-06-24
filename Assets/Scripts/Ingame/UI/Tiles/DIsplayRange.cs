using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Logics;
using UnityEditor.SettingsManagement;
using UnityEngine.LowLevelPhysics;
using TMPro;
using System.Linq;

public enum DisplayType
{
    Default,
    MoveRange,
    minAttackRange,
    maxAttackRange,
    EXRange,
    InteractRange,
    EnemyVision,
    ForbiddenSpots,
    HighLighted
}
public class DisplayRange
{
    private TileStyle[,] defaultstyles;
    private Vector2Int mousePos = new Vector2Int(); // 마우스 포지션
    private HashSet<Vector2Int> moveRange = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> minAttackRange = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> maxAttackRange = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> enemyVision = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> interactRange = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> EXRange = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> forbiddens = new HashSet<Vector2Int>();
    private Dictionary<DisplayType, HashSet<Vector2Int>> typeDict = new Dictionary<DisplayType, HashSet<Vector2Int>>();

    public void Init()
    {
        int mapX = IngameManager.Instance.mapManager.width;
        int mapY = IngameManager.Instance.mapManager.height;
        defaultstyles = new TileStyle[mapX, mapY];
        typeDict.Add(DisplayType.MoveRange, moveRange);
        typeDict.Add(DisplayType.minAttackRange, minAttackRange);
        typeDict.Add(DisplayType.maxAttackRange, maxAttackRange);
        typeDict.Add(DisplayType.EXRange, EXRange);
        typeDict.Add(DisplayType.EnemyVision, enemyVision);
        typeDict.Add(DisplayType.InteractRange, interactRange);
        typeDict.Add(DisplayType.ForbiddenSpots, forbiddens);
        for (int i = 0; i < mapX; i++)
        {
            for (int j = 0; j < mapY; j++)
            {
                defaultstyles[i, j] = GameManager.Instance._data.totalDB.tileStyle.Get(DisplayType.Default);
            }
        }
    }

    private void SetTile(Vector2Int tilePos)
    {
        if (!IngameManager.Instance.mapManager.IsInMap(tilePos))
            return;
        TileStyleDatabase styles = GameManager.Instance._data.totalDB.tileStyle;
        GameObject tile = IngameManager.Instance.mapManager.GetGridCellFromPosition(tilePos);
        if (mousePos == tilePos)
        {
            tile.GetComponentInChildren<SetTileColor>().SetTileStyle(styles.Get(DisplayType.HighLighted));
        }
        else if (forbiddens.Contains(tilePos))
        {
            tile.GetComponentInChildren<SetTileColor>().SetTileStyle(styles.Get(DisplayType.ForbiddenSpots));
        }
        else if (minAttackRange.Contains(tilePos))
        {
            tile.GetComponentInChildren<SetTileColor>().SetTileStyle(styles.Get(DisplayType.minAttackRange));
        }
        else if (maxAttackRange.Contains(tilePos))
        {
            tile.GetComponentInChildren<SetTileColor>().SetTileStyle(styles.Get(DisplayType.maxAttackRange));
        }
        else if (enemyVision.Contains(tilePos))
        {
            tile.GetComponentInChildren<SetTileColor>().SetTileStyle(styles.Get(DisplayType.EnemyVision));
        }
        else if (moveRange.Contains(tilePos))
        {
            tile.GetComponentInChildren<SetTileColor>().SetTileStyle(styles.Get(DisplayType.MoveRange));
        }
        else if (EXRange.Contains(tilePos))
        {
            tile.GetComponentInChildren<SetTileColor>().SetTileStyle(styles.Get(DisplayType.EXRange));
        }
        else if (interactRange.Contains(tilePos))
        {
            tile.GetComponentInChildren<SetTileColor>().SetTileStyle(styles.Get(DisplayType.InteractRange));
        }
        else
        {
            tile.GetComponentInChildren<SetTileColor>().SetTileStyle(styles.Get(DisplayType.Default));
        }
    }

    public void ShowRange(List<Vector2Int> range, DisplayType displayType)
    {
        switch (displayType)
        {
            case DisplayType.MoveRange:
                foreach (Vector2Int tilepos in range)
                {
                    moveRange.Add(tilepos);
                }
                break;
            case DisplayType.minAttackRange:
                foreach (Vector2Int tilepos in range)
                {
                    minAttackRange.Add(tilepos);
                }
                break;
            case DisplayType.maxAttackRange:
                foreach (Vector2Int tilepos in range)
                {
                    maxAttackRange.Add(tilepos);
                }
                break;
            case DisplayType.EnemyVision:
                foreach (Vector2Int tilepos in range)
                {
                    enemyVision.Add(tilepos);
                }
                break;
            case DisplayType.ForbiddenSpots:
                foreach (Vector2Int tilepos in range)
                {
                    forbiddens.Add(tilepos);
                }
                break;
            case DisplayType.EXRange:
                foreach (Vector2Int tilepos in range)
                {
                    EXRange.Add(tilepos);
                }
                break;
            case DisplayType.InteractRange:
                foreach (Vector2Int tilepos in range)
                {
                    interactRange.Add(tilepos);
                }
                break;
        }
        foreach (Vector2Int tilepos in range)
        {
            SetTile(tilepos);
        }
    }

    public void SetMouseTile(Vector2Int currpos, Vector2Int prevpos)
    {
        mousePos = currpos;
        SetTile(prevpos);
        SetTile(currpos);
    }

    public void HideRange(DisplayType type)
    {
        HashSet<Vector2Int> deleted = new HashSet<Vector2Int>(typeDict[type]);
        deleted.Add(mousePos);
        mousePos = new Vector2Int(-1, -1);
        typeDict[type].Clear();
        foreach (Vector2Int tilepos in deleted)
        {
            SetTile(tilepos);
        }
    }
    public void HideAllRange()
    {
        HashSet<Vector2Int> deleted = new HashSet<Vector2Int>();
        deleted.Add(mousePos);
        mousePos = new Vector2Int(-1, -1);
        foreach (HashSet<Vector2Int> set in typeDict.Values)
        {
            deleted.UnionWith(set);
            set.Clear();
        }
        foreach (Vector2Int tilepos in deleted)
        {
            SetTile(tilepos);
        }
    }
}
