using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    public IItem GetItemInInventory(int i);
    public bool TakeItem(GameObject item);
    public GameObject GetGameObject();
}
