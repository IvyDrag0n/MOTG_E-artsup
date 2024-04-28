using System;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    /* Declaring body Movement Variables */
    public float movementSpeed;

    public float rotationSpeed;

    private float _rotationAxis; // Player Input
    
    private float _thrustAxis; // Player Input

    private Rigidbody _tankRb; 

    private bool isBoosting; // Player Input

    private bool isGrounded;
        
    [SerializeField] private List<GameObject> smokeTrails; //VFX

    /* Sound Management Variables */

    [SerializeField] private AudioSource engineRev;

    private float normalSpeedRevPitch;

    [SerializeField] private AudioSource tankFire;

    [SerializeField] private AudioSource tankHit;
    
    /* Declaring End Game Variables */
    
    [SerializeField] private GameObject bigExplosion;

    [SerializeField] private GameObject looseScreen; //Death screen

    [SerializeField] private GameObject endScreen; // Timer end Screen

    [SerializeField] private float explodeForce;

    /* Declaring Camera movement variables */
    
    //The cameras
    [SerializeField] private CinemachineVirtualCamera tpsCam;
                                         
    [SerializeField] private CinemachineVirtualCamera fpsCam;
                                         
    [SerializeField] private CinemachineVirtualCamera backCam;
    
    //Tank Objects
    [SerializeField] private GameObject headController;

    private Transform _headTransform;

    [SerializeField] private GameObject cannonController;

    private Transform _cannonTransform;
    
    //Variables
    public float mouseSensitivityX;

    public float mouseSensitivityY;
    
    private Vector2 _viewRotation; // Player Input
    
    //private float _targetRotationY;

    private float _currentAngleY;

    private float _currentAngleZ;

    private bool _isFront = true; 

    public float maxVelocity;

    private float currentMaxVelocity;

    private float currentMovementSpeed;

    private float currentRotationSpeed;

    private float currentMouseSensitivityX;
    
    private float currentMouseSensitivityY;
    
    
    /* Declaring variables related to projectile handling and player status */
    
    //Game Objects
    public GameObject projectile;
    
    public GameObject projectileOrigin;

    [SerializeField] private GameObject cannonEnd;

    private Transform _cannonEndTransform;

    [SerializeField] private GameObject muzzleMain;

    private ParticleSystem muzzleMainVFX;
    
    public StatusBar statusBar;

    public GameObject hud;
    
    //Variables
    public float projectilePower;

    public float health;

    public float vigor;

    private bool isHurting;
    
    private bool isAlive;

    private bool canShoot;

    public float shootCooldown;

    private float shootTime;

    public int score = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        //Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        //Hide Game Over UI
        looseScreen.SetActive(false);
        endScreen.SetActive(false);
            
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
        currentMaxVelocity = maxVelocity;
        currentMovementSpeed = movementSpeed;
        currentRotationSpeed = rotationSpeed;
        currentMouseSensitivityX = mouseSensitivityX;
        currentMouseSensitivityY = mouseSensitivityY;
        canShoot = true;
        
        //Setup cameras
        tpsCam.enabled = true;
        fpsCam.enabled = false;
        backCam.enabled = false;
        
        //Setup Sound
        engineRev.pitch = 1;
        normalSpeedRevPitch = 1f;

    }

    private void Update()
    {
        if (isAlive)
        {
            if (health <= 0) // If the player gets to 0 health, he dies
            {
                //Make the tank explode
                Instantiate(bigExplosion, transform.position, quaternion.identity);
                _tankRb.AddForce(Vector3.up * explodeForce, ForceMode.Impulse);
                
                isAlive = false;
                
                //Show relevant UI only
                looseScreen.SetActive(true);
                hud.SetActive(false);
                
                //Show mouse
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                
                //Disable Tank Control
                currentMaxVelocity = 0f;
                currentRotationSpeed = 0f;
                currentMouseSensitivityX = 0f;
                currentMouseSensitivityY = 0f;
                
            }

            if (statusBar.isGameOver) //if the timer has run out, the game ends
            {
                //Show relevant UI
                endScreen.SetActive(true);
                hud.SetActive(false);
                
                //Show mouse
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                
                //Disable Tank Control
                currentMaxVelocity = 0f;
                currentRotationSpeed = 0f;
                currentMouseSensitivityX = 0f;
                currentMouseSensitivityY = 0f;
            }
            
            if (vigor == 0) //If vigor Runs out, revert movement to normal
            {
                //Set the speed
                currentMovementSpeed = movementSpeed;
                currentMaxVelocity = maxVelocity;

                isBoosting = false;
                
                foreach (GameObject trail in smokeTrails) //reset vfx color and size
                {
                    ParticleSystem.MainModule pMain = trail.GetComponent<ParticleSystem>().main;
                    pMain.startSizeMultiplier = 1;
                    pMain.startColor = new Color(208, 201, 208);

                }
            }
            
            if (isBoosting && vigor > 0) //While the player is boosting, consume vigor
            {
                vigor -= .5f;
                engineRev.pitch += (3f - engineRev.pitch) * Time.deltaTime; //The engine smoothly speeds up !
            }

            if (!isBoosting && (vigor < 100f)) //While not boosting, restore vigor
            {
                vigor += .5f;
            }
            
            
            
            statusBar.SetVigor(vigor); // Update vigor display

            if (isHurting && !statusBar.isGameOver) //If the player is in the fire, reduce health 
            {
                health -= .2f;
                statusBar.SetHealth(health); // Update health bar
            }
        }

        if (!canShoot && (Time.time >= shootTime + shootCooldown)) // Check if shoot cooldown has passed
        {
            canShoot = true;
        }

        if (!isAlive || statusBar.isGameOver) // When the game is over, the motor should smoothly stop
        {
            engineRev.pitch -= engineRev.pitch * Time.deltaTime;
        }
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.collider.CompareTag("Ground"))
        {
            tankHit.Play(); //Play tank hitting sound when hitting objects
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.collider.CompareTag("Ground")) //Detect when the tank is grounded
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("Ground")) //Detect when the player leaves the ground
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("hurtPlayer")) //Detect when the player enters the fire
        {
            isHurting = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("hurtPlayer")) // Detects when the player exits the fire
        {
            isHurting = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isGrounded) //Can only move if touching the ground
        {
            //Rotate tank depending on the player input
            transform.Rotate(new Vector3(0,1,0), currentRotationSpeed * _rotationAxis);
            
            //Move tank depending on player input
            Vector3 forwardMovement = new Vector3(0, 0, _thrustAxis);
            if (_tankRb.velocity.magnitude < currentMaxVelocity) //limit acceleration
            {
                _tankRb.AddRelativeForce(Time.deltaTime * currentMovementSpeed * forwardMovement, ForceMode.VelocityChange);
            }
            
            //Change rev sound pitch depending on the input while not boosting
            if (isAlive && !statusBar.isGameOver)
            {
                normalSpeedRevPitch = Mathf.Abs(_thrustAxis) * 1.2f + 1;
                engineRev.pitch += (normalSpeedRevPitch - engineRev.pitch) * Time.deltaTime;
            }

            //Get current rotation of turret and cannon
            _currentAngleY = _cannonTransform.localRotation.y * Mathf.Rad2Deg;
            _currentAngleZ = _headTransform.localRotation.z * Mathf.Rad2Deg;
        
            // Rotate turret depending on the input within a limit
            if ((_currentAngleZ < 50f && _viewRotation.x < 0) || (_currentAngleZ > -50f && _viewRotation.x > 0))
            {
                _headTransform.Rotate(Vector3.forward, currentMouseSensitivityX * -_viewRotation.x); //Rotate turret
            }
        
            if ((_currentAngleY < 12f && _viewRotation.y < 0) || (_currentAngleY > -17f && _viewRotation.y > 0))
            {
                _cannonTransform.Rotate(Vector3.up, currentMouseSensitivityY * -_viewRotation.y); //Rotate canon
            }
        }
        
    }
    
    
 /* The following methods are events called by the Input system */
 
 //Body Movement
    public void HandleRotation(InputAction.CallbackContext context)
    {
        _rotationAxis = context.ReadValue<float>();
    }

    public void HandleThrust(InputAction.CallbackContext context)
    {
        _thrustAxis = context.ReadValue<float>();
    }

    public void HandleBoost(InputAction.CallbackContext context)
    {

        if (context.started && vigor > 0) //Check if the player can boost
        {
            isBoosting = true;
            
            //Allow tank to go faster
            currentMovementSpeed = movementSpeed * 2;
            currentMaxVelocity = maxVelocity * 2;
            
            //Change color and size of vfx
            foreach (GameObject trail in smokeTrails)
            {
                ParticleSystem.MainModule pMain = trail.GetComponent<ParticleSystem>().main;
                pMain.startSizeMultiplier = 3;
                pMain.startColor = new Color(251, 221, 162);
            }
        }

        if (context.canceled)
        {
            isBoosting = false;
            
            //Reset speed
            currentMovementSpeed = movementSpeed;
            currentMaxVelocity = maxVelocity;
            
            //Reset particles
            foreach (GameObject trail in smokeTrails)
            {
                ParticleSystem.MainModule pMain = trail.GetComponent<ParticleSystem>().main;
                pMain.startSizeMultiplier = 1;
                pMain.startColor = new Color(208, 201, 208);

            }
        }
    }

    //View movement
    public void HandleView(InputAction.CallbackContext context)
    {
        _viewRotation = context.ReadValue<Vector2>();
    }

    //Shooting
    public void HandleShoot(InputAction.CallbackContext context)
    {
        if (context.started && canShoot && !(!isAlive || statusBar.isGameOver)) //Check if the player is alive, the timer is still counting and the cooldown has passed
        {
            //Spawn projectile
            GameObject clone = Instantiate(projectile, projectileOrigin.transform.position, projectileOrigin.transform.rotation);
            
            //Give the projectile velocity
            clone.GetComponent<Rigidbody>().AddRelativeForce(-projectilePower, 0, 0, ForceMode.Impulse);
            
            muzzleMainVFX.Play(); // play particles

            tankFire.Play(); //Play firing sound
            
            //Setup cooldown
            canShoot = false;
            shootTime = Time.time;
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
