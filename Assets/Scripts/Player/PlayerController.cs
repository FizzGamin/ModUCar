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
    private bool controlBuffer = false;

    public List<PlayerIKsolverLegs> legScripts;
    public List<PlayerIKArms> armScripts;
    public List<GameObject> legs;
    public Transform legRoot;
    public List<GameObject> arms;
    public Transform armRoot;

    public Transform root;

    private bool immune;
    public float maxHP;
    private float curHP;
    public float maxHunger = 100;
    private float curHunger;
    private float hungerTimer = 5;
    private GenericBarUI healthBar;
    private GenericBarUI hungerBar;
    private int jumps;

    private bool isDead;
    public GameObject playerRagdoll;
    private Vector3 spawnPos;

    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        spawnPos = this.transform.position;

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
        curHunger = maxHunger;
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        immune = false;
        healthBar = UIManager.GetHealthBarUI();
        hungerBar = UIManager.GetHungerBarUI();
        healthBar.SetBar(curHP, maxHP);
        hungerBar.SetBar(curHunger, maxHunger);
    }

    void Update()
    {
        if (isControlled)
        {
            //This boolean gets set to true when control is passed back to the player, but we want to skip that update since any keys pressed
            //will then be processed here as well which is evil
            if (controlBuffer)
            {
                controlBuffer = false;
            }
            else
            {
                HandleUse();
                HandleMovement();
                HandleInventoryKeys();
                HandlePause();
                HandleInteraction();
                HandleJumps();
            }
        }
        DrainHunger();
    }

    public void TakeDamage(float damage)
    {
        if (!immune)
        {
            curHP -= damage;
            healthBar.SetBar(curHP, maxHP);
            if (curHP <= 0)
            {
                if (!isDead)
                    OnDeath();
            }
        }
    }

    public override bool Heal(float hp)
    {
        if (curHP >= maxHP) return false;
        curHP += hp;
        if (curHP > maxHP) curHP = maxHP;
        healthBar.SetBar(curHP, maxHP);
        return true;
    }

    /// <summary>
    /// When the player dies this method should be triggered.
    /// Opens the deathMenuUI, puts in a ragdoll where the player died, and offsets the player to use as camera to look at the ragdoll.
    /// </summary>
    public void OnDeath()
    {
        isDead = true;
        this.ReleaseControl();
        UIManager.GetDeathMenuUI().Open(this);

        Instantiate(playerRagdoll, this.transform.position, Quaternion.identity);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Rigidbody>().isKinematic = true;
        player.transform.position = player.transform.position + new Vector3(30, 20, 30);
        player.transform.LookAt(GameObject.FindGameObjectWithTag("Ragdoll").transform);
        player.transform.Rotate(-90, 0, 0);
    }

    /// <summary>
    /// Triggered when the user chooses to respawn after dying.
    /// resets the stats of the player, moves it to the respawn position, and deletes the ragdoll.
    /// </summary>
    public void Respawn()
    {
        curHP = maxHP;
        healthBar.SetBar(curHP, maxHP);
        curHunger = maxHunger;
        hungerBar.SetBar(curHunger, maxHunger);
        for (int i = 0; i < inventoryUI.GetSize(); i++)
            DropItem(i);

        this.GiveControl();
        isDead = false;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = spawnPos;
        player.GetComponent<Rigidbody>().isKinematic = false;
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        Destroy(GameObject.FindGameObjectWithTag("Ragdoll"));
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
        if (Input.GetKeyDown(KeyCode.Space) && jumps < 2)
        {
            rb.AddForce(Vector3.up * jumpHeight*rb.mass, ForceMode.Impulse);
            jumps++;
        }

        rb.velocity = new Vector3(newVel.x, rb.velocity.y, newVel.z);
    }

    /// <summary>
    /// Assigns a number of jumps to the player based on if they are on the ground or not.
    /// </summary>
    private void HandleJumps()
    {
        int onGround = IsOnGround();
        if (onGround == 1)
            jumps = 0;
        else if (onGround == -1)
            jumps = 1;
    }

    /// <summary>
    /// Creates 4 rays pointing down, 1 on each side of the player and checks if the ray hits the ground and uses the 
    /// length of that value to determine if the player is on the ground or not. 
    /// </summary>
    /// <returns>1 if the player is on the ground, -1 if they are not, 0 otherwise.</returns>
    public int IsOnGround()
    {
        List<Ray> rayList = new List<Ray>
        {
            new Ray(root.position - new Vector3(2, 0, 0), Vector3.down),
            new Ray(root.position + new Vector3(2, 0, 0), Vector3.down),
            new Ray(root.position - new Vector3(0, 0, 2), Vector3.down),
            new Ray(root.position + new Vector3(0, 0, 2), Vector3.down)
        };

        foreach (Ray ray in rayList)
        {
            if (Physics.Raycast(ray, out RaycastHit info, 5, Physics.AllLayers))
            {

                if (info.distance < 1.5f)
                    return 1;
                else if (jumps == 0)
                    return -1;
            }
        }
        return 0;
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
        controlBuffer = true;

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

    /// <summary>
    /// Disables procedural movement on legs and arms, moves the player to a sitting position in the car, puts the player in the seat, and makes the player visible.
    /// </summary>
    /// <param name="seat"></param>
    public override void Sit(GameObject seat)
    {
        immune = true;
        transform.SetParent(seat.transform);
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.LookRotation(seat.transform.up, seat.transform.forward * -1);
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
            o.transform.position = legRoot.position + seat.transform.TransformDirection(new Vector3(footSpacing, 1.5f, 1.5f));
            i = i * -1;
        }
        //move and rotate arms into driving position
        foreach (GameObject o in arms)
        {
            float armSpacing = 1.3f * i;
            o.transform.position = legRoot.position + seat.transform.TransformDirection(new Vector3(armSpacing, 7f, 2.3f));
            Vector3 rot = new Vector3(-200, 180, 0);
            o.transform.localRotation = Quaternion.Euler(rot);
            i = i * -1;
        }
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
    }

    /// <summary>
    /// Enables procedural movement on legs and arms, moves the player out of the seat, and makes the player visible.
    /// </summary>
    /// <param name="pos">The current position of the player.</param>
    public override void GetUp(Vector3 pos)
    {
        immune = false;
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
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
    }

    public override IItem GetEquippedItem()
    {
        return inventoryUI.GetSelectedItem();
    }

    private void DrainHunger()
    {
        hungerTimer -= Time.deltaTime;
        if (hungerTimer < 0)
        {
            hungerTimer += 5f;
            if (curHunger > 0)
            {
                curHunger -= 1;
                hungerBar.SetBar(curHunger, maxHunger);
                if (curHunger > 60 && curHP < 100)
                {
                    Heal(1);
                }
            } else
            {
                TakeDamage(2);
            }
        }
    }

    public override bool Feed(float hunger)
    {
        if (curHunger >= maxHunger) return false;
        curHunger += hunger;
        if (curHunger > maxHunger) curHunger = maxHunger;
        hungerBar.SetBar(curHunger, maxHunger);
        return true;
    }

    public void ConsumeSelectedItem()
    {

    }

    public override void ConsumeEquipped()
    {
        IItem toConsume = inventoryUI.GetSelectedItem();
        inventoryUI.SetItem(inventoryUI.GetSelectedIndex(), null);
        Destroy(toConsume.gameObject);
    }
}
