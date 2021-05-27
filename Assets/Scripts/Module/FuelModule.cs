using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelModule : VehicleModule, IFuelContainer
{
    [SerializeField]
    public ItemQuality quality = ItemQuality.F;
    [SerializeField]
    private double capacity = 50;
    [SerializeField]
    private double fuel = 50;

    private void Awake()
    {
    }
    public override string GetName()
    {
        return "Fuel Module " + quality.ToString();
    }

    public override ItemQuality GetQuality()
    {
        return quality;
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
        else if (GameManager.GetPlayer()?.GetEquippedItem()?.GetComponent<FuelItem>() != null)
        {
            return "Refuel";
        }
        else
        {
            return "Refuel (Fuel container required)";
        }
    }

    public override void Interact(IPlayer player)
    {
        FuelItem fuelItem;
        if (isEquipped)
        {
            if ((fuelItem = GameManager.GetPlayer()?.GetEquippedItem()?.GetComponent<FuelItem>()) != null)
            {
                fuelItem.Refuel(this);
            }
        } else
        {
            base.Interact(player);
        }
    }

    public double GetFuel()
    {
        return fuel;
    }

    public double AddFuel(double fuelToAdd)
    {
        if (fuel + fuelToAdd > capacity)
        {
            double ret = fuelToAdd + fuel - capacity;
            fuel = capacity;
            Debug.Log(fuel);
            return ret;
        }
        else
        {
            fuel += fuelToAdd;
            Debug.Log(fuel);
            return 0.0;
        }
    }

    public bool ConsumeFuel(double fuelToConsume)
    {
        if (fuelToConsume < 0) throw new ArgumentException("Cannot consume negative fuel");
        if (fuelToConsume < fuel)
        {
            fuel -= fuelToConsume;
            Debug.Log("Fuel remaining: " + fuel);
            return true;
        }
        return false;
    }

    public double GetCapacity()
    {
        return capacity;
    }
}
