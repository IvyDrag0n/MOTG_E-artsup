using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class DefaultProjectile : MonoBehaviour
{
    public float damage; //Not currently used, but could be used if the game is expanded with multiplayer support
    
    [SerializeField] private GameObject explosionTemplate;
    
    public float timeLimit; //Maximum time a projectile can exist

    private float _lifeTime; //current life time of the bullet
    
    private void Start()
    {
        _lifeTime = 0; //Initialisation 
    }

    private void Update()
    {
        if (_lifeTime < timeLimit) //Update lifetime as time passes
        {
            _lifeTime += Time.deltaTime;
        }
        else //If the life time has achieved its maximum, destroy the bullet
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Spawn vfx and destroy bullet
        Instantiate(explosionTemplate, transform.position, quaternion.identity);
        Destroy(gameObject);
    }
}
