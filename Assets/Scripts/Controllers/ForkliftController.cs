using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class ForkliftController : MonoBehaviour
{
    [Header("Carriage Settings")]
    [SerializeField] private ConfigurableJoint carriageJoint;
    [SerializeField] private float liftSpeed = 0.5f;
    [SerializeField] private float liftMin = 0.2f;
    [SerializeField] private float liftMax = 2f;
    [Header("Mast Settings")]
    [SerializeField] private HingeJoint mastJoint;
    [SerializeField] private float tiltSpeed = 20f;
    [SerializeField] private float mastMinAngle = -5f;
    [SerializeField] private float mastMaxAngle = 3f;
    [Header("Fuel Settings")]
    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float fuelConsumptionRate = 5f;
    [SerializeField] private float lowFuelThreshold = 50f;
    [SerializeField] private float lowFuelSpeedMultiplier = 0.5f;
    [Header("Wheels")]
    [SerializeField] private WheelCollider wheelFL;
    [SerializeField] private WheelCollider wheelFR;
    [SerializeField] private WheelCollider wheelRL;
    [SerializeField] private WheelCollider wheelRR;
    [Header("Wheel Visuals")]
    [SerializeField] private Transform wheelFLVisual;
    [SerializeField] private Transform wheelFRVisual;
    [SerializeField] private Transform wheelRLVisual;
    [SerializeField] private Transform wheelRRVisual;
    [Header("Movement (Wheel-based)")]
    [SerializeField] private float maxMotorTorque = 2000f;
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private float maxBrakeTorque = 3000f;
    [SerializeField] private float brakeRampSpeed = 6000f;
    [SerializeField] private float motorRampSpeed = 3000f;
    [SerializeField] private float steerRampSpeed = 90f; // градусов в секунду — плавность поворота руля
    [Inject] private ForkliftInputs _controls;

    public float MaxFuel => maxFuel;
    private ReactiveProperty<float> _currentFuel = new ReactiveProperty<float>();
    public float FuelPercentage => _currentFuel.Value / maxFuel * 100f;
    public IObservable<float> FuelStream => _currentFuel.AsObservable();

    private Rigidbody _rb;
    private bool _engineRunning = false;
    private Vector2 _moveInput;
    private float _liftInput;
    private float _mastInput;
    private bool _brakeHeld;
    private float _currentLiftHeight = 0.2f;
    private float _currentMastAngle = 0f;
    private float _currentSteerAngle;
    private float _currentBrake;
    private float _currentMotor;
    public Vector2 CurrentMoveInput => _moveInput;

    private Vector3 _wheelVisualRotationOffset = new Vector3(0f, 0f, 90f);



    private void Awake()
    {
        _currentFuel.Value = maxFuel;
        _rb = GetComponent<Rigidbody>();
        carriageJoint.targetPosition = new Vector3(0f, -liftMin, 0f);
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
    private void ConsumeFuel()
    {
        bool isMoving = _moveInput.magnitude > 0.1f;
        float consumption = isMoving ? fuelConsumptionRate * 3f : fuelConsumptionRate;
        _currentFuel.Value = Mathf.Max(0f, _currentFuel.Value - consumption * Time.fixedDeltaTime);
    }
    private void HandleMovement()
    {
        float fuelPercent = _currentFuel.Value / maxFuel * 100f;
        float speedMultiplier = fuelPercent < lowFuelThreshold ? lowFuelSpeedMultiplier : 1f;

        float targetMotor = _brakeHeld ? 0f : _moveInput.y * maxMotorTorque * speedMultiplier;
        _currentMotor = Mathf.MoveTowards(_currentMotor, targetMotor, motorRampSpeed * Time.fixedDeltaTime);
        float steer = _moveInput.x * maxSteerAngle;

        wheelRL.steerAngle = -steer;
        wheelRR.steerAngle = -steer;

        wheelRL.steerAngle = -steer;
        wheelRR.steerAngle = -steer;

        wheelFL.motorTorque = _currentMotor;
        wheelFR.motorTorque = _currentMotor;

        bool braking = _brakeHeld;
        float forwardSpeedMs = Vector3.Dot(_rb.linearVelocity, transform.forward);
        float speedKmh = Mathf.Abs(forwardSpeedMs) * 3.6f;
        float brakeSoftness = Mathf.Clamp01(speedKmh / 3f); 
        float targetBrake = braking ? maxBrakeTorque * brakeSoftness : 0f;
        _currentBrake = Mathf.MoveTowards(_currentBrake, targetBrake, brakeRampSpeed * Time.fixedDeltaTime);
        wheelFL.brakeTorque = _currentBrake;
        wheelFR.brakeTorque = _currentBrake;
        wheelRL.brakeTorque = _currentBrake;
        wheelRR.brakeTorque = _currentBrake;

        UpdateWheelVisual(wheelFL, wheelFLVisual);
        UpdateWheelVisual(wheelFR, wheelFRVisual);
        UpdateWheelVisual(wheelRL, wheelRLVisual);
        UpdateWheelVisual(wheelRR, wheelRRVisual);
    }

    private void UpdateWheelVisual(WheelCollider collider, Transform visual)
    {
        collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        visual.position = pos;
        visual.rotation = rot * Quaternion.Euler(_wheelVisualRotationOffset);
    }
    private void HandleLift()
    {
        _currentLiftHeight = Mathf.Clamp(
            _currentLiftHeight + _liftInput * liftSpeed * Time.fixedDeltaTime,
            liftMin,
            liftMax
        );

        carriageJoint.targetPosition = new Vector3(0f, -_currentLiftHeight, 0f);
    }
    private void HandleMastTilt()
    {
        _currentMastAngle = Mathf.Clamp(
            _currentMastAngle + _mastInput * tiltSpeed * Time.fixedDeltaTime,
            mastMinAngle,
            mastMaxAngle
        );

        JointSpring spring = mastJoint.spring;
        spring.targetPosition = _currentMastAngle;
        mastJoint.spring = spring;
    }



    private void OnEnable()
    {
        _controls.Enable();

        _controls.Forklift.EngineStart.performed += OnEngineStart;

        _controls.Forklift.Move.performed += OnMovePerformed;
        _controls.Forklift.Move.canceled += OnMoveCanceled;

        _controls.Forklift.MoveFork.performed += OnLiftPerformed;
        _controls.Forklift.MoveFork.canceled += OnLiftCanceled;

        _controls.Forklift.MoveMast.performed += OnMastPerformed;
        _controls.Forklift.MoveMast.canceled += OnMastCanceled;

        _controls.Forklift.Brake.performed += OnBrakePerformed;
        _controls.Forklift.Brake.canceled += OnBrakeCanceled;
    }

    private void OnDisable()
    {
        _controls.Forklift.EngineStart.performed -= OnEngineStart;

        _controls.Forklift.Move.performed -= OnMovePerformed;
        _controls.Forklift.Move.canceled -= OnMoveCanceled;

        _controls.Forklift.MoveFork.performed -= OnLiftPerformed;
        _controls.Forklift.MoveFork.canceled -= OnLiftCanceled;

        _controls.Forklift.MoveMast.performed -= OnMastPerformed;
        _controls.Forklift.MoveMast.canceled -= OnMastCanceled;

        _controls.Forklift.Brake.performed -= OnBrakePerformed;
        _controls.Forklift.Brake.canceled -= OnBrakeCanceled;

        _controls.Disable();
    }

    private void OnEngineStart(InputAction.CallbackContext context) => _engineRunning = !_engineRunning;
    private void OnMovePerformed(InputAction.CallbackContext context) => _moveInput = context.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext context) => _moveInput = Vector2.zero;
    private void OnLiftPerformed(InputAction.CallbackContext context) => _liftInput = context.ReadValue<float>();
    private void OnLiftCanceled(InputAction.CallbackContext context) => _liftInput = 0f;
    private void OnMastPerformed(InputAction.CallbackContext context) => _mastInput = context.ReadValue<float>();
    private void OnMastCanceled(InputAction.CallbackContext context) => _mastInput = 0f;
    private void OnBrakePerformed(InputAction.CallbackContext context) => _brakeHeld = true;
    private void OnBrakeCanceled(InputAction.CallbackContext context) => _brakeHeld = false;
}