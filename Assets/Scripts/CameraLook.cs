using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private float sensitivity = 2f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;
    [SerializeField] private float minYaw = -60f;
    [SerializeField] private float maxYaw = 60f;

    private float _pitch = 0f;
    private float _yaw = 0f;
    private ForkliftControls _controls;

    private void Awake()
    {
        _controls = new ForkliftControls();
    }

    private void OnEnable() => _controls.Enable();

    private void OnDisable() => _controls.Disable();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 look = _controls.Forklift.Look.ReadValue<Vector2>();

        _pitch -= look.y * sensitivity;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        _yaw += look.x * sensitivity;
        _yaw = Mathf.Clamp(_yaw, minYaw, maxYaw);

        transform.localRotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }
}