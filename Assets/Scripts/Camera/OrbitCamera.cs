using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;  

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance = 8f;
    [SerializeField] private float sensitivity = 3f;

    [Inject] private ForkliftInputs _controls;

    private float _initialPitch = 20f;
    private float _minPitch = 10f;
    private float _maxPitch = 80f;
    private float _pitch;

    private float _initialYaw = 0f;
    private float _yaw;

    private Vector2 _lookInput;
    private bool _isDragging;

    private void Start()
    {
        _pitch = _initialPitch;
        _yaw = _initialYaw;

        UpdatePosition();
    }
    private void LateUpdate()
    {
        if (_isDragging && _lookInput != Vector2.zero) 
        {
            _yaw += _lookInput.x * sensitivity;
            _pitch -= _lookInput.y * sensitivity;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        transform.position = target.position + rotation * Vector3.back * distance;
        transform.LookAt(target.position);
    }

    private void OnEnable()
    {
        _controls.Enable();

        _controls.Forklift.ThrdpvLookMove.performed += OnLook;
        _controls.Forklift.ThrdpvLookMove.canceled += OnLook;
        _controls.Forklift.ThrdpvLookActivate.performed += OnDragStart;
        _controls.Forklift.ThrdpvLookActivate.canceled += OnDragEnd;
    }

    private void OnDisable()
    {
        _controls.Forklift.ThrdpvLookMove.performed -= OnLook;
        _controls.Forklift.ThrdpvLookMove.canceled -= OnLook;
        _controls.Forklift.ThrdpvLookActivate.performed -= OnDragStart;
        _controls.Forklift.ThrdpvLookActivate.canceled -= OnDragEnd;

        _controls.Disable();
    }
    private void OnLook(InputAction.CallbackContext context) => _lookInput = context.ReadValue<Vector2>();
    private void OnDragStart(InputAction.CallbackContext context) => _isDragging = true;
    private void OnDragEnd(InputAction.CallbackContext context) => _isDragging = false;
}