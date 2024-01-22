using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDatabase", menuName = "Database/MapDatabase", order = 3)]
public class MapDatabase : ScriptableObject
{
    public MapData[] MapDataList;
}


