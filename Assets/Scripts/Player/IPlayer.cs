using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPlayer : UserControllable
{
    public abstract IItem GetItemInInventory(int i);
    public abstract bool TakeItem(GameObject item);
    public abstract GameObject GetGameObject();

    public abstract Camera GetCamera();
}
