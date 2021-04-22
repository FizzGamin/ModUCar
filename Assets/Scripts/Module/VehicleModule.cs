using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VehicleModule : IItem, IInteractable
{
    [SerializeField]
    protected bool isEquipped = false;


    public virtual string GetInteractionText()
    {
        if (!isEquipped) return "Pick up " + GetName();
        else return null;
    }

    public virtual void Interact(IPlayer player)
    {
        if (!isEquipped) player.TakeItem(gameObject);
    }

    public void Equip(VehicleController vehicle)
    {
        isEquipped = true;
        OnEquip(vehicle);
    }

    protected abstract void OnEquip(VehicleController vehicle);

    public void Unequip(VehicleController vehicle)
    {
        isEquipped = false;
        OnUnequip(vehicle);
    }

    protected abstract void OnUnequip(VehicleController vehicle);
}
