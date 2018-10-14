﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : DamageableObject
{
    public Image fadeImg;
    public Animator fade;
    public Transform cameraTransform;
    public float currentHealth;
    public float maxHealth;
    public float regenPerSecond;
    public bool isDead;
    public GameObject[] meshParents;

    public Slider healthbar;

    public float[] extraHpPerArmourLevel;
    public float baseHp = 100;

    private Transform playerTransform;
    private Vector3 spawnLocation;
    private Quaternion spawnRotation;
    private ArrayList playerMeshes;

    public void Start()
    {
        currentHealth = maxHealth;

        playerMeshes = new ArrayList();
        foreach (GameObject parent in meshParents)
        {
            MeshRenderer[] meshes = parent.GetComponentsInChildren<MeshRenderer>();
            foreach(MeshRenderer mesh in meshes)
            {
                playerMeshes.Add(mesh);
            }
        }

        playerTransform = transform;
        spawnLocation = new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z);
        spawnRotation = new Quaternion(cameraTransform.rotation.x, cameraTransform.rotation.y, cameraTransform.rotation.z, cameraTransform.rotation.w);
        InvokeRepeating("Regenerate", 0.0f, 0.1f / regenPerSecond);
    }

    public void Update()
    {
        // check if player has fallen too far off the platforms
        if (playerTransform.position.y < -30 && !isDead)
        {
            StartCoroutine(die());
        }

        healthbar.value = currentHealth / maxHealth;
    }

    // set max hp at start of level
    public void setMaxHp (int armourLevel)
    {
        maxHealth = baseHp;
        if (armourLevel != 0) maxHealth += extraHpPerArmourLevel[armourLevel - 1];
    }

    // deal damage to the player
    public override bool damage(float damage)
    {
        if (currentHealth - damage <= 0 && !isDead)
        {
            StartCoroutine(die());
            return true;
        }
        else
        {
            currentHealth -= damage;
            return false;
        }
    }

    private IEnumerator die()
    {
        isDead = true;
        currentHealth = 0;

        // play death particle and make player invisible
        Instantiate(deathFX, transform.position, Quaternion.identity);

        foreach (MeshRenderer mr in playerMeshes)
        {
            mr.enabled = false;
        }
        deathSound.Play();

        // fade out
        fade.Play("fadeOut");

        while (fadeImg.color.a < 1)
        {
            yield return null;
        }

        StartCoroutine(respawn());
    }

    private IEnumerator respawn()
    {
        currentHealth = maxHealth;

        // set player back to start position and rotation
        playerTransform.position = spawnLocation;
        cameraTransform.rotation = spawnRotation;

        // make player visible again
        foreach (MeshRenderer mr in playerMeshes)
        {
            mr.enabled = true;
        }

        // fade in from black
        fade.Play("fadeIn");
        while (fadeImg.color.a > 0)
        {
            yield return null;
        }

        isDead = false;
    }

    // restore player health over time
    void Regenerate()
    {
        if (currentHealth < maxHealth && !isDead)
            currentHealth += 0.1f;
    }
}