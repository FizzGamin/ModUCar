using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YourFirstScript : MonoBehaviour
{
    private float playerMovementSpeed = 100f;
    Rigidbody rb;
    public GameObject tree;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GameObject[] tree = GameObject.FindGameObjectsWithTag("Tree");

        foreach(GameObject treeObj in tree)
            if(treeObj.GetComponent<tree>())
                treeObj.GetComponent<tree>().GetBigger();
    }

    public float getDamage()
    {
        return 5f;
        //tree.GetComponent<Tree>();

    }

    void moveLeftWheels(float force)
    {
        wheelFrontLeft.motorTorque = force;
        wheelBackLeft.motorTorque = force;
    }
    void moveRightWheels(float force)
    {
        wheelFrontRight.motorTorque = force;
        wheelBackRight.motorTorque = force;
    }

    void breakAllWheels(float force)
    {
        wheelFrontLeft.brakeTorque = force;
        wheelFrontRight.brakeTorque = force;
        wheelBackLeft.brakeTorque = force;
        wheelBackRight.brakeTorque = force;
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude > 5)
        {
            rb.velocity = rb.velocity.normalized * 5f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(-transform.up * Time.deltaTime * playerMovementSpeed, ForceMode.VelocityChange);
            //transform.position += -transform.up * Time.deltaTime * playerMovementSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(transform.up * Time.deltaTime * playerMovementSpeed, ForceMode.VelocityChange);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(-transform.right * Time.deltaTime * playerMovementSpeed, ForceMode.VelocityChange);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(transform.right * Time.deltaTime * playerMovementSpeed, ForceMode.VelocityChange);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            //collision.gameObject.GetComponent<Bullet>().getDamage();

        }
        
    }
}
