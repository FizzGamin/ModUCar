using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public List<WheelCollider> steeringWheelColliders;
    public List<WheelCollider> powerWheelColliders;

    public float vehiclePower = 10000f;
    public float vehicleSteeringAngle = 50f;


    bool isControlling = true;

    private void Start()
    {
        //Setup the vehicles substeps
        foreach (WheelCollider wc in powerWheelColliders)
        {
            wc.ConfigureVehicleSubsteps(10, 1, 2);
        }
        foreach (WheelCollider wc in steeringWheelColliders)
        {
            wc.ConfigureVehicleSubsteps(10, 1, 2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Reset each wheels break forces and torques back to 0
        foreach (WheelCollider wc in powerWheelColliders)
        {
            wc.brakeTorque = 0;
            wc.motorTorque = 0;
        }
        //reset the steering as well (and brakes)
        foreach (WheelCollider wc in steeringWheelColliders)
        {
            wc.brakeTorque = 0;
            wc.steerAngle = Mathf.Lerp(wc.steerAngle,0,5f*Time.deltaTime);
        }

        //If the vehicle is currently being controlled, check for inputs
        if (isControlling)
        {
            //Apply force to go forward
            if (Input.GetKey(KeyCode.W))
            {
                foreach(WheelCollider wc in powerWheelColliders)
                {
                    wc.motorTorque = vehiclePower;
                }
            }

            //Apply force to reverse
            if (Input.GetKey(KeyCode.S))
            {
                foreach (WheelCollider wc in powerWheelColliders)
                {
                    wc.motorTorque = -vehiclePower;
                }
            }

            //Turn left
            if (Input.GetKey(KeyCode.A))
            {
                foreach (WheelCollider wc in steeringWheelColliders)
                {
                    wc.steerAngle = Mathf.Lerp(wc.steerAngle, -vehicleSteeringAngle, 5f * Time.deltaTime);
                }
            }

            //Turn Right
            if (Input.GetKey(KeyCode.D))
            {
                foreach (WheelCollider wc in steeringWheelColliders)
                {
                    wc.steerAngle = Mathf.Lerp(wc.steerAngle, vehicleSteeringAngle, 5f * Time.deltaTime);
                }
            }

            //Break force applied to all wheels
            if (Input.GetKey(KeyCode.Space))
            {
                foreach (WheelCollider wc in powerWheelColliders)
                {
                    wc.brakeTorque = vehiclePower;
                }
                foreach (WheelCollider wc in steeringWheelColliders)
                {
                    wc.brakeTorque = vehiclePower;
                }
            }
        }
    }
}
