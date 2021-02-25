using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class TestGun : IPickupable, IEquippable
{
    private const int MAX_DIST = 10000;
    private const float SPREAD = 2;
    private const float DAMAGE = 30;
    private const float LASER_WIDTH = .04f;

    //Eventually, this item should be held by the player and the position of the laser should depend on the gun
    private Vector3 POS_OFFSET = new Vector3(.5f, -.5f, 1.5f);

    public override string GetName()
    {
        return "Test Gun";
    }

    public override ItemQuality GetQuality()
    {
        return ItemQuality.A;
    }

    public override string GetSpriteName()
    {
        return "TestGunSprite";
    }

    public override int GetWeight()
    {
        return 100;
    }

    public void Use(IPlayer player)
    {
        GameObject playerObj = player.GetGameObject();
        Vector3 forward = playerObj.transform.forward;
        Vector3 pos = playerObj.transform.position;

        Vector3 fireVector = Quaternion.Euler(Random.Range(SPREAD, -SPREAD), Random.Range(SPREAD, -SPREAD), Random.Range(SPREAD, -SPREAD)) * forward;

        RaycastHit hit;
        if (Physics.BoxCast(pos, new Vector3(LASER_WIDTH, LASER_WIDTH, LASER_WIDTH), fireVector, out hit, playerObj.transform.rotation, MAX_DIST))
        {
            IDamageable damageable = hit.transform.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(DAMAGE);
            }
            CreateLaser(pos + playerObj.transform.TransformDirection(POS_OFFSET), hit.point, hit.distance);
        } else
        {
            CreateLaser(pos + playerObj.transform.TransformDirection(POS_OFFSET), fireVector * MAX_DIST + pos, MAX_DIST);
        }
    }

    private void CreateLaser(Vector3 pos, Vector3 hitPos, float length)
    {
        GameObject laser = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(laser.GetComponent<Collider>());
        laser.transform.position = Vector3.Lerp(pos, hitPos, .5f);
        laser.transform.forward = (hitPos - pos).normalized;
        laser.transform.localScale = new Vector3(LASER_WIDTH, LASER_WIDTH, length);
        //Lasers don't hit other lasers
        laser.layer = 2;

        Renderer laserRenderer = laser.GetComponent<Renderer>();
        laserRenderer.material.SetColor("_Color", Color.red);

        Destroy(laser, .3f);
    }

    private Vector3 GetMousePoint(GameObject playerObj)
    {
        RaycastHit hit;
        if (Physics.Raycast(playerObj.GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition), out hit, MAX_DIST))
        {
            return hit.point;
        } else
        {
            return playerObj.transform.position + playerObj.transform.forward * MAX_DIST;
        }
    }
}
