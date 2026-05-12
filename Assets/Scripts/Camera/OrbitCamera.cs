using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance = 8f;
    [SerializeField] private float sensitivity = 3f;

    private float initialPitch = 20f;
    private float initialYaw = 0f;
    private float minPitch = 10f;
    private float maxPitch = 80f;
    private float _yaw;
    private float _pitch;

    private void Start()
    {
        _pitch = initialPitch;
        _yaw = initialYaw;
    }

    private void LateUpdate()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            _yaw += delta.x * sensitivity * Time.deltaTime;
            _pitch -= delta.y * sensitivity * Time.deltaTime;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        }

        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        transform.position = target.position + rotation * Vector3.back * distance;
        transform.LookAt(target.position);
    }
}