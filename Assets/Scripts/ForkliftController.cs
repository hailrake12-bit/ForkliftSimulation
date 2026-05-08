using UnityEngine;
using UnityEngine.InputSystem;

public class ForkliftController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 60f;

    [Header("Fork Settings")]
    [SerializeField] private Transform forkAssembly;
    [SerializeField] private float forkSpeed = 1f;
    [SerializeField] private float forkMinY = 0.1f;
    [SerializeField] private float forkMaxY = 2f;

    private Rigidbody _rb;
    private ForkliftControls _controls;
    private bool _engineRunning = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _controls = new ForkliftControls();
    }

    private void OnEnable()
    {
        _controls.Enable();
        _controls.Forklift.EngineStart.performed += OnEngineStart;
    }

    private void OnDisable()
    {
        _controls.Disable();
        _controls.Forklift.EngineStart.performed -= OnEngineStart;
    }

    private void OnEngineStart(InputAction.CallbackContext context)
    {
        _engineRunning = !_engineRunning;
        Debug.Log(_engineRunning ? "Двигатель запущен" : "Двигатель выключен");
    }

    private void FixedUpdate()
    {
        if (!_engineRunning) return;

        HandleMovement();
        HandleFork();
    }

    private void HandleMovement()
    {
        Vector2 input = _controls.Forklift.Move.ReadValue<Vector2>();

        Vector3 moveDirection = transform.forward * input.y * moveSpeed;
        _rb.linearVelocity = new Vector3(moveDirection.x, _rb.linearVelocity.y, moveDirection.z);

        float turn = input.x * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        _rb.MoveRotation(_rb.rotation * turnRotation);
    }

    private void HandleFork()
    {
        if (forkAssembly == null) return;

        float forkInput = 0f;

        if (_controls.Forklift.ForkUp.IsPressed()) forkInput = 1f;
        if (_controls.Forklift.ForkDown.IsPressed()) forkInput = -1f;

        Vector3 pos = forkAssembly.localPosition;
        pos.y = Mathf.Clamp(pos.y + forkInput * forkSpeed * Time.fixedDeltaTime, forkMinY, forkMaxY);
        forkAssembly.localPosition = pos;
    }
}