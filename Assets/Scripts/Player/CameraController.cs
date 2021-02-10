using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour, IPlayer
{
    private const float Y_ANGLE_MAX = 90;
    private const float Y_ANGLE_MIN = -90;
    private const string INTERACTION_HUD_NAME = "InteractionHud";

    public float speed = 10;
    public float sensitivity = 3;
    public float maxInteractDistance = 5;
    public GameObject testObj;

    private float yAngle;
    private Text interactionHud;
    private GameObject prevLookedAt; //This holds the current interactable object being looked at, null if not looking at an interactable
    
    void Start()
    {
        GameManager.instance.SetPlayer(this);
        Debug.Log(testObj.GetComponent<IItem>().GetWeight());
        Screen.lockCursor = true;
        yAngle = transform.eulerAngles.x; //Up and down is somehow x but whatever
        interactionHud = GameObject.Find(INTERACTION_HUD_NAME).GetComponent<Text>();
    }

    void Update()
    {
        HandleInteraction();
        HandleMovement();

        //DEBUG
        if (Input.GetKeyDown("e"))
        {
            GameManager g = GameManager.instance;
            LootService s = g.GetLootService();
            s.TestItemGeneration(ItemQuality.B, 100000);
        }
    }

    public IItem GetItemInInventory(int i)
    {
        return null;
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

    private void HandleInteraction()
    {
        if (Input.GetKeyDown("f"))
        {
            IInteractable interactable = prevLookedAt.GetComponent<IInteractable>();
            if (interactable != null)
            {
                //We are likely going to want this to be asynchronous in the future
                interactable.Interact(this);
                interactionHud.text = "(F) " + interactable.GetInteractionText();
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
                    interactionHud.text = "(F) " + interactable.GetInteractionText();
                    interactionHud.enabled = true;
                }
                else
                {
                    interactionHud.text = "";
                    interactionHud.enabled = false;
                }
            }
        }
        else
        {
            prevLookedAt = null;
            interactionHud.text = "";
            interactionHud.enabled = false;
        }
    }
}
