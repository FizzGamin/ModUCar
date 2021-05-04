using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFuelContainer
{
    public double GetFuel();
    public double GetCapacity();
    /// <summary>
    /// Adds fuel to the IFuelContainer
    /// </summary>
    /// <param name="fuelToAdd">The amount of fuel to add</param>
    /// <returns>Amount of excess fuel that was not used to fill the container</returns>
    public double AddFuel(double fuelToAdd);

    public bool ConsumeFuel(double fuelToConsume);
}
