﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Swarm minion that follows the swarm center around
public class SwarmMinion : DamageableObject {

    public float patrolSpeed = 1f;
    public float swarmSpeed = 5f;
    public int damageDone = 1;
    public float damageCooldown = 2f;
    public float health = 1;
    public GameObject deathAnimation;

    public SwarmEnemy swarm;

    Vector3 targetPosition;
    float currentCooldown = 0;

    // Choose random spot to move to
	void Start () {
        targetPosition = randomLocation();
	}

    // Damage and destroy the minion if it dies as a result
    public override bool damage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Instantiate(deathFX, transform.position, Quaternion.identity);
            Instantiate(deathAnimation, transform.position, Quaternion.identity);
            deathSound.Play();
            Destroy(gameObject);
            return true;
        }

        return false;
    }

    void Update () {
        float moveDist = patrolSpeed * Time.deltaTime;

        // Check for swarming the player
        if (swarm.swarming) moveDist = swarmSpeed * Time.deltaTime;

        // Check if at target
        if ((transform.position - targetPosition).magnitude < moveDist + 0.0001)
        {
            targetPosition = randomLocation();
        }

        // Move towards goal position and rotation
        transform.LookAt(targetPosition);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveDist);

        // Reduce cooldown
        if (currentCooldown > -1)
        {
            currentCooldown -= Time.deltaTime;
        }
    }

    // Damage player on a collision
    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        DamageableObject dobj = other.GetComponent<DamageableObject>();

        if (other.tag == "Player" && currentCooldown <= 0)
        {
            dobj.damage(damageDone);
            currentCooldown = damageCooldown;
        }
    }

    // Find spot around swarm center to move to
    Vector3 randomLocation()
    {
        return Random.onUnitSphere * swarm.radius + swarm.transform.position;
    }
}
