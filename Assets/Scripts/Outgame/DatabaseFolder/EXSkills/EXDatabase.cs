using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ingame;

[CreateAssetMenu(fileName = "EXDatabase", menuName = "Database/EXDatabase", order = 2)]
public class EXDatabase : ScriptableObject
{
    public PlayerEX[] EXList;
}
