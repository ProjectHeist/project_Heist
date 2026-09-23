using UnityEngine;

[CreateAssetMenu(fileName = "SuspicionSettings", menuName = "Scriptable Objects/SuspicionSettings")]
public class SuspicionSettings : ScriptableObject
{
    public int nearSus;
    public int midSus;
    public int farSus;
    public float nearRatio;
    public float midRatio;
    public int GetSuspicionByDistance(Vector2Int source, Vector2Int dest, float visionRange)
    {
        float distance = Mathf.Sqrt((source.x - dest.x) * (source.x - dest.x) + (source.y - dest.y) * (source.y - dest.y));
        if (distance <= visionRange * nearRatio)
        {
            return nearSus;
        }
        else if (distance <= visionRange * midRatio)
        {
            return midSus;
        }
        else
        {
            return farSus;
        }
    }
    public int GetSuspicionByDistance(float dist, float visionRange)
    {
        if (dist <= visionRange * nearRatio)
        {
            return nearSus;
        }
        else if (dist <= visionRange * midRatio)
        {
            return midSus;
        }
        else
        {
            return farSus;
        }
    }

    public int GetSuspicionDecayByPattern(EnemyPatternType pattern)
    {
        switch (pattern)
        {
            case EnemyPatternType.Patrol:
            case EnemyPatternType.Guard:
                return normalDecay;
            case EnemyPatternType.Alert:
                return alertDecay;
            case EnemyPatternType.Chase:
                return chaseDecay;
            default:
                return 0;
        }
    }

    public int enemyMemoryturn;
    public int chaseThreshold;
    public int alertThreshold;
    public int alertDecay;
    public int chaseDecay;
    public int normalDecay;
}
