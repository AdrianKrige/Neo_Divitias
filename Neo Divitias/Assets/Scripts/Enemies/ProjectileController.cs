﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls the projectile of an enemy, destroying it after a certain amount of time or until a collision
public class ProjectileController : MonoBehaviour {

    public float speed = 1f;
    public int damage = 1;
    public float duration = 10f;

    float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update () {
        // Move projectile forward and check for time out
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (Time.time > startTime + duration)
        {
            Destroy(gameObject);
        }
	}

    // If projectile hits
    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        DamageableObject dobj = other.GetComponent<DamageableObject>();

        // Check for damaging target
        if (dobj)
        {
            dobj.damage(damage);
        }

        Destroy(gameObject);
    }
}
