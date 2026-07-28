using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private float sensitivity = 2f;

    [Inject] private ForkliftInputs _controls;

    private float _minPitch = -75f;
    private float _maxPitch = 60f;
    private float _pitch = 0f;

    private float _minYaw = -60f;
    private float _maxYaw = 60f;
    private float _yaw = 0f;

    private Vector2 _lookInput;

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




    private void OnEnable()
    {
        _controls.Enable();

        _controls.Forklift.FpvLook.performed += OnLookPerformed;
        _controls.Forklift.FpvLook.canceled += OnLookCanceled;
    }
    private void OnDisable()
    {
        _controls.Forklift.FpvLook.performed -= OnLookPerformed;
        _controls.Forklift.FpvLook.canceled -= OnLookCanceled;

        _controls.Disable();
    }

    private void OnLookPerformed(InputAction.CallbackContext context) => _lookInput = context.ReadValue<Vector2>();

    private void OnLookCanceled(InputAction.CallbackContext context) => _lookInput = Vector2.zero;
}