using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    public IItem GetItemInInventory(int i);
    public bool TakeItem(GameObject item);
    public GameObject GetGameObject();

    /// <summary>
    /// A method to allow communication between the player and other objects that involve key bindings, gives control back to the player
    /// </summary>
    public void PassControl();

    /// <summary>
    /// Allows other scripts to take control from the player
    /// </summary>
    public void TakeControl();

    public Camera GetCamera();
}
