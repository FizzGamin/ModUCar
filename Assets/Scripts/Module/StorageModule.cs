using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageModule : VehicleModule
{
    [SerializeField]
    public ItemQuality quality = ItemQuality.F;
    [SerializeField]
    public int numSlots = 5;
    private IItem[] items;

    private void Awake()
    {
        items = new IItem[numSlots];
    }
    public override string GetName()
    {
        return "Storage Module " + quality.ToString();
    }

    public override ItemQuality GetQuality()
    {
        return quality;
    }

    public override string GetSpriteName()
    {
        return null;
    }

    protected override void OnEquip(VehicleController vehicle)
    {
        Debug.Log("Equipped " + gameObject.name);
    }

    protected override void OnUnequip(VehicleController vehicle)
    {
        Debug.Log("Unequipped " + gameObject.name);
    }

    public override string GetInteractionText()
    {
        if (!isEquipped) return base.GetInteractionText();
        else return "Open storage";
    }

    public override void Interact(IPlayer player)
    {
        if (!isEquipped) base.Interact(player);
        else
        {
            player.ReleaseControl();
            StorageUI storageUI = StorageUI.CreateStorageUI(this);
            storageUI.Open(player);
        }
    }

    public IItem SetItem(IItem item, int i)
    {
        if (i >= numSlots) throw new ArgumentException("Index for StorageModule.SetItem out of bounds");
        IItem ret = items[i];
        items[i] = item;
        return ret;
    }

    public IItem[] GetItems()
    {
        return (IItem[])items.Clone();
    }

}
