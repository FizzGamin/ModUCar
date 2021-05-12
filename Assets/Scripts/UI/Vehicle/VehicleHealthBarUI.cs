using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleHealthBarUI : GenericBarUI
{
    void Awake()
    {
        UIManager.SetVehicleHealthBarUI(this);
        gameObject.SetActive(false);
    }
}
