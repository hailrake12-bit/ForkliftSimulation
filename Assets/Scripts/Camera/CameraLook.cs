using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private float sensitivity = 2f;

    private float _minPitch = -75f;
    private float _maxPitch = 60f;
    private float _minYaw = -60f;
    private float _maxYaw = 60f;
    private float _pitch = 0f;
    private float _yaw = 0f;
    private ForkliftControls _controls;
    private Vector2 _lookInput;

    private void Awake()
    {
        _controls = new ForkliftControls();
    }

    private void OnEnable()
    {
        _controls.Forklift.FpvLook.performed += OnLook;
        _controls.Forklift.FpvLook.canceled += OnLook;
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Forklift.FpvLook.performed -= OnLook;
        _controls.Forklift.FpvLook.canceled -= OnLook;
        _controls.Disable();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (_lookInput == Vector2.zero) return;

        _pitch -= _lookInput.y * sensitivity;
        _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

        _yaw += _lookInput.x * sensitivity;
        _yaw = Mathf.Clamp(_yaw, _minYaw, _maxYaw);

        transform.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }
}