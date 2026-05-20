using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class ForkliftController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 60f;

    [Header("Carriage Settings")]
    [SerializeField] private ConfigurableJoint carriageJoint;
    [SerializeField] private float liftSpeed = 0.5f;

    [Header("Mast Settings")]
    [SerializeField] private HingeJoint mastJoint;
    [SerializeField] private float tiltSpeed = 20f;
    [SerializeField] private float mastHoldForce = 5000f;

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
    private float _currentLiftHeight = 0f;


    private float _currentMastAngle = 0f;
    [SerializeField] private float mastMinAngle = -5f;
    [SerializeField] private float mastMaxAngle = 3f;


    private float liftMin = 0f;
    private float liftMax = 100f;
    private bool _engineRunning = false;

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
        HandleLift();
        HandleMastTilt();
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

    /*private void HandleFork()
    {
        if (forkAssembly == null || _forkRb == null) return;

        float forkInput = 0f;
        if (_controls.Forklift.ForkUp.IsPressed()) forkInput = 1f;
        if (_controls.Forklift.ForkDown.IsPressed()) forkInput = -1f;

        float currentMinY = _hasCargoAttached ? _forkMinYWithCargo : forkMinY;

        float localY = forkAssembly.localPosition.y;
        localY = Mathf.Clamp(
            localY + forkInput * forkSpeed * Time.fixedDeltaTime,
            currentMinY,
            forkMaxY
        );

        Vector3 worldTarget = transform.TransformPoint(
            new Vector3(forkAssembly.localPosition.x, localY, forkAssembly.localPosition.z)
        );
        _forkRb.MovePosition(worldTarget);
    }*/

    private void HandleLift()
    {
        if (carriageJoint == null) return;

        float liftInput = 0f;
        if (_controls.Forklift.ForkUp.IsPressed()) liftInput = 1f;
        if (_controls.Forklift.ForkDown.IsPressed()) liftInput = -1f;

        // Накапливаем целевую высоту

        _currentLiftHeight = Mathf.Clamp(
            _currentLiftHeight + liftInput * liftSpeed * Time.fixedDeltaTime,
            liftMin,
            liftMax
        );

        // Говорим джойнту куда тянуть каретку
        carriageJoint.targetPosition = new Vector3(0f, -_currentLiftHeight, 0f);

        //Debug.Log($"target={_currentLiftHeight}, carriageY={carriageJoint.transform.localPosition.y}");
    }

    private void HandleMastTilt()
    {
        if (mastJoint == null) return;

        float tiltInput = 0f;
        if (Keyboard.current.zKey.isPressed) tiltInput = 1f;
        if (Keyboard.current.xKey.isPressed) tiltInput = -1f;

        float minAngle = mastJoint.limits.min;
        float maxAngle = mastJoint.limits.max;

        _currentMastAngle = Mathf.Clamp(
            _currentMastAngle + tiltInput * tiltSpeed * Time.fixedDeltaTime,
            minAngle,
            maxAngle
        );

        JointSpring spring = mastJoint.spring;
        spring.targetPosition = _currentMastAngle;
        mastJoint.spring = spring;

        //Debug.Log($"target={_currentMastAngle}, mastZ={mastJoint.transform.localPosition.z}");
    }

}