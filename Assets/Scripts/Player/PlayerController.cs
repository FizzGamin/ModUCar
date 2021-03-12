using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : IPlayer
{
    private const float Y_ANGLE_MAX = 90;
    private const float Y_ANGLE_MIN = -90;
    private const int INVENTORY_SIZE = 3;

    public int walkSpeed = 150;
    public int sprintSpeed = 350;
    public float sensitivity = 3;
    public float maxInteractDistance = 15;
    public float dropDistance = 5;

    private int speed;
    private float yAngle;
    private Camera playerCamera;
    private InteractionHud interactionHud;
    private InventoryUI inventoryUI;
    private GameObject prevLookedAt; //This holds the current interactable object being looked at, null if not looking at an interactable
    private IItem[] inventory;
    private int slotSelected = 0;
    private bool inventoryChanged = false;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.SetPlayer(this);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        isControlled = true;
        playerCamera = gameObject.GetComponentInChildren<Camera>();
        yAngle = playerCamera.transform.eulerAngles.x; // Up and down is somehow x but whatever
        interactionHud = UIManager.GetInteractionHud();
        inventoryUI = UIManager.GetInventoryUI();
        inventory = new IItem[INVENTORY_SIZE];
        rb = GetComponent<Rigidbody>();
        speed = walkSpeed;
    }

    void Update()
    {
        HandlePause();

        if (isControlled)
        {
            HandleInteraction();
            HandleUse();
            HandleMovement();
            HandleInventoryKeys();

            if (inventoryChanged)
            {
                InventoryUpdated();
            }
        }
    }

    public override IItem GetItemInInventory(int i)
    {
        return inventory[i];
    }

    private void HandleMovement()
    {
        Vector3 dir = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = sprintSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = walkSpeed;
        }
        //The player model is backwards..?
        if (Input.GetKey(KeyCode.W))
        {
            dir -= transform.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            dir -= transform.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            dir += transform.up;
        }
        if (Input.GetKey(KeyCode.D))
        {
            dir += transform.right;
        }
        if (Input.GetAxis("Mouse X") != 0)
        {
            transform.eulerAngles = transform.eulerAngles + new Vector3(0, Input.GetAxis("Mouse X") * sensitivity, 0);
        }
        if (Input.GetAxis("Mouse Y") != 0)
        {
            yAngle = Mathf.Clamp(yAngle + Input.GetAxis("Mouse Y") * sensitivity * -1, Y_ANGLE_MIN, Y_ANGLE_MAX);
            playerCamera.transform.eulerAngles = new Vector3(yAngle, playerCamera.transform.eulerAngles.y, playerCamera.transform.eulerAngles.z);
        }
        rb.MovePosition(transform.position + dir.normalized * Time.deltaTime * speed);
    }

    private void HandleUse()
    {
        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse) && inventory[slotSelected] != null)
        {
            IEquippable equipped = inventory[slotSelected].GetComponent<IEquippable>();
            if (equipped != null)
            {
                equipped.Use(this);
            }
        }
    }

    private void HandleInteraction()
    {
        Transform cameraTransform = GetCamera().transform;

        if (Input.GetKeyDown("f") && prevLookedAt != null)
        {
            IInteractable interactable = FindInteractableFromObject(prevLookedAt);
            if (interactable != null)
            {
                //We are likely going to want this to be asynchronous in the future
                interactable.Interact(this);
                interactionHud.Enable("(F) " + interactable.GetInteractionText());
            }
        }

        //Update the currently looked at thing
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, maxInteractDistance))
        {
            if (hit.transform.gameObject != prevLookedAt)
            {
                prevLookedAt = hit.transform.gameObject;
                IInteractable interactable = FindInteractableFromObject(prevLookedAt);
                if (interactable != null)
                {
                    interactionHud.Enable("(F) " + interactable.GetInteractionText());
                }
                else
                {
                    interactionHud.Disable();
                }
            }
        }
        else
        {
            prevLookedAt = null;
            interactionHud.Disable();
        }
    }

    private IInteractable FindInteractableFromObject(GameObject obj)
    {
        IInteractable ret = obj.GetComponent<IInteractable>();
        if (ret != null) return ret;
        ret = obj.GetComponentInParent<IInteractable>();
        return ret;
    }

    private void HandleInventoryKeys()
    {
        for (int i = 0; i < INVENTORY_SIZE; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                slotSelected = i;
                inventoryChanged = true;
            }
        }

        if (Input.GetKeyDown("q"))
        {
            DropCurrentItem();
        }
    }

    private void DropCurrentItem()
    {
        if (inventory[slotSelected] == null) return;

        RaycastHit hit;
        Vector3 dropPoint;
        Transform cameraTransform = GetCamera().transform;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, dropDistance, ~(1 >> 3)))
        {
            dropPoint = hit.point;
        }
        else
        {
            dropPoint = cameraTransform.position + cameraTransform.forward * dropDistance;
        }
        IItem cur = inventory[slotSelected];
        cur.gameObject.SetActive(true);
        cur.transform.position = dropPoint;
        inventory[slotSelected] = null;
        inventoryChanged = true;
    }

    public override bool TakeItem(GameObject item)
    {
        if (inventory[slotSelected] == null)
        {
            item.SetActive(false);
            inventory[slotSelected] = item.GetComponent<IItem>();
            inventoryChanged = true;
            return true;
        }

        return false;
    }

    private void InventoryUpdated()
    {
        inventoryChanged = false;
        inventoryUI.UpdateInventory(inventory, slotSelected);

        //Temporary inventory viewing code, should only run when the inventory is modified
        //Will eventually be placed with whatever UI code is necessary
        /*Debug.Log("Inventory:\n\n");
        for (int i = 0; i < INVENTORY_SIZE; i++)
        {
            string line = "\n[" + i + "]: ";
            if (inventory[i] != null)
            {
                line += inventory[i].GetName();
            } else+
            {
                line += "Empty";
            }
            if (slotSelected == i)
            {
                line += " <-- SELECTED";
            }
            Debug.Log(line + "\n\n");
        }*/
    }

    public override GameObject GetGameObject()
    {
        return gameObject;
    }

    public override void GiveControl()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        isControlled = true;

        //Set their speed back to normal
        speed = walkSpeed;
    }

    public override void ReleaseControl()
    {
        isControlled = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        UnityEngine.Cursor.visible = true;
    }

    public override Camera GetCamera()
    {
        return playerCamera;
    }
}
