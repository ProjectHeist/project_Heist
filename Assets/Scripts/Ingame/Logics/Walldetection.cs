using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logics;
using UnityEngine;

public class Walldetection
{
    public bool IsWallBetween(Vector3 curr, Vector3 target)
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
}
