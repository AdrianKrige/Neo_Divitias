﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Patrols along given path until encountering player
public class PatrollingEnemy : DamageableObject {

    public float patrolSpeed = 2f;
    public float chaseSpeed = 2.5f;
    public float chaseDistance = 10f;
    public float distanceFromPlayer = 5f;
    public Transform[] positions;
    public float rotationSpeed = 1f;
    public int damageDone = 1;
    public float fireCooldown = 1f;
    public float health = 5;
    public float agroDuration = 10f;

    public FireAtPlayer shooter;
    public bool isDestructible = true;
    public GameObject deathAnimation;

    float agroEnd = -1f;
    int currentTarget = 0;
    float currentCooldown = 0;
    Transform[] player;
    Rigidbody[] playerBody;

    public override bool damage(float damage)
    {
        // Check for damaging enemy
        if (!isDestructible) return false;
        health -= damage;
        if (health <= 0)
        {
            Instantiate(deathFX, transform.position, Quaternion.identity);
            Instantiate(deathAnimation, transform.position, Quaternion.identity);
            deathSound.Play();
            Destroy(gameObject);
            return true;
        }

        // Agro enemy onto player
        agroEnd = Time.time + agroDuration;
        return false;
    }

    void Start () {
        // Set position to 0th tranform
        transform.position = positions[0].position;

        // Find players
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        player = new Transform[players.Length];
        playerBody = new Rigidbody[players.Length];
        for (int i = 0; i < players.Length; ++i)
        {
            player[i] = players[i].transform;
            playerBody[i] = players[i].GetComponent<Rigidbody>();
        }
    }
	
	void Update () {
        int playerIndex = 0;
        float distance = float.MaxValue;

        // Find closest player
        for (int i = 0; i < player.Length; ++i)
        {
            float tempD = (player[i].position - transform.position).magnitude;
            if (tempD < distance)
            {
                playerIndex = i;
                distance = tempD;
            }
        }

        // Patrol
        if (distance > chaseDistance && agroEnd < Time.time)
        {
            float moveDist = patrolSpeed * Time.deltaTime;

            // Check if at target
            if ((transform.position - positions[currentTarget].position).magnitude < moveDist + 0.0001)
            {
                currentTarget = (currentTarget + 1) % positions.Length;
            }

            // Lerp towards goal rotation
            Quaternion currentRotation = transform.rotation;
            transform.LookAt(positions[currentTarget]);
            Quaternion goalRotation = transform.rotation;
            transform.rotation = currentRotation;
            transform.rotation = Quaternion.Lerp(currentRotation, goalRotation, rotationSpeed * Time.deltaTime);

            // Move towards goal position
            transform.position = Vector3.MoveTowards(transform.position, positions[currentTarget].position, moveDist);
        }

        // Chase
        else
        {
            float moveDist = chaseSpeed * Time.deltaTime;

            // Look at player and move towards if outside range
            transform.LookAt(player[playerIndex].position);
            if (distance > distanceFromPlayer)
            {
                transform.position = Vector3.MoveTowards(transform.position, player[playerIndex].position, moveDist);
            }

            // Fire if able
            if (currentCooldown <= 0)
            {
                shooter.straightFire(player[playerIndex], damageDone);
                currentCooldown = fireCooldown;
            } else if (currentCooldown > -1)
            {
                currentCooldown -= Time.deltaTime;
            }
        }
	}
}
