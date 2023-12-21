using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logics;

public class BillBoard : MonoBehaviour
{
    void LateUpdate()
    {
        transform.LookAt(transform.position + IngameManager.Instance.mainCamera.forward);
    }
}
