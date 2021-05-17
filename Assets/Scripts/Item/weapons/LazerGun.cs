using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerGun : IPickupable, IEquippable
{
    public string weaponName = "Default Name";
    public ItemQuality weaponQuality;
    public string weaponSprite;
    public int damage;

    public GameObject shotPrefab;
    public float shootRate;

    float range = 1000.0f;
    private float shootRateTimeStamp;
    private GameObject muzzle;
    private LazerShoot muzzleScript;

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
        return weaponSprite;
    }

    void Awake()
    {
        muzzle = GameObject.FindGameObjectWithTag("Player").transform.GetChild(8).gameObject;
        muzzleScript = muzzle.GetComponent<LazerShoot>();
    }

    public void Use(IPlayer player)
    {
        muzzleScript.ShootRay(shotPrefab, range, damage);
        shootRateTimeStamp = Time.time + shootRate;
    }
}
