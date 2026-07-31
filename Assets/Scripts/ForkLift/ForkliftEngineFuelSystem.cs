using System;
using UniRx;
using UnityEngine;
using Zenject;

public class ForkliftEngineFuelSystem : MonoBehaviour
{
    [Header("Fuel Settings")]
    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float fuelConsumptionRate = 5f;
    [SerializeField] private float lowFuelThreshold = 50f;
    [SerializeField] private float lowFuelSpeedMultiplier = 0.5f;

    [Inject] private ForkliftInputHandler _inputHandler;

    private readonly ReactiveProperty<float> _currentFuel = new ReactiveProperty<float>();
    private readonly ReactiveProperty<bool> _engineRunning = new ReactiveProperty<bool>(false);

    public float MaxFuel => maxFuel;
    public float FuelPercentage => _currentFuel.Value / maxFuel * 100f;
    public bool IsOutOfFuel => _currentFuel.Value <= 0f;
    public float SpeedMultiplier => FuelPercentage < lowFuelThreshold ? lowFuelSpeedMultiplier : 1f;
    public IObservable<float> FuelStream => _currentFuel.AsObservable();

    public bool IsRunning => _engineRunning.Value;
    public IObservable<bool> EngineStateStream => _engineRunning.AsObservable();

    private void Awake() => _currentFuel.Value = maxFuel;

    private void OnEnable()
    {
        _inputHandler.EngineStartPressed
            .Subscribe(_ => _engineRunning.Value = !_engineRunning.Value)
            .AddTo(this);
    }

    private void FixedUpdate()
    {
        if (!_engineRunning.Value || IsOutOfFuel) return;

        bool isMoving = _inputHandler.MoveInput.magnitude > 0.1f;
        float consumption = isMoving ? fuelConsumptionRate * 3f : fuelConsumptionRate;
        _currentFuel.Value = Mathf.Max(0f, _currentFuel.Value - consumption * Time.fixedDeltaTime);
    }
}