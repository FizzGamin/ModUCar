using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungerBarUI : GenericBarUI
{
    void Awake()
    {
        UIManager.SetHungerBarUI(this);
    }
}
