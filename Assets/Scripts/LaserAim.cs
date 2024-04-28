using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAim : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    [SerializeField] private PlayerController playerController;

    private Transform projectileOrigin;

    [SerializeField] private Transform projectileEnd;
    
    private LayerMask projectileCollisionMask;


    private void Awake()
    {
        // get objects the projectile can collide with
        int projectileLayer = playerController.projectile.gameObject.layer;
        for (int i = 0; i < 32; i++)
        {
            if (!Physics.GetIgnoreLayerCollision(projectileLayer, i)) //if the projectile can collide with the layer, add it to the mask
            {
                projectileCollisionMask |= 1 << i;
            }
        }
    }

    private void Start()
    {
        projectileOrigin = playerController.projectileOrigin.transform; //For easier access
    }

    private void Update()
    {
        DrawProjection();
    }

    private void DrawProjection()
    {
        //setup variables
        lineRenderer.positionCount = 2;
        Vector3 startPosition = projectileOrigin.position;
        Vector3 endPosition = projectileEnd.position;
        
        //Set two positions of the laser
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1,endPosition);
        
        //Check if the laser collides with an object
        if (Physics.Raycast(startPosition,
                (endPosition - startPosition).normalized,
                out RaycastHit hit,
                (endPosition - startPosition).magnitude,
                projectileCollisionMask))
        {
            lineRenderer.SetPosition(1, hit.point); //Stop the laser at the object
        }
    }
}
