using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerShoot : MonoBehaviour
{
    RaycastHit hit;

    public void ShootRay(GameObject shotPrefab, float range, int damage)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, range))
        {
            this.transform.LookAt(hit.point);
            GameObject laser = Instantiate(shotPrefab, transform.position, transform.rotation);
            laser.GetComponent<ShotBehavior>().SetDamage(damage);
            laser.GetComponent<ShotBehavior>().SetTarget(hit.point);
            Destroy(laser, 2f);
        }
        else
        {
            Vector3 point = (Camera.main.transform.forward * 1000) + Camera.main.transform.position;//ViewportToWorldPoint(Input.mousePosition);
            Debug.Log(Input.mousePosition);
            this.transform.LookAt(point);
            GameObject laser = Instantiate(shotPrefab, transform.position, transform.rotation);
            laser.GetComponent<ShotBehavior>().SetDamage(damage);
            laser.GetComponent<ShotBehavior>().SetTarget(point);
            Destroy(laser, 2f);
        }
    }
}
