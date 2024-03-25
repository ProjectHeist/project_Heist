using System.Collections;
using System.Collections.Generic;
using Logics;
using UnityEngine;

public class Walldetection
{
    public bool IsWallBetween(Vector3 curr, Vector3 target)
    {
        Ray ray = new Ray(curr, target - curr);
        Physics.Raycast(ray, out RaycastHit hit);
        if (hit.transform.CompareTag("Wall"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
