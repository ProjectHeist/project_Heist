using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "ScriptableObject/DataManagerScriptableObject", order = 1)]
public class Database : ScriptableObject
{
    [SerializeField]
    public PatrolRoute[] Routes;
}
