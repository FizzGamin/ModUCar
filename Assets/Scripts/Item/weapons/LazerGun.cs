using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerGun : IPickupable, IEquippable
{
    public string weaponName = "Default Name";
    public ItemQuality weaponQuality = ItemQuality.F;
    public Sprite weaponSprite;

    public GameObject shotPrefab;
    public float shootRate;

    float range = 1000.0f;
    private float shootRateTimeStamp;
    private GameObject muzzle;
    private LazerShoot muzzleScript;
    private Vector3 localPos;

    public override string GetName()
    {
        return weaponName;
    }

    public override ItemQuality GetQuality()
    {
        return weaponQuality;
    }

    public override string GetSpriteName()
    {
        if (weaponSprite)
            return weaponSprite.name;
        else
            return "MissingSprite";
    }

    public Vector3 GetLocalPos()
    {
        return localPos;
    }

    void Awake()
    {
        muzzle = this.transform.GetChild(3).gameObject;
        muzzleScript = muzzle.GetComponent<LazerShoot>();

        localPos = new Vector3(2.25f, -2.5f, 7.25f);
    }

    public void Use(IPlayer player)
    {
        /*
            IDamageable damageable = hit.transform.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(DAMAGE);
            }
        }*/

        if (Time.time > shootRateTimeStamp)
        {
            muzzleScript.ShootRay(shotPrefab, range);
            shootRateTimeStamp = Time.time + shootRate;
        }
    }
}
