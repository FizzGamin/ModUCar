using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretModule : VehicleModule
{
    public ItemQuality quality = ItemQuality.F;

    GameObject top;
    GameObject turret;
    bool enemyInSightRange;
    bool attacked;
    
    public float sightRange;
    public LayerMask whatIsEnemy;

    TurretLazer shootScript;
    public float rateOfFire;

    public override string GetName()
    {
        return "Turret Module " + quality.ToString();
    }

    public override ItemQuality GetQuality()
    {
        return quality;
    }

    public override string GetSpriteName()
    {
        return null;
    }

    protected override void OnEquip(VehicleController vehicle)
    {
        Debug.Log("Equipped " + gameObject.name);
    }

    protected override void OnUnequip(VehicleController vehicle)
    {
        Debug.Log("Unequipped " + gameObject.name);
    }

    private void Awake()
    {
        top = this.transform.GetChild(1).gameObject;
        turret = this.transform.GetChild(2).gameObject;
        attacked = false;
        shootScript = gameObject.GetComponent<TurretLazer>();
    }

    // Update is called once per frame
    void Update()
    {
        enemyInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsEnemy);

        if (enemyInSightRange)
        {
            Vector3 enemyPos = FindNearestEnemy();
            if (enemyPos != null)
                Attack(enemyPos);
        }
    }

    /// <summary>
    /// Turns the turret to the nearest enemy and shoots at it.
    /// </summary>
    /// <param name="enemyPos">The position of the enemy to attack.</param>
    private void Attack(Vector3 enemyPos)
    {
        Vector3 topLook = new Vector3(enemyPos.x, top.transform.position.y, enemyPos.z);
        Vector3 turretLook = new Vector3(enemyPos.x, CheckValue(enemyPos, 20, -7), enemyPos.z);
        top.transform.LookAt(topLook);
        turret.transform.LookAt(turretLook);

        //SHOOT
        if (!attacked)
        {
            shootScript.Fire();
            //BarrelKickback();
            attacked = true;
            Invoke(nameof(ResetAttack), rateOfFire);
        }
    }

    /// <summary>
    /// Smoothly, quickly moves the barrel backwards and then forwards again for some kickback motion.
    /// </summary>
    private void BarrelKickback()
    {
        float lerp;
        float speed = 1;
        Vector3 oldPos = turret.transform.position;

        for (lerp = 0; lerp < 1; lerp += Time.deltaTime * speed)
        {
            turret.transform.position = Vector3.Lerp(turret.transform.position, turret.transform.position - (turret.transform.forward * 5), lerp);
            new WaitForSeconds(0.1f);
        }
        for (lerp = 0; lerp < 1; lerp += Time.deltaTime * speed)
        {
            turret.transform.position = Vector3.Lerp(turret.transform.position - (turret.transform.forward * 5), oldPos, lerp);
            new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// Checks the y position value from turret to the enemy. Changes it if it is too large.
    /// </summary>
    /// <param name="enemyPos">the position of the enemy in the angle calculation.</param>
    /// <param name="maxAngle">The maximum angle allowed.</param>
    /// <param name="minAngle">The minimum angle allowed.</param>
    /// <returns>The new checked angle.</returns>
    private float CheckValue(Vector3 enemyPos, float maxAngle, float minAngle)
    {
        Vector3 flatAngle = new Vector3(enemyPos.x, turret.transform.position.y, enemyPos.z);
        Vector3 tiltAngle = new Vector3(enemyPos.x, enemyPos.y, enemyPos.z);
        float angle = Vector3.SignedAngle(flatAngle, tiltAngle, Vector3.forward);

        if (angle > maxAngle)
            return flatAngle.y;
        else if (angle < minAngle)
            return flatAngle.y;
        else
            return enemyPos.y;
    }

    /// <summary>
    /// Finds the nearest enemy to this turret module.
    /// </summary>
    /// <returns>The closest enemy's position, null if there is no closest enemy.</returns>
    private Vector3 FindNearestEnemy()
    {
        GameObject[] spiders = GameObject.FindGameObjectsWithTag("Enemy_Spider");
        GameObject[] trucks = GameObject.FindGameObjectsWithTag("Enemy_Truck");

        GameObject closest = null;
        float closestLength = Mathf.Infinity;
        for (int i = 0; i < spiders.Length; i++)
        {
            float dist = Vector3.Distance(spiders[i].transform.position, this.transform.position);
            if (dist < closestLength)
            {
                closest = spiders[i];
                closestLength = dist;
            }
        }
        for (int i = 0; i < trucks.Length; i++)
        {
            float dist = Vector3.Distance(trucks[i].transform.position, this.transform.position);
            if (dist < closestLength)
            {
                closest = trucks[i];
                closestLength = dist;
            }
        }

        return closest.transform.Find("CenterPoint").transform.position;
    }

    private void ResetAttack()
    {
        attacked = false;
    }

    /// <summary>
    /// Creates a gizmo showing the sight range of the Spider
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
