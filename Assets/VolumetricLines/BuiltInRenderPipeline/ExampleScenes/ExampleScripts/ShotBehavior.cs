using UnityEngine;
using System.Collections;

public class ShotBehavior : MonoBehaviour 
{
	private Vector3 target;
	public GameObject collisionExplosion;
	public float speed;

	private int damage;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position += transform.forward * Time.deltaTime * 1000f;

		// transform.position += transform.forward * Time.deltaTime * 300f;// The step size is equal to speed times frame time.
		float step = speed * Time.deltaTime;

		if (target != null)
		{
			if (transform.position == target)
			{
				Explode();
				return;
			}
			transform.position = Vector3.MoveTowards(transform.position, target, step);
		}
	}

	public void SetTarget(Vector3 target)
	{
		this.target = target;
	}

	private void Explode()
	{
		if (collisionExplosion != null)
		{
			GameObject explosion = (GameObject)Instantiate(
				collisionExplosion, transform.position, transform.rotation);
			Destroy(gameObject);
			Destroy(explosion, 1f);
		}
	}

	void OnTriggerEnter(Collider other)
    {
		IDamageable damageable = other.transform.gameObject.GetComponent<IDamageable>();
		if (damageable != null && damageable.GetType() != typeof(PlayerController))
		{
			damageable.TakeDamage(damage);
			Destroy(gameObject);
		}
		else if(damageable.GetType() != typeof(PlayerController))
        {
			Destroy(gameObject);
		}
		
	}

	public void SetDamage(int damage)
    {
		this.damage = damage;
    }
}
