using UnityEngine;

[CreateAssetMenu(fileName = "TileStyleDatabase", menuName = "Scriptable Objects/TileStyleDatabase")]
public class TileStyleDatabase : ScriptableObject
{
    public TileStyle[] styles;
    public TileStyle Get(DisplayType type) => styles[(int)type];
}
