using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.Mathematics;
using UnityEditor.Timeline;
using UnityEngine;

public class SC_DefaultProjectile : MonoBehaviour
{
    public float damage;

    public bool isStunning;
    
    [SerializeField] private GameObject explosionTemplate;
    
    private Rigidbody rb;

    private GameObject explosion;

    public float timeLimit;

    private float _lifeTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _lifeTime = 0;
        

    }

    private void Update()
    {
        if (_lifeTime < timeLimit)
        {
            _lifeTime += Time.deltaTime;
           
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        explosion = Instantiate(explosionTemplate, transform.position, quaternion.identity);
        Destroy(gameObject);
        

    }
}
