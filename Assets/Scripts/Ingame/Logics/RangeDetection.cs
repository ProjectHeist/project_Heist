using UnityEngine;

public class RangeDetection
{
    public bool IsInRange(Vector2Int currentPos, Vector2Int targetPos, int range)
    {
        Vector2Int diff = targetPos - currentPos;
        return diff.x * diff.x + diff.y * diff.y <= range * range;
    }
}
