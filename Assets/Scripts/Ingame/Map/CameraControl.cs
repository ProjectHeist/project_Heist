using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    // Start is called before the first frame update
    public float CameraSpeed = 20.0f;
    public float scrollspeed = 2000.0f;
    private float baseAngle;
    private int rotationstep = 0;
    void Start()
    {
        baseAngle = transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        float scrollwheel = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.W))
        {
            Quaternion Rotation = Quaternion.Euler(0, rotationstep * 90f, 0);
            transform.position += Rotation * new Vector3(1, 0, 1) * CameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Quaternion Rotation = Quaternion.Euler(0, rotationstep * 90f, 0);
            transform.position -= Rotation * new Vector3(1, 0, 1) * CameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Quaternion Rotation = Quaternion.Euler(0, rotationstep * 90f, 0);
            transform.position += Rotation * new Vector3(1, 0, -1) * CameraSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Quaternion Rotation = Quaternion.Euler(0, rotationstep * 90f, 0);
            transform.position -= Rotation * new Vector3(1, 0, -1) * CameraSpeed * Time.deltaTime;
        }
        if (scrollwheel > 0.01f || scrollwheel < -0.01f)
        {
            Vector3 cameraDirection = transform.localRotation * Vector3.forward;
            transform.position += cameraDirection * Time.deltaTime * scrollwheel * scrollspeed;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            rotationstep = (rotationstep + 1) % 4;
            StartCoroutine(RotateCamera(rotationstep));
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rotationstep = (rotationstep - 1) % 4;
            StartCoroutine(RotateCamera(rotationstep));
        }
    }

    IEnumerator RotateCamera(int rotationstep)
    {
        float targetAngle = baseAngle + rotationstep * 90.0f;
        Quaternion start = transform.rotation;
        Quaternion end = Quaternion.Euler(transform.eulerAngles.x, targetAngle, transform.eulerAngles.z);
        float t = 0.0f;
        while (t < 0.2f)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(start, end, t / 0.2f);
            yield return null;
        }
        if (t >= 0.2f)
        {
            transform.rotation = end;
        }
        yield return new WaitForSeconds(0.2f);
    }
}
