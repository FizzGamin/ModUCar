using UnityEngine;

public abstract class IPickupable : IItem, IInteractable
{
    public string GetInteractionText()
    {
        return "Pick up";
    }

    public void Interact(IPlayer player)
    {
        player.TakeItem(gameObject);
    }
}
