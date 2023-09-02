using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerState : CharacterState
{
    void Awake() // 이후에 제대로 GameManager의 DataManager의 함수를 통해 불러올 예정
    {
        GetPlayerInfo();
    }
    public void GetPlayerInfo()
    {
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
