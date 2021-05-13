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
        //For now we are just going to consume the fuel item
        GameManager.GetPlayer().ConsumeEquipped();
    }
}
