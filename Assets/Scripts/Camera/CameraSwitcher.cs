using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera debugCamera;

    private ForkliftControls _controls;
    private bool _isMainCamera = true;

    private void Awake()
    {
        _controls = new ForkliftControls();
    }

    private void OnEnable()
    {
        _controls.Forklift.SwitchCamera.performed += OnSwitchCamera;
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Forklift.SwitchCamera.performed -= OnSwitchCamera;
        _controls.Enable();
    }

    private void OnSwitchCamera(InputAction.CallbackContext context)
    {
        Debug.Log("SwitchCamera triggered");
        _isMainCamera = !_isMainCamera;
        mainCamera.enabled = _isMainCamera;
        debugCamera.enabled = !_isMainCamera;
    }
}