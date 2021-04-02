using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VehicleModule : IItem, IInteractable
{
    [SerializeField]
    protected bool isEquipped = false;

    public string GetInteractionText()
    {
        if (!isEquipped) return "Pick up " + GetName();
        else return null;
    }

    public void Interact(IPlayer player)
    {
        if (!isEquipped) player.TakeItem(gameObject);
    }

    public abstract void Equip(VehicleController vehicle);

    public abstract void Unequip(VehicleController vehicle);
}
