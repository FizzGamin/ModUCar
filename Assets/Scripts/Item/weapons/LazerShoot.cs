using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerShoot : MonoBehaviour
{
    RaycastHit hit;
    public LayerMask myLayerMask;

    public void ShootRay(GameObject shotPrefab, float range, int damage)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, range, myLayerMask))
        {
            this.transform.LookAt(hit.point);
            GameObject laser = Instantiate(shotPrefab, transform.position, transform.rotation);
            laser.GetComponent<ShotBehavior>().SetDamage(damage);
            laser.GetComponent<ShotBehavior>().SetTarget(hit.point);
            Destroy(laser, 2f);
        }
        else
        {
            Vector3 point = (Camera.main.transform.forward * 1000) + Camera.main.transform.position;
            this.transform.LookAt(point);
            GameObject laser = Instantiate(shotPrefab, transform.position, transform.rotation);
            laser.GetComponent<ShotBehavior>().SetDamage(damage);
            laser.GetComponent<ShotBehavior>().SetTarget(point);
            laser.GetComponent<ShotBehavior>().SetDistance(Mathf.Infinity);
            Destroy(laser, 2f);
        }
    }

    public void TurretShootRay(GameObject shotPrefab, float range, int damage, GameObject muzzle)
    {
        Ray ray = new Ray(muzzle.transform.position, muzzle.transform.forward);
        if (Physics.Raycast(ray, out hit, range, myLayerMask))
        {
            //this.transform.LookAt(hit.point);
            GameObject laser = Instantiate(shotPrefab, transform.position + (transform.forward * 7), transform.rotation);
            laser.GetComponent<ShotBehavior>().SetDamage(damage);
            laser.GetComponent<ShotBehavior>().SetTarget(hit.point);
            Destroy(laser, 2f);
        }
        else
        {
            Vector3 point = (muzzle.transform.forward * 1000) + muzzle.transform.position;
            //this.transform.LookAt(point);
            GameObject laser = Instantiate(shotPrefab, transform.position + (transform.forward * 7), transform.rotation);
            laser.GetComponent<ShotBehavior>().SetDamage(damage);
            laser.GetComponent<ShotBehavior>().SetTarget(point);
            laser.GetComponent<ShotBehavior>().SetDistance(Mathf.Infinity);
            Destroy(laser, 2f);
        }
    }
}
