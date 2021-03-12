using System.Collections.Generic;
using UnityEngine;

public class VehicleController : UserControllable, IInteractable
{
    public List<WheelCollider> steeringWheelColliders;
    public List<WheelCollider> powerWheelColliders;
    public GameObject cameraCenter;

    public float vehiclePower = 10000f;
    public float vehicleSteeringAngle = 50f;
    public float cameraDistance = 10f;

    bool isControlling = false;

    //Camera related
    private Camera playerCamera;
    private Transform cameraOriginalParent = null;
    private Vector3 origCameraOffset = Vector3.zero;
    private Vector3 cameraOffset = new Vector3(0, 10, -25);
    private float mouseSensitivity = 3;

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
            HandleMouseMovement();
            HandlePause();
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

    private void HandleMouseMovement()
    {
        if (Input.GetAxis("Mouse X") != 0)
        {
            cameraCenter.transform.eulerAngles = cameraCenter.transform.eulerAngles + new Vector3(0, Input.GetAxis("Mouse X") * mouseSensitivity, 0);
        }
        if (Input.GetAxis("Mouse Y") != 0)
        {
            cameraCenter.transform.eulerAngles = new Vector3(cameraCenter.transform.eulerAngles.x + Input.GetAxis("Mouse Y") * mouseSensitivity * -1, cameraCenter.transform.eulerAngles.y, cameraCenter.transform.eulerAngles.z);
        }
    }

    private void GetOutOfVehicle()
    {
        this.ReleaseControl();
        playerCamera.transform.SetParent(cameraOriginalParent);
        playerCamera.transform.localPosition = origCameraOffset;
        GameManager.GetPlayer().GiveControl();
    }

    public void Interact(IPlayer player)
    {
        player.ReleaseControl();
        this.GiveControl();

        //Move the camera to the car
        playerCamera = player.GetCamera();
        cameraOriginalParent = playerCamera.transform.parent;
        origCameraOffset = playerCamera.transform.localPosition;
        playerCamera.transform.SetParent(cameraCenter.transform);
        playerCamera.transform.localPosition = cameraOffset;
        playerCamera.transform.LookAt(cameraCenter.transform);

        //Reset orientation of cameraCenter
        cameraCenter.transform.eulerAngles = Vector3.zero;
    }

    public string GetInteractionText()
    {
        return "Get in";
    }

    public override void GiveControl()
    {
        isControlling = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void ReleaseControl()
    {
        isControlling = false;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
}
