using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : IPickupable, IEquippable
{
    public string weaponName = "Default Name";
    public ItemQuality weaponQuality = ItemQuality.F;
    public Sprite weaponSprite;

    //Eventually, this item should be held by the player and the position of the laser should depend on the gun
    private Vector3 POS_OFFSET = new Vector3(.5f, -.5f, 1.5f);

    private const int MAX_DIST = 10000;
    private const float SPREAD = 2;
    private const float DAMAGE = 30;
    private const float LASER_WIDTH = .04f;

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

    public void Use(IPlayer player)
    {
        GameObject playerObj = player.GetGameObject();
        Vector3 forward = playerObj.transform.forward;
        Vector3 pos = playerObj.transform.position;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            IDamageable damageable = hit.transform.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(DAMAGE);
            }
            CreateLaser(pos + playerObj.transform.TransformDirection(POS_OFFSET), hit.point, hit.distance);
        }

        //RaycastHit hit;
        //if (Physics.BoxCast(pos, new Vector3(LASER_WIDTH, LASER_WIDTH, LASER_WIDTH), fireVector, out hit, playerObj.transform.rotation, MAX_DIST))
        //{
        //    IDamageable damageable = hit.transform.gameObject.GetComponent<IDamageable>();
        //    if (damageable != null)
        //    {
        //        damageable.TakeDamage(DAMAGE);
        //    }
        //    CreateLaser(pos + playerObj.transform.TransformDirection(POS_OFFSET), hit.point, hit.distance);
        //}
        //else
        //{
        //    CreateLaser(pos + playerObj.transform.TransformDirection(POS_OFFSET), fireVector * MAX_DIST + pos, MAX_DIST);
        //}
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
}
