using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FoodConsumable : IPickupable, IEquippable
{
    [SerializeField]
    private float hungerToRestore = 20;
    [SerializeField]
    private string itemName = "";
    [SerializeField]
    private ItemQuality itemQuality = ItemQuality.B;
    [SerializeField]
    private string spriteName = "";

    public override string GetName()
    {
        return itemName;
    }

    public override ItemQuality GetQuality()
    {
        return itemQuality;
    }

    public override string GetSpriteName()
    {
        return spriteName;
    }

    public void Use(IPlayer player)
    {
        if (player.Feed(hungerToRestore))
        {
            player.ConsumeEquipped();
        }
    }
}
