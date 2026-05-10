using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject debugCamera;

    private bool _isMainCamera = true;

    private void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            _isMainCamera = !_isMainCamera;
            mainCamera.SetActive(_isMainCamera);
            debugCamera.SetActive(!_isMainCamera);
        }
    }
}