using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public PlayerController player;

    public Animator animator;

    public scoreCounter scoreCounter;

    [SerializeField] private BoxCollider targetCollider;

    private void OnCollisionEnter(Collision other)
    {
        if (animator != null)
        {
            if (other.collider.CompareTag("Projectile")) //detect when the target is hit
            {
                animator.SetTrigger("onShoot"); //play fly away animation
                
                //Update score
                player.score += 100; 
                scoreCounter.UpdateScore(player.score);
                
                //Disable collider to avoid negative scale issues
                targetCollider.enabled = false;

            }
        }
    }
}
