using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class SC_PlayerController : MonoBehaviour
{
    //Declaring body Movement Variables
    public float movementSpeed;

    public float rotationSpeed;

    private float _rotationAxis;
    
    private float _thrustAxis;
    
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
    
    //Declaring Camera movement variables

    [SerializeField] private float limitAngleMin;

    [SerializeField] private float limitAngleMax;

    public float mouseSensitivityX;

    public float mouseSensitivityY;
    
    private Vector2 _viewRotation;
    
    private float _targetRotationY;

    private float _currentAngleY;

    private float _currentAngleZ;

    private bool _isFront = true;
    
    //Declaring variables related to projectile handling

    public float projectilePower;

    public float health;
    
    //Declaring Tank variables

    
    
    // Start is called before the first frame update
    void Start()
    {
        //Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _headTransform = headController.transform;
        _cannonTransform = cannonController.transform;
        _cannonEndTransform = cannonEnd.transform;

        tpsCam.enabled = true;
        fpsCam.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        //Apply calculated Movement to body from handlers
        transform.Rotate(new Vector3(0,1,0), rotationSpeed * _rotationAxis);
        Vector3 forwardMovement = new Vector3(0, 0, _thrustAxis);
        transform.Translate(movementSpeed * Time.deltaTime * forwardMovement);
        
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
            GameObject clone = Instantiate(projectile, _cannonEndTransform.position, _cannonEndTransform.rotation);
            Quaternion cloneRotation = clone.transform.rotation;
            clone.GetComponent<Rigidbody>().AddRelativeForce(-projectilePower, 0, 0, ForceMode.Impulse);
            clone.GetComponent<SC_DefaultProjectile>().damage = 100f;
            clone.GetComponent<SC_DefaultProjectile>().isStunning = false;
        }
    }

    
    public void HandleShootSecondary(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameObject clone = Instantiate(projectile, _cannonEndTransform.position, _cannonEndTransform.rotation);
            Quaternion cloneRotation = clone.transform.rotation;
            clone.GetComponent<Rigidbody>().AddRelativeForce(-projectilePower, 0, 0, ForceMode.Impulse);
            clone.GetComponent<SC_DefaultProjectile>().damage = 0f;
            clone.GetComponent<SC_DefaultProjectile>().isStunning = true;
        }
    }

    //Aiming
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
