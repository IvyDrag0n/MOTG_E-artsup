using System;
using System.Runtime.CompilerServices;
using Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class SC_PlayerController : MonoBehaviour
{
    //Declaring body Movement Variables
    public float movementSpeed;

    public float rotationSpeed;

    private float _rotationAxis;
    
    private float _thrustAxis;

    private Rigidbody _tankRb;
    
    //Declaring Game Objects

    [SerializeField] private GameObject headController;

    private Transform _headTransform;

    [SerializeField] private GameObject cannonController;

    private Transform _cannonTransform;

    [SerializeField] private GameObject projectile;

    [SerializeField] private GameObject cannonEnd;

    private Transform _cannonEndTransform;

    [SerializeField] private CinemachineVirtualCamera tpsCam;

    [SerializeField] private CinemachineVirtualCamera fpsCam;

    [SerializeField] private CinemachineVirtualCamera backCam;

    [SerializeField] private GameObject muzzleMain;

    private ParticleSystem muzzleMainVFX;

    [SerializeField] private GameObject projectileOrigin;

    [SerializeField] private GameObject bigExplosion;

    [SerializeField] private GameObject looseScreen;
    
    //Declaring Camera movement variables

    public float mouseSensitivityX;

    public float mouseSensitivityY;
    
    private Vector2 _viewRotation;
    
    private float _targetRotationY;

    private float _currentAngleY;

    private float _currentAngleZ;

    private bool _isFront = true;

    public float maxVelocity;
    
    //Declaring variables related to projectile handling

    public float projectilePower;

    public float health;

    [SerializeField] private float explodeForce;

    private bool isAlive;
    

    
    
    // Start is called before the first frame update
    void Start()
    {
        //Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        looseScreen.SetActive(false);
            
        //Get easier access to components
        _headTransform = headController.transform;
        _cannonTransform = cannonController.transform;
        _cannonEndTransform = cannonEnd.transform;
        _tankRb = GetComponent<Rigidbody>();
        
        //Initialize cannon vfx
        muzzleMainVFX = muzzleMain.GetComponent<ParticleSystem>();
        muzzleMainVFX.Pause();
        
        //Other Variables
        isAlive = true;
        
        //Setup cameras
        tpsCam.enabled = true;
        fpsCam.enabled = false;
        backCam.enabled = false;
        
    }

    private void Update()
    {
        if (isAlive)
        {
            if (health == 0)
            {
                LooseGame();
                isAlive = false;
                looseScreen.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Apply calculated Movement to body from handlers
        transform.Rotate(new Vector3(0,1,0), rotationSpeed * _rotationAxis);
        Vector3 forwardMovement = new Vector3(0, 0, _thrustAxis);
        //transform.Translate(movementSpeed * Time.deltaTime * forwardMovement);
        if (_tankRb.velocity.magnitude < maxVelocity)
        {
            _tankRb.AddRelativeForce(Time.deltaTime * movementSpeed * forwardMovement, ForceMode.VelocityChange);
        }
        
        //apply calculated rotation to turret and cannon empty objects within rotation limits
        _currentAngleY = _cannonTransform.localRotation.y * Mathf.Rad2Deg;
        _currentAngleZ = _headTransform.localRotation.z * Mathf.Rad2Deg;
        
        if ((_currentAngleZ < 50f && _viewRotation.x < 0) || (_currentAngleZ > -50f && _viewRotation.x > 0))
        {
            _headTransform.Rotate(Vector3.forward, mouseSensitivityX * -_viewRotation.x);
        }
        
        if ((_currentAngleY < 12f && _viewRotation.y < 0) || (_currentAngleY > -17f && _viewRotation.y > 0))
        {
            _cannonTransform.Rotate(Vector3.up, mouseSensitivityY * -_viewRotation.y);
        }
    }
    
    // Other methods

    private void LooseGame() //SpawnExplosion
    {
        Instantiate(bigExplosion, transform.position, quaternion.identity);
        _tankRb.AddForce(Vector3.up * explodeForce, ForceMode.Impulse);
    }
    
 // the following methods are events called by the Input system
 
 //Body Movement
    public void HandleRotation(InputAction.CallbackContext context)
    {
        _rotationAxis = context.ReadValue<float>();
    }

    public void HandleThrust(InputAction.CallbackContext context)
    {
        _thrustAxis = context.ReadValue<float>();
    }

    //View movement
    public void HandleView(InputAction.CallbackContext context)
    {
        _viewRotation = context.ReadValue<Vector2>();
    }

    //Shooting
    public void HandleShootMain(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameObject clone = Instantiate(projectile, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
            Quaternion cloneRotation = clone.transform.rotation;
            clone.GetComponent<Rigidbody>().AddRelativeForce(-projectilePower, 0, 0, ForceMode.Impulse);
            clone.GetComponent<SC_DefaultProjectile>().damage = 100f;
            muzzleMainVFX.Play();
        }
    }
    //Changing Camera View
    public void CameraSwitch(InputAction.CallbackContext context)
    {
        if (_isFront)
        {
            if (context.started)
            {
                tpsCam.enabled = false;
                fpsCam.enabled = true;
            }

            if (context.canceled)
            {
                tpsCam.enabled = true;
                fpsCam.enabled = false;
            }
        }
    }

    public void FrontBackSwitch(InputAction.CallbackContext context)
    {
        if (_isFront)
        {
            if (context.started)
            {
                tpsCam.enabled = false;
                fpsCam.enabled = false;
                backCam.enabled = true;
                _isFront = false;
            }
        }
        else
        {
            if (context.canceled)
            {
                tpsCam.enabled = true;
                fpsCam.enabled = false;
                backCam.enabled = false;
                _isFront = true;

            }
        }
    }
}
