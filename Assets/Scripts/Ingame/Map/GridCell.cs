using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logics;
using Ingame;

public class GridCell : MonoBehaviour
{
    // Start is called before the first frame update
    public int posX;
    public int posY;
    // 현재 그리드 내에 물체가 존재하는지, 물체의 종류는 무엇인지
    public List<bool> isPlayerIn = new List<bool>();
    public List<bool> isEnemyIn = new List<bool>();
    public List<bool> isInSight = new List<bool>();
    public GameObject playerInThisGrid = null;
    public GameObject enemyInThisGrid = null;
    public GameObject objectInThisGrid = null;
    public bool isOccupied = false;
    // 그리드를 마우스로 선택했는지
    public bool isSelected = false;

    public void init()
    {
        for (int i = 0; i < IngameManager.Instance.players.Count; i++)
        {
            isPlayerIn.Add(false);
        }
        for (int i = 0; i < IngameManager.Instance.mapManager.EnemyNum; i++)
        {
            isEnemyIn.Add(false);
        }
        for (int i = 0; i < IngameManager.Instance.mapManager.EnemyNum; i++)
        {
            isInSight.Add(false);
        }
    }

    public void SetPosition(int x, int y)
    {
        posX = x;
        posY = y;
    }

    public void SetPlayer(int i, bool exist)
    {
        if (IngameManager.Instance.isPlayerDead[i]) return;
        isPlayerIn[i] = exist;
        if (exist)
        {
            playerInThisGrid = IngameManager.Instance.players[i];
        }
        else
        {
            playerInThisGrid = null;
        }
    }

    public void SetEnemy(int i, bool exist)
    {
        if (IngameManager.Instance.isEnemyDead[i]) return;
        isEnemyIn[i] = exist;
        if (exist)
        {
            enemyInThisGrid = IngameManager.Instance.enemies[i];
        }
        else
        {
            enemyInThisGrid = null;
        }
    }

    public void SetSight(int i, bool exist)
    {
        isInSight[i] = exist;
    }

    public void CheckIfPlayerIsDetected()
    {
        if (playerInThisGrid != null)
        {
            for (int i = 0; i < isInSight.Count; i++)
            {
                if (IngameManager.Instance.isEnemyDead[i]) continue;
                if (isInSight[i])
                {
                    EnemyVision enemyVision = IngameManager.Instance.enemies[i].GetComponent<EnemyVision>();
                    enemyVision.PlayerEnterSight(playerInThisGrid);
                }
                else if (playerInThisGrid.GetComponent<PlayerState>().enemyDetectedPlayer.Count != 0)
                {
                    List<GameObject> enemies = playerInThisGrid.GetComponent<PlayerState>().enemyDetectedPlayer;
                    if (enemies.Contains(IngameManager.Instance.enemies[i]))
                    {
                        EnemyVision enemyVision = IngameManager.Instance.enemies[i].GetComponent<EnemyVision>();
                        enemyVision.PlayerExitSight(playerInThisGrid);
                    }
                }
            }
        }
    }

    public void CheckPlayer() // 아군이 그리드에 존재한다면 해당 아군을 playerInThisGrid에 저장, 존재하지 않는다면 null로 초기화
    {
        bool playerIn = false;
        for (int i = 0; i < isPlayerIn.Count; i++)
        {
            if (isPlayerIn[i])
            {
                playerInThisGrid = IngameManager.Instance.players[i];
                playerIn = true;
            }
        }
        if (!playerIn)
        {
            playerInThisGrid = null;
        }
    }
    public void CheckEnemy()
    {
        bool enemyIn = false;
        for (int i = 0; i < isEnemyIn.Count; i++)
        {
            if (isEnemyIn[i])
            {
                enemyInThisGrid = IngameManager.Instance.enemies[i];
                enemyIn = true;
            }
        }
        if (!enemyIn)
        {
            enemyInThisGrid = null;
        }
    }
    public void CheckObject()
    {
        Vector2Int position = new Vector2Int(posX, posY);
        Vector3 gridpos = IngameManager.Instance.mapManager.GetWorldPositionFromGridPosition(position);
        Collider[] colliders = Physics.OverlapSphere(gridpos, 0.4f);
        List<string> tags = IngameManager.Instance.tags;
        for (int i = 0; i < colliders.Length; i++)
        {
            if (tags.Contains(colliders[i].gameObject.tag))
            {
                objectInThisGrid = colliders[i].gameObject;
            }
        }
    }
}
