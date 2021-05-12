using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlModuleFuelTank : MonoBehaviour, IFuelContainer, IInteractable
{
    [SerializeField]
    private double capacity = 50.0;
    [SerializeField]
    private double fuel = 50.0;

    public double AddFuel(double fuelToAdd)
    {
        if (fuel + fuelToAdd > capacity)
        {
            double ret = fuelToAdd + fuel - capacity;
            fuel = capacity;
            return ret;
        } else
        {
            fuel += fuelToAdd;
            return 0.0;
        }
    }

    public double GetFuel()
    {
        return fuel;
    }

    public bool ConsumeFuel(double fuelToConsume)
    {
        if (fuelToConsume < 0) throw new ArgumentException("Cannot consume negative fuel");
        if (fuelToConsume < fuel)
        {
            fuel -= fuelToConsume;
            return true;
        }
        return false;
    }

    public string GetInteractionText()
    {
        if (GameManager.GetPlayer()?.GetEquippedItem()?.GetComponent<FuelItem>() != null)
        {
            return "Refuel";
        }
        else
        {
            return "Refuel (Fuel container required)";
        }
    }

    public void Interact(IPlayer player)
    {
        FuelItem fuelItem;
        if ((fuelItem = GameManager.GetPlayer()?.GetEquippedItem()?.GetComponent<FuelItem>()) != null)
        {
            fuelItem.Refuel(this);
        }
    }

    public double GetCapacity()
    {
        return capacity;
    }
}
