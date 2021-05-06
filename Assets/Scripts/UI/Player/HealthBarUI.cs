using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarUI : GenericBarUI
{
    void Awake()
    {
        UIManager.SetHealthBarUI(this);
    }
}
