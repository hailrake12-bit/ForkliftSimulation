using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance = 8f;
    [SerializeField] private float sensitivity = 3f;

    private float _initialPitch = 20f;
    private float _initialYaw = 0f;
    private float _minPitch = 10f;
    private float _maxPitch = 80f;
    private float _yaw;
    private float _pitch;
    private ForkliftControls _controls;
    private Vector2 _lookInput;
    private bool _isDragging;

    private void Awake()
    {
        _controls = new ForkliftControls();
    }

    private void OnEnable()
    {
        _controls.Forklift.ThrdpvLookMove.performed += OnLook;
        _controls.Forklift.ThrdpvLookMove.canceled += OnLook;
        _controls.Forklift.ThrdpvLookActivate.performed += OnDragStart;
        _controls.Forklift.ThrdpvLookActivate.canceled += OnDragEnd;
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Forklift.ThrdpvLookMove.performed -= OnLook;
        _controls.Forklift.ThrdpvLookMove.canceled -= OnLook;
        _controls.Forklift.ThrdpvLookActivate.performed -= OnDragStart;
        _controls.Forklift.ThrdpvLookActivate.canceled -= OnDragEnd;
        _controls.Disable();
    }

    private void Start()
    {
        _pitch = _initialPitch;
        _yaw = _initialYaw;
    }

    private void LateUpdate()
    {
        if (_isDragging && _lookInput != Vector2.zero) 
        {
            _yaw += _lookInput.x * sensitivity * Time.deltaTime;
            _pitch -= _lookInput.y * sensitivity * Time.deltaTime;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            transform.position = target.position + rotation * Vector3.back * distance;
        }
        transform.LookAt(target.position);

    }

    private void OnLook(InputAction.CallbackContext context)
    {
         _lookInput = context.ReadValue<Vector2>();
    }

    private void OnDragStart(InputAction.CallbackContext context)
    {
        _isDragging = true;
    }

    private void OnDragEnd(InputAction.CallbackContext context)
    {
        _isDragging = false;
    }
}