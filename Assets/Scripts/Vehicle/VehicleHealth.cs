using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleHealth : MonoBehaviour, IDamageable
{
    public float health;
    private float maxHealth;
    public GameObject healthBarVisibility;
    public GameObject healthBar;

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Enemy health: " + health + "  damage: " + damage);
        healthBar.GetComponent<Image>().fillAmount -= (0.01f * damage * 100 / maxHealth);
        if (health <= 0)
            Invoke(nameof(OnDeath), .1f);
    }

    public void OnDeath()
    {
        //MAYBE DROP THE MODULES BEFORE BEING DESTROYED
        Destroy(gameObject);
    }

    /// <summary>
    /// Checks if the player is inside this car gameObject.
    /// </summary>
    /// <returns>True if the player is inside the car, false otherwise.</returns>
    public bool IsPlayerInside()
    {
        return GameObject.FindGameObjectWithTag("Player").transform.root.tag != "Player";
    }

    void Awake()
    {
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayerInside())
            healthBarVisibility.SetActive(false);
        else
            healthBarVisibility.SetActive(true); ;
    }
}
