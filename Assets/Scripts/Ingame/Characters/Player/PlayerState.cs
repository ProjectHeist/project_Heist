using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerState : CharacterState
{
    public PlayerState()
    {
        canAttack = 1;
        canMove = 1;
    }
    public PlayerState(PlayerStat playerStat, WeaponStat weaponStat)
    {
        Number = playerStat.playerNumber;
        Name = playerStat.playerName;
        HP = playerStat.playerHP;
        accuracy = playerStat.playerAccuracy;
        moveRange = playerStat.playerMoveRange;
        damage = weaponStat.weaponDamage;
        maxAttackRange = weaponStat.weaponMaxAttackRange;
        minAttackRange = weaponStat.weaponMinAttackRange;
        critRate = weaponStat.weaponCritRate;
    }

    private int Number; // 도감에 존재하는 플레이어 넘버
    private string Name; // 플레이어의 실제 이름 
    private int Class; // 플레이어의 클래스

    public int playerNumber
    {
        get
        {
            return Number;
        }
    }
    public string playerName
    {
        get
        {
            return Name;
        }
    }
    public int playerClass
    {
        get
        {
            return Class;
        }
    }
    void Awake() // 이후에 제대로 GameManager의 DataManager의 함수를 통해 불러올 예정
    {
        GetPlayerInfo();
    }
    public void GetPlayerInfo()
    {
        Number = 0;
        HP = 100;
        accuracy = 0.5f;
        moveRange = 10;
        damage = 20;
        maxAttackRange = 12;
        minAttackRange = 6;
        critRate = 0.2f;
        canAttack = 1;
        canMove = 1;
    }
    public void OnPlayerHit(int damage)
    {
        OnCharacterHit(damage);
    }
}
