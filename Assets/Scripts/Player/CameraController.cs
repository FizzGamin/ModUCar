using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed = 10;
    public float sensitivity = 3;
    
    void Start()
    {
        Screen.lockCursor = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("w"))
        {
            MoveInDirection(transform.forward);
        }
        if (Input.GetKey("s"))
        {
            MoveInDirection(transform.forward * -1);
        }
        if (Input.GetKey("a"))
        {
            MoveInDirection(transform.right * -1);
        }
        if (Input.GetKey("d"))
        {
            MoveInDirection(transform.right);
        }
        if (Input.GetAxis("Mouse X") != 0)
        {
            RotateInDirection(new Vector3(0, Input.GetAxis("Mouse X") * sensitivity, 0));
        }
        if (Input.GetAxis("Mouse Y") != 0)
        {
            RotateInDirection(new Vector3(Input.GetAxis("Mouse Y") * sensitivity * -1, 0, 0));
        }
    }
    private void MoveInDirection(Vector3 vector)
    {
        transform.position = transform.position + (vector.normalized * speed * Time.deltaTime);
    }

    private void RotateInDirection(Vector3 vector)
    {
        
        transform.eulerAngles = transform.eulerAngles + vector;
    }
}
