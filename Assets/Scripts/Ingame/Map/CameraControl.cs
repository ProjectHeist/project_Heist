using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    // Start is called before the first frame update
    public float CameraSpeed = 20.0f;
    public float scrollspeed = 2000.0f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float scrollwheel = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 cameraDirection = transform.localRotation * Vector3.forward;
            transform.position += new Vector3(1, 0, 1) * CameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= new Vector3(1, 0, 1) * CameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(1, 0, -1) * CameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= new Vector3(1, 0, -1) * CameraSpeed * Time.deltaTime;
        }
        if (scrollwheel > 0.01f || scrollwheel < -0.01f)
        {
            Vector3 cameraDirection = transform.localRotation * Vector3.forward;
            transform.position += cameraDirection * Time.deltaTime * scrollwheel * scrollspeed;
        }
    }
}
