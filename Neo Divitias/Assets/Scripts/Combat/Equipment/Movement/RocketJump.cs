﻿using UnityEngine;

public class RocketJump : MovementItem {

    public override void Activate()
    {
        if (nextActivate < Time.time)
        {
            nextActivate = Time.time + cooldown[level-1];
            StartCoroutine(cooldownTimer.abilityActivate(nextActivate, cooldown[level - 1]));
            activationSound.Play();

            // perform rocket jump
            player.AddForce(Vector3.up * movementForce[level - 1], ForceMode.Impulse);
        }
    }
}
