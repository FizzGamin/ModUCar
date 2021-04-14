using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleUISlot : ItemSlot
{
    private ModuleSlot connectedSlot;
    public override void SetItem(IItem item)
    {
        if (!CanHold(item)) throw new ArgumentException("Tried to set module UI slot to incompatible object " + item.gameObject.name);
        if (item != null)
        {
            connectedSlot.SetModule(item.GetComponent<VehicleModule>());
        } else
        {
            connectedSlot.SetModule(null);
        }
        base.SetItem(item);
    }
    public override bool CanHold(IItem item)
    {
        if (item == null) return true; //We actually want this to be valid as well
        return item.GetComponent<VehicleModule>() != null;
    }

    public void SetConnectedModuleSlot(ModuleSlot slot)
    {
        connectedSlot = slot;
    }
}
