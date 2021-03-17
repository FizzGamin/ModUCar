using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : IPlayer
{
    private const float Y_ANGLE_MAX = 90;
    private const float Y_ANGLE_MIN = -90;

    public float speed = 10;
    public float sensitivity = 3;
    public float dropDistance = 2;

    private Camera playerCamera;
    private float yAngle;
    private InventoryUI inventoryUI;
    private int inventorySize;
    private IItem[] inventory;
    private int slotSelected = 0;
    private bool inventoryChanged = false;

    void Start()
    {
        GameManager.SetPlayer(this);
        this.GiveControl();

        //Camera
        playerCamera = gameObject.GetComponentInChildren<Camera>();
        yAngle = playerCamera.transform.eulerAngles.x; // Up and down is somehow x but whatever

        //Inventory
        inventoryUI = UIManager.GetInventoryUI();
        inventorySize = inventoryUI.GetSize();
        inventory = new IItem[inventorySize];
    }

    void Update()
    {
        if (isControlled)
        {
            HandleInteraction();
            HandleUse();
            HandleMovement();
            HandleInventoryKeys();
            HandlePause();

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

    private void MoveInDirection(Vector3 vector)
    {
        transform.position = transform.position + (vector.normalized * speed * Time.deltaTime);
    }

    private void HandleMovement()
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
            transform.eulerAngles = transform.eulerAngles + new Vector3(0, Input.GetAxis("Mouse X") * sensitivity, 0);
        }
        if (Input.GetAxis("Mouse Y") != 0)
        {
            yAngle = Mathf.Clamp(yAngle + Input.GetAxis("Mouse Y") * sensitivity * -1, Y_ANGLE_MIN, Y_ANGLE_MAX);
            transform.eulerAngles = new Vector3(yAngle, transform.eulerAngles.y, transform.eulerAngles.z);
        }
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

    private void HandleInventoryKeys()
    {
        for (int i = 0; i < inventorySize; i++)
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
        if (Physics.Raycast(transform.position, transform.forward, out hit, dropDistance))
        {
            dropPoint = hit.point;
        } else
        {
            dropPoint = transform.position + transform.forward * dropDistance;
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
    }

    public override void ReleaseControl()
    {
        isControlled = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        UnityEngine.Cursor.visible = true;
    }

    public override Camera GetCamera()
    {
        return gameObject.GetComponent<Camera>();
    }

    public override void Sit(GameObject seat)
    {
        throw new System.NotImplementedException();
    }

    public override void GetUp(Vector3 pos)
    {
        throw new System.NotImplementedException();
    }
}
