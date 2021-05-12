using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretLazer : MonoBehaviour
{
    public int damage;

    public GameObject shotPrefab;
    //public float shootRate;

    float range = 1000.0f;
    private float shootRateTimeStamp;
    private GameObject muzzle;
    private LazerShoot muzzleScript;


    void Awake()
    {
        muzzle = gameObject.transform.Find("turret").gameObject;
        muzzleScript = muzzle.GetComponent<LazerShoot>();
    }

    public void Fire()
    {
        muzzleScript.TurretShootRay(shotPrefab, range, damage, muzzle);
        //shootRateTimeStamp = Time.time + shootRate;
    }
}
