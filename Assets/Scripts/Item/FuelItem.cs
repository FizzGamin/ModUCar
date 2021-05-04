using UnityEngine;

public abstract class FuelItem : IPickupable
{
    [SerializeField]
    private double capacity = 50;
    [SerializeField]
    private double fuel = 50;

    public double GetFuel()
    {
        return fuel;
    }
    public void Refuel(IFuelContainer fuelContainer)
    {
        fuel = fuelContainer.AddFuel(fuel);
        Debug.Log("Fuel remaining: " + fuel);
    }
}
