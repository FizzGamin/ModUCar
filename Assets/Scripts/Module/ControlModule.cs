using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlModule : VehicleModule
{
    public GameObject seat;

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

    protected override void OnEquip(VehicleController vehicle)
    {
        //Not implemented
    }

    protected override void OnUnequip(VehicleController vehicle)
    {
        //Not implemented
    }
}
