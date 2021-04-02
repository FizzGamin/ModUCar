using UnityEngine;
using UnityEngine.UI;

public class VehicleInteractionHud : TextInteractionHud
{
    void Start()
    {
        UIManager.SetVehicleInteractionHud(this);
    }

    protected override string GetTextPrefix()
    {
        return "(E) ";
    }
}
