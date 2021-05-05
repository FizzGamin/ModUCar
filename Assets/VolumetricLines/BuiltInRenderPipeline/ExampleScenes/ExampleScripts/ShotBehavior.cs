using UnityEngine;
using System.Collections;

public class ShotBehavior : MonoBehaviour 
{
	private Vector3 target;
	public GameObject collisionExplosion;
	public float speed;

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

	void Explode()
	{
		if (collisionExplosion != null)
		{
			GameObject explosion = (GameObject)Instantiate(
				collisionExplosion, transform.position, transform.rotation);
			Destroy(gameObject);
			Destroy(explosion, 1f);
		}


	}
}
