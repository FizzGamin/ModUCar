using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPlayer : UserControllable
{
    public float maxInteractDistance = 15;

    private GameObject prevLookedAt; //This holds the current interactable object being looked at, null if not looking at an interactable

    public abstract IItem GetItemInInventory(int i);
    public abstract bool TakeItem(GameObject item);
    public abstract GameObject GetGameObject();
    public abstract Camera GetCamera();
    public abstract void Sit(GameObject seat);
    public abstract void GetUp(Vector3 pos);


    protected void HandleInteraction()
    {
        Transform cameraTransform = GetCamera().transform;

        if (Input.GetKeyDown("f") && prevLookedAt != null)
        {
            IInteractable interactable = FindInteractableFromObject(prevLookedAt);
            if (interactable != null)
            {
                //We are likely going to want this to be asynchronous in the future
                interactable.Interact(this);
                UIManager.GetInteractionHud().Enable("(F) " + interactable.GetInteractionText());
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
                    UIManager.GetInteractionHud().Enable("(F) " + interactable.GetInteractionText());
                }
                else
                {
                    UIManager.GetInteractionHud().Disable();
                }
            }
        }
        else
        {
            prevLookedAt = null;
            UIManager.GetInteractionHud().Disable();
        }
    }

    private IInteractable FindInteractableFromObject(GameObject obj)
    {
        IInteractable ret = obj.GetComponent<IInteractable>();
        if (ret != null) return ret;
        ret = obj.GetComponentInParent<IInteractable>();
        return ret;
    }
}
