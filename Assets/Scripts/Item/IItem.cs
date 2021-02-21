using System;
using UnityEngine;

[Serializable]
public abstract class IItem : MonoBehaviour
{
    public Sprite itemSprite;
    public abstract GameObject CreateItem();
    public abstract string GetName();
    public abstract ItemQuality GetQuality();
    /// <summary>
    /// Gets the weight of the item for item generation
    /// </summary>
    /// <returns>An integer >= 1, standard weight is 100</returns>
    public abstract int GetWeight();
    public abstract string GetSpriteName();
}