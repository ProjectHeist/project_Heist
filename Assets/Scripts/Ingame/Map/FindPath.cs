using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPath
{
    public Vector3Int[,] spots;
    private static FindPath instance;
    public static FindPath Instance
    {
        get
        {
            if (null == instance)
            {
                instance = new FindPath();
            }
            return instance;
        }
    }

}
