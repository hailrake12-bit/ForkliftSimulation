using System;
using UniRx;
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

    [Header("Fuel Settings")]
    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float fuelConsumptionRate = 5f;
    [SerializeField] private float lowFuelThreshold = 50f;
    [SerializeField] private float lowFuelSpeedMultiplier = 0.5f;

    public float FuelPercentage => _currentFuel.Value / maxFuel * 100f;
    public IObservable<float> FuelStream => _currentFuel.AsObservable();
    public float MaxFuel => maxFuel;

    private Rigidbody _rb;
    private ForkliftControls _controls;
    private ReactiveProperty<float> _currentFuel = new ReactiveProperty<float>();
    private float _forkMinYWithCargo = 0.219f;
    private bool _engineRunning = false;
    private bool _hasCargoAttached = false;

    private void Start()
    {
        _currentFuel.Value = maxFuel;
    }

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
        if (_currentFuel.Value <= 0f)
        {
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
            return;
        }
        ConsumeFuel();
        HandleMovement();
        HandleFork();
    }

    private void HandleMovement()
    {
        Vector2 input = _controls.Forklift.Move.ReadValue<Vector2>();

        float fuelPercent = _currentFuel.Value / maxFuel * 100f;
        float speedMultiplier = fuelPercent < lowFuelThreshold ? lowFuelSpeedMultiplier : 1f;

        Vector3 moveDirection = transform.forward * input.y * moveSpeed * speedMultiplier;
        _rb.linearVelocity = new Vector3(moveDirection.x, _rb.linearVelocity.y, moveDirection.z);

        float turn = input.x * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        _rb.MoveRotation(_rb.rotation * turnRotation);
    }

    private void ConsumeFuel()
    {
        Vector2 input = _controls.Forklift.Move.ReadValue<Vector2>();
        bool isMoving = input.magnitude > 0.1f;
        float consumption = isMoving ? fuelConsumptionRate * 3f : fuelConsumptionRate;
        _currentFuel.Value = Mathf.Max(0f, _currentFuel.Value - consumption * Time.fixedDeltaTime);
    }

    private void HandleFork()
    {
        if (forkAssembly == null) return;

        float forkInput = 0f;
        if (_controls.Forklift.ForkUp.IsPressed()) forkInput = 1f;
        if (_controls.Forklift.ForkDown.IsPressed()) forkInput = -1f;

        float currentMinY = _hasCargoAttached ? _forkMinYWithCargo : forkMinY;

        Vector3 pos = forkAssembly.localPosition;
        pos.y = Mathf.Clamp(pos.y + forkInput * forkSpeed * Time.fixedDeltaTime, currentMinY, forkMaxY);
        forkAssembly.localPosition = pos;
    }

    public void SetCargoAttached(bool value)
    {
        _hasCargoAttached = value;
    }
}