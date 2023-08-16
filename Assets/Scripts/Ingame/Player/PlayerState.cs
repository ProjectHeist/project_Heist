using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    // PlayerStat ScriptableObject와 WeaponStat ScriptableObject로부터 데이터를 받아와 플레이어의 현재 상태를 나타내는 클래스. 현재는 일단 임시로 데이터를 부여
    public int HP = 100;
    public int moveRange = 50;
    public int critRate = 20;
    public float accuracy = 80;
    public int hide = 3;
    public int damage = 40;
    public int attackRange = 8;
}
