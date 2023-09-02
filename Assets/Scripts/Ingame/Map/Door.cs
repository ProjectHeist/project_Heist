using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isOpened = false;
    private int DoorMove = 0;  // if doorMove=0 stop if doorMove=1 open if doorMove=2 close

    private float WaitForSeconds = 1.0f;
    private float speed = 1f;

    public void OnDoorInteracted()
    {
        Vector2Int gridpos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(transform.position);
        if (isOpened) // 열려있냐? 
        {
            StartCoroutine(Close());
            IngameManager.Instance.mapManager.GridIsWalkable(gridpos.x, gridpos.y, false);
            isOpened = false;
        }
        else
        {
            StartCoroutine(Open());
            IngameManager.Instance.mapManager.GridIsWalkable(gridpos.x, gridpos.y, true);
            isOpened = true;
        }
    }

    IEnumerator Open()
    {
        Vector3 targetPosition = transform.position + Vector3.forward;
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;

    }

    IEnumerator Close()
    {
        Vector3 targetPosition = transform.position + Vector3.back;
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;

    }
}
