using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.iOS;

public class SimpleControl : MonoBehaviour {
	
	//all left wheels
    public GameObject[] leftWheels;

	//all right wheels
    public GameObject[] rightWheels;

    public GameObject leftTrack;
    private Renderer _leftTrackRenderer;


    public GameObject rightTrack;
    private Renderer _rightTrackRenderer;


    public float wheelsSpeed;
    public float tracksSpeed;

    private float _thrustAxis;
    private float _rotationAxis;

    private Vector2 rotationRatio;

    private void Start()
    {
        _leftTrackRenderer = leftTrack.transform.GetComponent<Renderer>();
        _rightTrackRenderer = rightTrack.transform.GetComponent<Renderer>();
    }

    void Update ()
    {
        rotationRatio = CalculateRotationRatio(_thrustAxis, _rotationAxis);
        
        //Wheels Rotation
        foreach (GameObject wheel in leftWheels)
        {
            wheel.transform.Rotate(new Vector3(wheelsSpeed * rotationRatio.x, 0,0));
        }

        foreach (GameObject wheel in rightWheels)
        {
            wheel.transform.Rotate(new Vector3(wheelsSpeed * rotationRatio.y, 0, 0));
        }
        
        //Tracks Rotation
        _leftTrackRenderer.material.mainTextureOffset += new Vector2(0, Time.deltaTime * tracksSpeed * rotationRatio.x);
        _rightTrackRenderer.material.mainTextureOffset += new Vector2(0, Time.deltaTime * tracksSpeed * rotationRatio.y);
        
    }
    
    //Get player movement input (thrust)
    public void HandleThrust(InputAction.CallbackContext context)
    {
        _thrustAxis = context.ReadValue<float>();
    }
    
    //Get player movement input (steering)
    public void HandleRotation(InputAction.CallbackContext context)
    {
        _rotationAxis = context.ReadValue<float>();
    }

    //Calculate left and right track/wheel rotation coefficient
    private Vector2 CalculateRotationRatio(float thrust, float steering)
    {
        float leftSpeed = 0;
        float rightSpeed = 0;
        
        if (steering == 0f) //If the tank is not steering, the thrust should control the rotation ratio
        {
            leftSpeed = thrust;
            rightSpeed = thrust;
        }

        if (thrust == 0f) // if the tank is steering on the spot, the rotation of the tracks should be opposed
        {
            leftSpeed = steering;
            rightSpeed = -steering;
        }

        else //This formula calculates the ratio for a composite ratio, with the thrust deciding the positive/negative, and the steering the amount
        {
            leftSpeed = thrust * (.25f * steering + .75f);
            rightSpeed = thrust * (-.25f * steering + .75f);
        }
        
        return new Vector2(leftSpeed,rightSpeed);
    }
}
