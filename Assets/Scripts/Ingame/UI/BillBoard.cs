using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    void LateUpdate()
    {
        transform.LookAt(transform.position + IngameManager.Instance.mainCamera.forward);
    }
}
