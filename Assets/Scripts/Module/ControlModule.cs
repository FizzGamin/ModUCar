using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlModule : VehicleModule
{
    public GameObject seat;

    public override void Equip(VehicleController vehicle)
    {
        throw new System.NotImplementedException();
    }

    public override string GetName()
    {
        return "Control Module";
    }

    public override ItemQuality GetQuality()
    {
        return ItemQuality.F;
    }

    public GameObject GetSeat()
    {
        return seat;
    }

    public override string GetSpriteName()
    {
        return null;
    }

    public override void Unequip(VehicleController vehicle)
    {
        throw new System.NotImplementedException();
    }
}
