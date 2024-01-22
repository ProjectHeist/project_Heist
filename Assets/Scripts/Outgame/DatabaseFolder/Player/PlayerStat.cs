using UnityEngine;
[System.Serializable]
[CreateAssetMenu(fileName = "PlayerStat", menuName = "DataElements/PlayerStat", order = 0)]
public class PlayerStat : ScriptableObject
{
    public int playerCost;
    public float playerPercent;
    public int playerNumber;

    public int playerClass;

    public string playerName;

    public int playerHP;

    public float playerAccuracy;

    public int playerMoveRange;
    public int PlayerEX;

}

