using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera debugCamera;

    [Inject] private ForkliftInputs _controls;

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
        bool _isMainCamera = mainCamera.enabled;
        mainCamera.enabled = !_isMainCamera;
        debugCamera.enabled = _isMainCamera;
    }
}