using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : ItemSlot
{
    public override bool CanHold(IItem item)
    {
        return true;
    }
}
