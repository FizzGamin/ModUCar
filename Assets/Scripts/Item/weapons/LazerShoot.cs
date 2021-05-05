using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerShoot : MonoBehaviour
{
    RaycastHit hit;

    public void ShootRay(GameObject shotPrefab, float range)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, range))
        {
            GameObject laser = Instantiate(shotPrefab, transform.position, transform.rotation);
            laser.GetComponent<ShotBehavior>().SetTarget(hit.point);
            Destroy(laser, 2f);
        }
        else
        {
            GameObject laser = Instantiate(shotPrefab, transform.position, transform.rotation);
            laser.GetComponent<ShotBehavior>().SetTarget(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
            Destroy(laser, 2f);
        }
    }
}
