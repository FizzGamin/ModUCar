using UnityEngine;

public abstract class IItem : MonoBehaviour
{
    public virtual GameObject CreateItem()
    {
        return Instantiate(this.gameObject);
    }
    public abstract string GetName();
    public abstract ItemQuality GetQuality();
    /// <summary>
    /// Gets the weight of the item for item generation
    /// </summary>
    /// <returns>An integer >= 1, standard weight is 100</returns>
    public virtual int GetWeight()
    {
        return 100;
    }
    public abstract string GetSpriteName();
}