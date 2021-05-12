using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VehicleController : UserControllable, IInteractable, IDamageable
{
    public List<WheelCollider> steeringWheelColliders;
    public List<WheelCollider> powerWheelColliders;
    public GameObject cameraCenter;

    public float vehiclePower = 10000f;
    public float vehicleSteeringAngle = 50f;
    public float cameraDistance = 10f;

    private List<ModuleSlot> moduleSlots = new List<ModuleSlot>();
    private GenericBarUI fuelBar;
    private GenericBarUI vehicleHealthBar;

    public float health = 300;
    private float maxHealth = 300;
    public GameObject healthBarVisibility;
    public GameObject healthBar;
    private GenericBarUI healthBarUI;


    bool isControlling = false;

    //Camera related
    private Camera playerCamera;
    private Transform cameraOriginalParent = null;
    private Vector3 origCameraOffset = Vector3.zero;
    private Quaternion origCameraRotation = Quaternion.identity;
    private Vector3 cameraOffset = new Vector3(0, 0, -25);
    private float mouseSensitivity = 3;
    private float yAngle = 0;
    private const float Y_ANGLE_MAX = 90;
    private const float Y_ANGLE_MIN = -90;

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

        foreach (ModuleSlot moduleSlot in gameObject.GetComponentsInChildren<ModuleSlot>())
        {
            moduleSlots.Add(moduleSlot);
        }

        fuelBar = UIManager.GetFuelBarUI();
        vehicleHealthBar = UIManager.GetVehicleHealthBarUI();
    }

    // Update is called once per frame
    void Update()
    {
        //Reset each wheels break forces and torques back to 0
        
            
        foreach (WheelCollider wc in powerWheelColliders)
        {
            //IF ther eis no player in car, use breaks
            if (!isControlling)
                wc.brakeTorque = vehiclePower * 3;
            else
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
            UpdateFuelBar();
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Enemy health: " + health + "  damage: " + damage);
        healthBar.GetComponent<Image>().fillAmount -= (0.01f * damage * 100 / maxHealth);
        healthBarUI.SetBar(health, maxHealth);
        if (health <= 0)
            Invoke(nameof(OnDeath), .1f);
    }

    public void OnDeath()
    {
        //MAYBE DROP THE MODULES BEFORE BEING DESTROYED
        Destroy(gameObject);
    }

    void RunPlayerVehicleControl()
    {
        float driveInfluence = Input.GetAxis("Vertical");
        float steeringInfluence = Input.GetAxis("Horizontal");

        //We check driveInfluence so that it won't call the ConsumeFuel function if the driveInfluence is 0
        if (driveInfluence == 0 || !ConsumeFuel(driveInfluence))
        {
            if (driveInfluence != 0) Debug.Log("Out of fuel, refill the tank in ControlModule or StorageModule (or start with more fuel in either)");
            driveInfluence = 0;
        }

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
            cameraCenter.transform.localEulerAngles = cameraCenter.transform.localEulerAngles + new Vector3(0, Input.GetAxis("Mouse X") * mouseSensitivity, 0);
        }
        if (Input.GetAxis("Mouse Y") != 0)
        {
            yAngle = Mathf.Clamp(yAngle + Input.GetAxis("Mouse Y") * mouseSensitivity * -1, Y_ANGLE_MIN, Y_ANGLE_MAX);
            cameraCenter.transform.localEulerAngles = new Vector3(yAngle, cameraCenter.transform.localEulerAngles.y, cameraCenter.transform.localEulerAngles.z);
        }
    }

    private void GetOutOfVehicle()
    {
        this.ReleaseControl();
        playerCamera.transform.SetParent(cameraOriginalParent);
        playerCamera.transform.localPosition = origCameraOffset;
        playerCamera.transform.localRotation = origCameraRotation;
        GameManager.GetPlayer().GetUp(GetControlModule().GetSeat().transform.position);
        GameManager.GetPlayer().GiveControl();
    }

    public void Interact(IPlayer player)
    {
        player.ReleaseControl();
        ModuleUI newModuleUI = ModuleUI.CreateModuleUI(this);
        newModuleUI.Open(player);
    }

    public void GetIn(IPlayer player)
    {
        ControlModule controlModule = GetControlModule();
        if (controlModule != null)
        {
            player.ReleaseControl();
            this.GiveControl();

            //Move the camera to the car
            playerCamera = player.GetCamera();
            cameraOriginalParent = playerCamera.transform.parent;
            origCameraOffset = playerCamera.transform.localPosition;
            origCameraRotation = playerCamera.transform.localRotation;
            playerCamera.transform.SetParent(cameraCenter.transform);
            playerCamera.transform.localPosition = cameraOffset;
            playerCamera.transform.LookAt(cameraCenter.transform);
            playerCamera.transform.eulerAngles = new Vector3(playerCamera.transform.eulerAngles.x, playerCamera.transform.eulerAngles.y, transform.eulerAngles.z);
            player.Sit(controlModule.GetSeat());

            //Reset orientation of cameraCenter
            cameraCenter.transform.eulerAngles = Vector3.zero;
            yAngle = 0;
        }
    }

    public string GetInteractionText()
    {
        return "Open Module Menu";
    }

    public string GetVehicleInteractionText()
    {
        if (GetControlModule() != null)
        {
            return "Get in";
        }
        else
        {
            return "This vehicle needs a control module!";
        }
    }

    public override void GiveControl()
    {
        isControlling = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        fuelBar.gameObject.SetActive(true);
        UpdateFuelBar();
        vehicleHealthBar.gameObject.SetActive(true);
        vehicleHealthBar.SetBar(health, maxHealth);
        healthBarVisibility.SetActive(false);
    }

    public override void ReleaseControl()
    {
        isControlling = false;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        fuelBar.gameObject.SetActive(false);
        vehicleHealthBar.gameObject.SetActive(false);
        healthBarVisibility.SetActive(true);
    }

    private ControlModule GetControlModule()
    {
        return gameObject.GetComponentInChildren<ControlModule>();
    }

    public List<VehicleModule> GetVehicleModules()
    {
        return moduleSlots.ConvertAll((slot) => {return slot.GetModule();});
    }

    public List<ModuleSlot> GetModuleSlots()
    {
        return new List<ModuleSlot>(moduleSlots);
    }

    private bool ConsumeFuel(float driveInfluence)
    {
        List<VehicleModule> modules = GetVehicleModules();
        List<VehicleModule> nonFuelModules = new List<VehicleModule>();
        double fuelToConsume = Math.Abs((double)driveInfluence * Time.deltaTime * .1);

        //Look to take fuel from FuelModules first
        foreach (VehicleModule module in modules)
        {
            if (module == null) continue;
            FuelModule fuelModule;
            if ((fuelModule = module.GetComponent<FuelModule>()) != null)
            {
                if (fuelModule.ConsumeFuel(fuelToConsume))
                {
                    return true;
                }
            } else
            {
                nonFuelModules.Add(module);
            }
        }

        //Look for other fuel containers in the modules
        foreach (VehicleModule module in nonFuelModules)
        {
            IFuelContainer fuelContainer;
            if ((fuelContainer = module.GetComponentInChildren<IFuelContainer>()) != null)
            {
                if (fuelContainer.ConsumeFuel(fuelToConsume))
                {
                    return true;
                }
            }
        }

        //No driving for you today
        return false;
    }

    private void UpdateFuelBar()
    {
        IFuelContainer[] fuelContainers = gameObject.GetComponentsInChildren<IFuelContainer>();
        double totalCapacity = 0;
        double totalFuel = 0;

        foreach (IFuelContainer fuelContainer in fuelContainers)
        {
            totalCapacity += fuelContainer.GetCapacity();
            totalFuel += fuelContainer.GetFuel();
        }

        fuelBar.SetBar(totalFuel, totalCapacity);
    }
}
