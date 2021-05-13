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
    public abstract IItem GetEquippedItem();

    /// <summary>
    /// A method that heals the player for a specified amount of health
    /// </summary>
    /// <param name="hp">Amount of health to heal</param>
    /// <returns>Whether or not any healing was done e.g. for whether or not to consume a healing item</returns>
    public abstract bool Heal(float hp);
    /// <summary>
    /// A method that feeds the player for a specified amount of hunger
    /// </summary>
    /// <param name="hunger">Amount of hunger to restore</param>
    /// <returns>Whether or not any hunger was restored e.g. for whether or not to consume a food item</returns>
    public abstract bool Feed(float hunger);
    public abstract void ConsumeEquipped();

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
                UIManager.GetInteractionHud().Enable(interactable.GetInteractionText());
            }
        }

        if (Input.GetKeyDown("e") && prevLookedAt != null)
        {
            VehicleController vehicleController = prevLookedAt.GetComponent<VehicleController>();
            if (vehicleController != null)
            {
                vehicleController.GetIn(this);
                UIManager.GetVehicleInteractionHud().Enable(vehicleController.GetVehicleInteractionText());
            }
        }

        //Update the currently looked at thing
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, maxInteractDistance, ~(1 << 3)))
        {
            if (hit.collider.transform.gameObject != prevLookedAt)
            {
                Debug.Log(hit.collider.transform.gameObject.name);
                prevLookedAt = hit.collider.transform.gameObject;
                VehicleController vehicleController = prevLookedAt.GetComponent<VehicleController>();
                if (vehicleController != null)
                {
                    UIManager.GetVehicleInteractionHud().Enable(vehicleController.GetVehicleInteractionText());
                } else
                {
                    UIManager.GetVehicleInteractionHud().Disable();
                }
                IInteractable interactable = FindInteractableFromObject(prevLookedAt);
                if (interactable != null)
                {
                    UIManager.GetInteractionHud().Enable(interactable.GetInteractionText());
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
            UIManager.GetVehicleInteractionHud().Disable();
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
