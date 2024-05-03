using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Ingame;
using Logics;

public class EnemyMovement : MonoBehaviour
{
    public Vector2Int GetModifiedDist(string rot, int dist)
    {
        switch (rot)
        {
            case "+x": //+x
                return new Vector2Int(dist, 0);
            case "-x": //-x
                return new Vector2Int(-dist, 0);
            case "+y": //+y
                return new Vector2Int(0, dist);
            case "-y": //-y
                return new Vector2Int(0, -dist);
        }
        return new Vector2Int(0, 0);
    }

    public bool InRange(Vector2Int startPos, Vector2Int targetPos, int range)
    {
        EnemyState es = gameObject.GetComponent<EnemyState>();
        int xdist = targetPos.x - startPos.x;
        int ydist = targetPos.y - startPos.y;
        int dist = Math.Abs(xdist) + Math.Abs(ydist);
        if (dist < range)
        {
            return true;
        }
        return false;
    }

    public void changeFaceDir(Vector2Int targetPos)
    {
        EnemyState es = gameObject.GetComponent<EnemyState>();
        Vector2Int startPos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(gameObject.transform.position);
        int xdist = targetPos.x - startPos.x;
        int ydist = targetPos.y - startPos.y;
        if (Math.Abs(xdist) > Math.Abs(ydist))
        {
            if (xdist > 0)
            {
                int prev = es.faceDir;
                es.faceDir = 0;
                Rotate(prev, es.faceDir);
            }
            else
            {
                int prev = es.faceDir;
                es.faceDir = 2;
                Rotate(prev, es.faceDir);
            }
        }
        else if (Math.Abs(ydist) > Math.Abs(xdist))
        {
            if (ydist > 0)
            {
                int prev = es.faceDir;
                es.faceDir = 1;
                Rotate(prev, es.faceDir);
            }
            else
            {
                int prev = es.faceDir;
                es.faceDir = 3;
                Rotate(prev, es.faceDir);
            }
        }
    }

    private void Rotate(int prev, int curr)
    {
        Vector3 angle = new Vector3(0, 90, 0) * (prev - curr);
        transform.eulerAngles = angle;
    }
}
