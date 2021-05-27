using UnityEngine;
using System.Collections;

public class ShotBehavior : MonoBehaviour 
{
	private Vector3 target;
	public GameObject collisionExplosion;
	public float speed;
	private float distance;
	private bool hasHit = false;

	private int damage;

	public void SetTarget(Vector3 target)
	{
		this.target = target;
		distance = Vector3.Distance(target, this.transform.position);
	}

	public void SetDistance(float dist)
    {
		distance = dist;
    }
	public void SetDamage(int damage)
    {
		this.damage = damage;
    }

	// Update is called once per frame
	void Update () 
	{
		transform.position += transform.forward * Time.deltaTime * 1000f;

		// transform.position += transform.forward * Time.deltaTime * 300f;// The step size is equal to speed times frame time.
		float step = speed * Time.deltaTime;

		if (target != null)
		{
			float dist = Vector3.Distance(target, this.transform.position);
			if (dist > distance)//transform.position == target)
			{
				Destroy(gameObject);
				Explode();
				return;
			}
			transform.position = Vector3.MoveTowards(transform.position, target, step);
			distance = Vector3.Distance(target, this.transform.position);
		}
	}

	/// <summary>
	/// Instatiates an explosion particle system the destroys it for an explosion effect.
	/// </summary>
	private void Explode()
	{
		if (collisionExplosion != null)
		{
			GameObject explosion = (GameObject)Instantiate(
				collisionExplosion, transform.position , transform.rotation);
			Destroy(gameObject);
			Destroy(explosion, 1f);
		}
	}

	void OnTriggerEnter(Collider other)
    {
		GameObject parent = other.transform.root.gameObject;
		Debug.Log("Bullet hit: " + parent);
		if (parent.tag != "Player")
		{
			if (!hasHit)
			{
				hasHit = true;
				Destroy(gameObject);
				Explode();
				IDamageable damageable = other.GetComponentInParent<IDamageable>();
				if (damageable != null)
					damageable.TakeDamage(damage);
			}
		}
	}


}
