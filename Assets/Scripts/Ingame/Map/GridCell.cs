using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    // Start is called before the first frame update
    public int posX;
    public int posY;
    // 현재 그리드 내에 물체가 존재하는지, 물체의 종류는 무엇인지
    public GameObject playerInThisGrid = null;
    public GameObject enemyInThisGrid = null;
    public GameObject objectInThisGrid = null;
    public bool isOccupied = false;
    // 그리드를 마우스로 선택했는지
    public bool isSelected = false;
    public void SetPosition(int x, int y)
    {
        posX = x;
        posY = y;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CheckPlayer()
    {
        Vector2Int position = new Vector2Int(posX, posY);
        Vector3 gridpos = MapManager.Instance.GetWorldPositionFromGridPosition(position);
        Collider[] colliders = Physics.OverlapSphere(gridpos, 0.4f);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.tag == "Player")
            {
                playerInThisGrid = colliders[i].gameObject;
            }
        }
    }
    public void CheckEnemy()
    {
        Vector2Int position = new Vector2Int(posX, posY);
        Vector3 gridpos = MapManager.Instance.GetWorldPositionFromGridPosition(position);
        Collider[] colliders = Physics.OverlapSphere(gridpos, 0.4f);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.tag == "Enemy")
            {
                enemyInThisGrid = colliders[i].gameObject;
            }
        }
    }
}
