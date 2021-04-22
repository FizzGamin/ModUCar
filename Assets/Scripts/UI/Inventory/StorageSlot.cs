using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StorageSlot : ItemSlot
{
    private StorageModule connectedModule;
    private int index;
    public override void SetItem(IItem item)
    {
        if (!CanHold(item)) throw new ArgumentException("Tried to set module UI slot to incompatible object " + item.gameObject.name);
        connectedModule.SetItem(item, index);
        base.SetItem(item);
    }
    public override bool CanHold(IItem item)
    {
        return true;
    }

    public void SetConnectedStorageSlot(StorageModule module, int index)
    {
        connectedModule = module;
        this.index = index;
    }
}
