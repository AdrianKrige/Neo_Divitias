﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls physics, movement and movement input for the first player
// Uses primary input from joystick controller
// Uses secondary input for mouse and keyboard control
public class Player1Controller : PlayerController {
    PlayerHealth playerHealth;
    Rigidbody body;
    Vector3 forward, right;
    float distToGround;

    void Start () {
        playerHealth = GetComponent<PlayerHealth>();
        body = GetComponent<Rigidbody>();
        forward = transform.forward;
        right = transform.right;
        distToGround = GetComponent<Collider>().bounds.extents.y;
	}
	
	void FixedUpdate () {
        // Get speed and max velocity
        float moveSpeedCurrent = moveSpeed;
        float maxVelocityCurrent = maxVelocity;

        if (Input.GetButton("Sprint p1") || Input.GetButton("Sprint"))
        {
            moveSpeedCurrent *= sprintMultiplier;
        }

        // Aim in direction of camera
        forward = currentCamera.transform.forward;
        forward.y = 0;
        right = currentCamera.transform.right;
        right.y = 0;

        if (!playerHealth.isDead)
        {
            // Set velocity
            if (Input.GetAxis("Vertical p1") > 0 || Input.GetAxis("Vertical mouse") > 0)
            {
                body.AddForce(forward * moveSpeedCurrent);
            }
            else if (Input.GetAxis("Vertical p1") < 0 || Input.GetAxis("Vertical mouse") < 0)
            {
                body.AddForce(-forward * moveSpeedCurrent);
            }

            if (Input.GetAxis("Horizontal p1") > 0 || Input.GetAxis("Horizontal mouse") > 0)
            {
                body.AddForce(right * moveSpeedCurrent);
            }
            else if (Input.GetAxis("Horizontal p1") < 0 || Input.GetAxis("Horizontal mouse") < 0)
            {
                body.AddForce(-right * moveSpeedCurrent);
            }
        }

        // If not movement input
        if ((Input.GetAxis("Horizontal p1") == 0 || Input.GetAxis("Horizontal mouse") == 0)
            && (Input.GetAxis("Vertical p1") == 0 || Input.GetAxis("Vertical mouse") == 0) )
        {
            Vector3 reverse = -body.velocity;
            reverse.y = 0;
            body.AddForce(reverse * friction);
        }

        // Clamp horizontal velocity

        if (restrictVel)
        {
            float y = body.velocity.y;
            body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
            body.velocity = Vector3.ClampMagnitude(body.velocity, maxVelocityCurrent);
            body.velocity = new Vector3(body.velocity.x, y, body.velocity.z);
        }
    }

    void Update()
    {
        // Check for jumping
        if ((Input.GetButtonDown("Jump p1") || Input.GetButtonDown("Jump")) && isGrounded() && !playerHealth.isDead)
        {
            body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // Check if still on the ground
    bool isGrounded ()
    {
        return Physics.Raycast(transform.position, Vector3.down, distToGround + 0.2f);
    }
}
