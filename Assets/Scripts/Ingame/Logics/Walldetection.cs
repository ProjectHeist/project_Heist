using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logics;
using UnityEngine;

public class Walldetection
{
    public bool IsWallBetween(Vector3 curr, Vector3 target) // 대체해야 함
    {
        Ray ray = new Ray(curr, target - curr);
        RaycastHit[] hit = Physics.RaycastAll(ray);
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider.CompareTag("Wall"))
            {
                return true;
            }
        }
        return false;
    }

    public bool HasLineOfSight(Vector2Int from, Vector2Int to)
    {
        MapManager map = IngameManager.Instance.mapManager;
        int xStart = from.x;
        int yStart = from.y;
        int xEnd = to.x;
        int yEnd = to.y;
        int dx = Mathf.Abs(xEnd - xStart);
        int dy = Mathf.Abs(yEnd - yStart);
        int sx = xStart < xEnd ? 1 : -1;
        int sy = yStart < yEnd ? 1 : -1;
        int err = dx - dy;
        while (true)
        {
            bool isTarget = xStart == xEnd && yStart == yEnd;
            int[] isWall = { 1, 2 };
            if (!isTarget && isWall.Contains(map.spots[xStart, yStart].z) && !(xStart == from.x && yStart == from.y))
            {
                Debug.Log(map.spots[xStart, yStart].z);
                return false;
            }
            if (isTarget)
            {
                return true;
            }
            int err2 = 2 * err;
            if (err2 > -dy)
            {
                err -= dy;
                xStart += sx;
            }
            if (err2 < dx)
            {
                err += dx;
                yStart += sy;
            }
        }
    }
}
