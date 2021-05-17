using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCanConsumable : FuelItem
{
    public override string GetName()
    {
        return "Gas Can";
    }

    public override ItemQuality GetQuality()
    {
        return ItemQuality.C;
    }

    public override string GetSpriteName()
    {
        return "GasCan";
    }
    public override int GetWeight()
    {
        return 200;
    }
}
