using System;
using UnityEngine;
using Zenject;

public class ForkliftMovement : MonoBehaviour
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
    [SerializeField] private float motorRampSpeed = 3000f;
    [SerializeField] private float brakeRampSpeed = 6000f;
    [SerializeField] private float steerRampSpeed = 90f;
    [Inject] private ForkliftInputHandler _inputHandler;
    [Inject] private ForkliftEngineFuelSystem _fuelSystem;

    public float MaxFuel => _fuelSystem.MaxFuel;
    public float FuelPercentage => _fuelSystem.FuelPercentage;
    public IObservable<float> FuelStream => _fuelSystem.FuelStream;
    public float CurrentSpeedKmh => Mathf.Abs(Vector3.Dot(_rb.linearVelocity, transform.forward)) * 3.6f;

    private Rigidbody _rb;
    public Vector2 CurrentMoveInput => _inputHandler.MoveInput;
    private float _currentMotor;
    private float _currentBrake;
    private float _currentSteerAngle;
    private float _currentLiftHeight = 0.2f;
    private float _currentMastAngle = 0f;
    private Vector3 _wheelVisualRotationOffset = new Vector3(0f, 0f, 90f);


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        carriageJoint.targetPosition = new Vector3(0f, -liftMin, 0f);
    }
    private void FixedUpdate()
    {
        if (!_fuelSystem.IsRunning) return;

        if (_fuelSystem.IsOutOfFuel)
        {
            _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, 0f);
            return;
        }

        HandleMovement();
        HandleLift();
        HandleMastTilt();
    }
    private void HandleMovement()
    {
        var moveInput = _inputHandler.MoveInput;
        bool brakeHeld = _inputHandler.BrakeHeld;

        float targetMotor = brakeHeld ? 0f : moveInput.y * maxMotorTorque * _fuelSystem.SpeedMultiplier;
        _currentMotor = Mathf.MoveTowards(_currentMotor, targetMotor, motorRampSpeed * Time.fixedDeltaTime);
        wheelFL.motorTorque = _currentMotor;
        wheelFR.motorTorque = _currentMotor;

        float targetSteer = moveInput.x * maxSteerAngle;
        _currentSteerAngle = Mathf.MoveTowards(_currentSteerAngle, targetSteer, steerRampSpeed * Time.fixedDeltaTime);
        wheelRL.steerAngle = -_currentSteerAngle;
        wheelRR.steerAngle = -_currentSteerAngle;

        float forwardSpeedMs = Vector3.Dot(_rb.linearVelocity, transform.forward);
        float speedKmh = Mathf.Abs(forwardSpeedMs) * 3.6f;
        float brakeSoftness = Mathf.Clamp01(speedKmh / 3f);
        float targetBrake = brakeHeld ? maxBrakeTorque * brakeSoftness : 0f;
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
            _currentLiftHeight + _inputHandler.LiftInput * liftSpeed * Time.fixedDeltaTime,
            liftMin, liftMax);

        carriageJoint.targetPosition = new Vector3(0f, -_currentLiftHeight, 0f);
    }

    private void HandleMastTilt()
    {
        _currentMastAngle = Mathf.Clamp(
            _currentMastAngle + _inputHandler.MastInput * tiltSpeed * Time.fixedDeltaTime,
            mastMinAngle, mastMaxAngle);

        JointSpring spring = mastJoint.spring;
        spring.targetPosition = _currentMastAngle;
        mastJoint.spring = spring;
    }
}