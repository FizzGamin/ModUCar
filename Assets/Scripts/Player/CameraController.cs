using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour, IPlayer
{
    private const float Y_ANGLE_MAX = 90;
    private const float Y_ANGLE_MIN = -90;
    private const int INVENTORY_SIZE = 3;

    public float speed = 10;
    public float sensitivity = 3;
    public float maxInteractDistance = 5;
    public float dropDistance = 2;

    private float yAngle;
    private InteractionHud interactionHud;
    private InventoryUI inventoryUI;
    private PauseMenuUI pauseMenuUI;
    private GameObject prevLookedAt; //This holds the current interactable object being looked at, null if not looking at an interactable
    private IItem[] inventory;
    private int slotSelected = 0;
    private bool inventoryChanged = false;

    //This keeps track of whether or not the key-presses made are going to be handled by the camera or by something else, for example, being in menu
    private bool isControlled = true;

    void Start()
    {
        GameManager.SetPlayer(this);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        yAngle = transform.eulerAngles.x; //Up and down is somehow x but whatever
        interactionHud = UIManager.GetInteractionHud();
        inventoryUI = UIManager.GetInventoryUI();
        inventory = new IItem[INVENTORY_SIZE];
        pauseMenuUI = UIManager.GetPauseMenuUI();
    }

    void Update()
    {
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

    public IItem GetItemInInventory(int i)
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

    private void HandleInteraction()
    {
        if (Input.GetKeyDown("f") && prevLookedAt != null)
        {
            IInteractable interactable = prevLookedAt.GetComponent<IInteractable>();
            if (interactable != null)
            {
                //We are likely going to want this to be asynchronous in the future
                interactable.Interact(this);
                interactionHud.Enable("(F) " + interactable.GetInteractionText());
            }
        }

        //Update the currently looked at thing
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, maxInteractDistance))
        {
            if (hit.transform.gameObject != prevLookedAt)
            {
                prevLookedAt = hit.transform.gameObject;
                IInteractable interactable = prevLookedAt.GetComponent<IInteractable>();
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

    public bool TakeItem(GameObject item)
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

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void PassControl()
    {
        isControlled = true;
    }
}
