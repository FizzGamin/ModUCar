using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour, IInteractable
{
    public List<WheelCollider> steeringWheelColliders;
    public List<WheelCollider> powerWheelColliders;
    public GameObject cameraCenter;

    public float vehiclePower = 10000f;
    public float vehicleSteeringAngle = 50f;
    public float cameraDistance = 10f;

    bool isControlling = false;
    private Camera playerCamera;

    private void Start()
    {
        //Setup the vehicles substeps
        foreach (WheelCollider wc in powerWheelColliders)
        {
            wc.ConfigureVehicleSubsteps(.5f, 1, 2);
        }
        foreach (WheelCollider wc in steeringWheelColliders)
        {
            wc.ConfigureVehicleSubsteps(.5f, 1, 2);
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
            wc.steerAngle = Mathf.Lerp(wc.steerAngle, 0, 5f * Time.deltaTime);
        }

        //If the vehicle is currently being controlled, check for inputs
        if (isControlling)
        {
            RunPlayerVehicleControl();
        }
    }



    void RunPlayerVehicleControl()
    {
        float driveInfluence = Input.GetAxis("Vertical");
        float steeringInfluence = Input.GetAxis("Horizontal");

        //Apply force to go forward
        foreach (WheelCollider wc in powerWheelColliders)
        {
            wc.motorTorque = driveInfluence * vehiclePower;
        }

        //adjust steering wheel angle
        foreach (WheelCollider wc in steeringWheelColliders)
        {
            wc.steerAngle = Mathf.Lerp(wc.steerAngle, vehicleSteeringAngle * steeringInfluence, 5f * Time.deltaTime);
        }

        //Break force applied to all wheels
        if (Input.GetButton("Brakes"))
        {
            foreach (WheelCollider wc in powerWheelColliders)
            {
                wc.brakeTorque = vehiclePower * 3;
            }
            foreach (WheelCollider wc in steeringWheelColliders)
            {
                wc.brakeTorque = vehiclePower * 3;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            GetOutOfVehicle();
        }

        //DEV MODE FLIP KEY
        if (Input.GetKeyDown(KeyCode.F))
        {
            transform.position += new Vector3(0, 3, 0);
            transform.eulerAngles = Vector3.zero;
        }
    }

    private void GetOutOfVehicle()
    {
        GameManager.GetPlayer().PassControl();
    }

    public void Interact(IPlayer player)
    {
        player.TakeControl();
    }

    public string GetInteractionText()
    {
        return "Get in";
    }
}
