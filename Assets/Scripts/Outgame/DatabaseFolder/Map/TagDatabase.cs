using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TagDatabase", menuName = "Database/TagDatabase", order = 4)]
public class TagDatabase : ScriptableObject
{
    public string[] tags;
}
