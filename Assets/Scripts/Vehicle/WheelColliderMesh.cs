using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelColliderMesh : MonoBehaviour
{

    private WheelCollider wheelCollider;
    private Transform wheelMesh;
    private Vector3 wheelPos;
    private Quaternion wheelRot;
    public int sideMultiplier = 1;

    // Start is called before the first frame update
    void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
        wheelMesh = transform.GetChild(0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (wheelCollider && wheelMesh)
        {
            wheelCollider.GetWorldPose(out wheelPos, out wheelRot);
            wheelMesh.position = wheelPos;
            wheelMesh.rotation = wheelRot;
            //wheelMesh.Rotate(sideMultiplier * wheelCollider.rpm / 60 * 360 * Time.fixedDeltaTime, 0, 0);
        }
    }
}
