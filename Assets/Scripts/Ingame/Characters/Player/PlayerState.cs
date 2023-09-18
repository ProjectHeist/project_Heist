using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerState : CharacterState
{
    public int EXcooldown; // 현재 남아있는 쿨타임
    private int EXcooltime; // 쿨타임
    public PlayerState()
    {
        canAttack = 1;
        canMove = 1;
        EXcooldown = 0;
    }
    public void SetState(PlayerStat playerStat, WeaponStat weaponStat)
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
    [SerializeField]
    private string Name; // 플레이어의 실제 이름 
    private int Class; // 플레이어의 클래스
    public int InteractionTime = 0;
    public bool isInteracting = false;

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
    public void OnPlayerHit(int damage)
    {
        OnCharacterHit(damage);
    }
}
