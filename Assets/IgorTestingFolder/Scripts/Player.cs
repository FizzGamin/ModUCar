using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * 5, ForceMode.VelocityChange);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            GetComponent<Rigidbody>().AddForce(Vector3.Left * 5, ForceMode.VelocityChange);
        }

        if (Input.GetKeyDown(KeyCode.D)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.Right * 5, ForceMode.VelocityChange);
        }

    }
}
