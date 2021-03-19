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

    void Start()
    {
        GameManager.SetPlayer(this);
        this.GiveControl();

        //Camera
        playerCamera = gameObject.GetComponentInChildren<Camera>();
        yAngle = playerCamera.transform.eulerAngles.x; // Up and down is somehow x but whatever

        //Inventory
        inventoryUI = UIManager.GetInventoryUI();
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
        }
    }

    public override IItem GetItemInInventory(int i)
    {
        return inventoryUI.GetItem(i);
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
        if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse) && inventoryUI.GetSelectedItem() != null)
        {
            IEquippable equipped = inventoryUI.GetSelectedItem().GetComponent<IEquippable>();
            if (equipped != null)
            {
                equipped.Use(this);
            }
        }
    }

    private void HandleInventoryKeys()
    {
        for (int i = 0; i < inventoryUI.GetSize(); i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                inventoryUI.Select(i);
            }
        }

        if (Input.GetKeyDown("q"))
        {
            DropCurrentItem();
        }
    }

    private void DropCurrentItem()
    {
        if (inventoryUI.GetSelectedItem() == null) return;

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
        IItem cur = inventoryUI.GetSelectedItem();
        cur.gameObject.SetActive(true);
        cur.transform.position = dropPoint;
        inventoryUI.SetItem(inventoryUI.GetSelectedIndex(), null);
    }

    public override bool TakeItem(GameObject item)
    {
        if (inventoryUI.GetSelectedItem() == null)
        {
            item.SetActive(false);
            inventoryUI.SetItem(inventoryUI.GetSelectedIndex(), item.GetComponent<IItem>());
            return true;
        }

        return false;
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
