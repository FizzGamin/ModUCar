using System;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : IPlayer, IDamageable
{
    private const float Y_ANGLE_MAX = 90;
    private const float Y_ANGLE_MIN = -90;

    public int walkSpeed = 20;
    public int sprintSpeed = 30;
    public int jumpHeight = 10;
    public float sensitivity = 3;
    public float dropDistance = 5;

    private int speed;
    private float yAngle;
    private Camera playerCamera;
    private InventoryUI inventoryUI;
    private Rigidbody rb;
    private Vector3 resetRotation = Vector3.zero;

    public List<PlayerIKsolverLegs> legScripts;
    public List<PlayerIKArms> armScripts;
    public List<GameObject> legs;
    public Transform legRoot;
    public List<GameObject> arms;
    public Transform armRoot;

    public Vector3 playerStartPos;

    public float maxHP;
    private float curHP;
    public void TakeDamage(float damage)
    {
        curHP -= damage;
        if (curHP <= 0)
        {
            Invoke(nameof(OnDeath), .5f);
        }
    }
    public void OnDeath()
    {
        Debug.Log("You died");
        //for now just drop items and respawn
        Respawn();
    }
    public void Respawn()
    {
        for (int i = 0; i < inventoryUI.GetSize(); i++)
            DropItem(i);
        this.transform.position = playerStartPos;

    }

    // Start is called before the first frame update
    void Start()
    {
        playerStartPos = this.transform.position;

        GameManager.SetPlayer(this);
        this.GiveControl();

        //Camera
        playerCamera = gameObject.GetComponentInChildren<Camera>();
        yAngle = playerCamera.transform.eulerAngles.x; // Up and down is somehow x but whatever

        //Inventory
        inventoryUI = UIManager.GetInventoryUI();

        rb = GetComponent<Rigidbody>();
        speed = walkSpeed;
        resetRotation = transform.eulerAngles;

        curHP = maxHP;
    }

    void Update()
    {

        if (isControlled)
        {
            HandleUse();
            HandleMovement();
            HandleInventoryKeys();
            HandlePause();
            HandleInteraction();
        }

        // Update the Player health bar visual
        RectTransform rt = GameObject.Find("Health").GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(curHP * 6.4f, 80);
    }

    public override IItem GetItemInInventory(int i)
    {
        return inventoryUI.GetItem(i);
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

        Vector3 newVel = dir.normalized * speed;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpHeight*rb.mass, ForceMode.Impulse);
        }

        rb.velocity = new Vector3(newVel.x, rb.velocity.y, newVel.z);
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
            DropItem(inventoryUI.GetSelectedIndex());
        }
    }

    private void DropItem(int index)
    {
        if (inventoryUI.GetItem(index) == null) return;

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
        IItem cur = inventoryUI.GetItem(index);
        cur.gameObject.SetActive(true);
        cur.transform.position = dropPoint;
        inventoryUI.SetItem(index, null);
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

    public override void Sit(GameObject seat)
    {
        transform.SetParent(seat.transform);
        transform.localPosition = Vector3.zero;
        transform.up = seat.transform.forward * -1;
        transform.GetComponent<Rigidbody>().isKinematic = true;

        //disable procedural movement scripts
        foreach (PlayerIKsolverLegs p in legScripts)
        {
            p.Disable();
        }
        foreach (PlayerIKArms p in armScripts)
        {
            p.Disable();
        }

        //move legs to driving position
        int i = -1;
        foreach (GameObject o in legs)
        {
            float footSpacing = 1.5f * i;
            o.transform.position = new Vector3(legRoot.position.x + footSpacing, legRoot.position.y + 1.8f, legRoot.position.z + 2);
            i = i * -1;
        }
        //move and rotate arms into driving position
        foreach (GameObject o in arms)
        {
            float armSpacing = 1.5f * i;
            o.transform.position = new Vector3(armRoot.position.x + armSpacing, armRoot.position.y + 0.7f, armRoot.position.z + 3);
            Vector3 rot = new Vector3(-90, 270, 270);
            o.transform.rotation = Quaternion.Euler(rot);
            i = i * -1;
        }
    }

    public override void GetUp(Vector3 pos)
    {
        transform.SetParent(null);
        transform.position = pos + new Vector3(0, 4, 0);
        transform.GetComponent<Rigidbody>().isKinematic = false;
        transform.eulerAngles = resetRotation;

        //enable procedural movement scripts
        foreach (PlayerIKsolverLegs p in legScripts)
        {
            p.Enable();
        }
        foreach (PlayerIKArms p in armScripts)
        {
            p.Enable();
        }

        //reset the legs and arm positions somehow??
        
    }
}
