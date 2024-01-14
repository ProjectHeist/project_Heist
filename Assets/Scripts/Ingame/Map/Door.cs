using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Logics;

public class Door : MonoBehaviour
{
    public int type; // 0은 슬라이딩 도어, 1은 금고 도어
    public bool isOpened = false;
    public int requiredTime = 0;
    private float speed = 1f;
    public Vector3 pos = new Vector3(0, 0, 0);

    public void OnDoorInteracted()
    {
        if (isOpened) // 열려있냐? 
        {
            if (type == 0)
                StartCoroutine(SlideClose());
            else
                StartCoroutine(VaultClose());
            Vector2Int gridpos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(pos);
            IngameManager.Instance.mapManager.GridIsWalkable(gridpos.x, gridpos.y, false);
            isOpened = false;
        }
        else
        {
            pos = transform.position;
            if (type == 0)
                StartCoroutine(SlideOpen());
            else
                StartCoroutine(VaultOpen());
            Vector2Int gridpos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(pos);
            IngameManager.Instance.mapManager.GridIsWalkable(gridpos.x, gridpos.y, true);
            isOpened = true;
        }
    }

    IEnumerator VaultOpen()
    {
        float seconds = 3.0f;
        float current = 0.0f;
        while (seconds > current)
        {
            transform.RotateAround(pos - new Vector3(0.45f, 0, 0.3f), Vector3.up, 30 * Time.deltaTime);
            current += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator VaultClose()
    {
        float seconds = 3.0f;
        float current = 0.0f;
        while (seconds > current)
        {
            transform.RotateAround(pos - new Vector3(0.45f, 0, 0.3f), Vector3.up, -30 * Time.deltaTime);
            current += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator SlideOpen()
    {
        Vector3 targetPosition = transform.position + transform.forward;
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;

    }

    IEnumerator SlideClose()
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
