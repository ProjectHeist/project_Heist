using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Logics;

public class Door : MonoBehaviour
{
    public int type; // 0은 슬라이딩 도어, 1은 금고 도어
    public GameObject doorModel;
    public bool isOpened = false;
    public bool DoorIsOpening = true; // 문이 열리는 중인가?
    public int requiredTime = 0;
    private float speed = 1f;
    public Vector3 pos;

    public void OnDoorInteracted()
    {
        if (isOpened) // 열려있냐? 
        {
            if (type == 0)
                StartCoroutine(SlideClose());
            else
                StartCoroutine(VaultClose());
        }
        else
        {
            if (type == 0)
                StartCoroutine(SlideOpen());
            else
                StartCoroutine(VaultOpen());
        }
    }

    public IEnumerator VaultOpen()
    {
        pos = gameObject.transform.position;
        DoorIsOpening = true;
        float seconds = 3.0f;
        float current = 0.0f;
        while (seconds > current)
        {
            doorModel.transform.RotateAround(pos - new Vector3(0.45f, 0, 0.3f), Vector3.up, 30 * Time.deltaTime);
            current += Time.deltaTime;
            yield return null;
        }
        Vector2Int gridpos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(pos);
        IngameManager.Instance.mapManager.GridIsWalkable(gridpos.x, gridpos.y, 0);
        isOpened = true;
        DoorIsOpening = false;
    }

    public IEnumerator VaultClose()
    {
        pos = gameObject.transform.position;
        DoorIsOpening = true;
        float seconds = 3.0f;
        float current = 0.0f;
        while (seconds > current)
        {
            doorModel.transform.RotateAround(pos - new Vector3(0.45f, 0, 0.3f), Vector3.up, -30 * Time.deltaTime);
            current += Time.deltaTime;
            yield return null;
        }
        Vector2Int gridpos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(pos);
        IngameManager.Instance.mapManager.GridIsWalkable(gridpos.x, gridpos.y, 2);
        isOpened = false;
        DoorIsOpening = false;
    }

    public IEnumerator SlideOpen()
    {
        pos = gameObject.transform.position;
        DoorIsOpening = true;
        Vector3 targetPosition = doorModel.transform.position + doorModel.transform.forward;
        while (doorModel.transform.position != targetPosition)
        {
            doorModel.transform.position = Vector3.MoveTowards(doorModel.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        Vector2Int gridpos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(pos);
        IngameManager.Instance.mapManager.GridIsWalkable(gridpos.x, gridpos.y, 0);
        isOpened = true;
        doorModel.transform.position = targetPosition;
        DoorIsOpening = false;
    }

    public IEnumerator SlideClose()
    {
        pos = gameObject.transform.position;
        DoorIsOpening = true;
        Vector3 targetPosition = doorModel.transform.position + Vector3.back;
        while (doorModel.transform.position != targetPosition)
        {
            doorModel.transform.position = Vector3.MoveTowards(doorModel.transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        Vector2Int gridpos = IngameManager.Instance.mapManager.GetGridPositionFromWorld(pos);
        IngameManager.Instance.mapManager.GridIsWalkable(gridpos.x, gridpos.y, 2);
        isOpened = false;
        doorModel.transform.position = targetPosition;
        DoorIsOpening = false;
    }
}
