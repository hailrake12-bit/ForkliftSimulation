using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class ForkliftInputHandler : IInitializable, IDisposable
{
    [Inject] private ForkliftInputs _controls;

    private readonly ReactiveProperty<Vector2> _moveInput = new ReactiveProperty<Vector2>(Vector2.zero);
    private readonly ReactiveProperty<float> _liftInput = new ReactiveProperty<float>(0f);
    private readonly ReactiveProperty<float> _mastInput = new ReactiveProperty<float>(0f);
    private readonly ReactiveProperty<bool> _brakeHeld = new ReactiveProperty<bool>(false);
    private readonly Subject<Unit> _engineStartPressed = new Subject<Unit>();
    public Vector2 MoveInput => _moveInput.Value;
    public float LiftInput => _liftInput.Value;
    public float MastInput => _mastInput.Value;
    public bool BrakeHeld => _brakeHeld.Value;

    public IObservable<Vector2> MoveStream => _moveInput.AsObservable();
    public IObservable<float> LiftStream => _liftInput.AsObservable();
    public IObservable<float> MastStream => _mastInput.AsObservable();
    public IObservable<bool> BrakeStream => _brakeHeld.AsObservable();
    public IObservable<Unit> EngineStartPressed => _engineStartPressed.AsObservable();

    public void Initialize()
    {
        Debug.Log("ForkliftInputHandler.Initialize called");
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

    public void Dispose()
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

    private void OnEngineStart(InputAction.CallbackContext ctx) => _engineStartPressed.OnNext(Unit.Default);
    private void OnMovePerformed(InputAction.CallbackContext ctx) => _moveInput.Value = ctx.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext ctx) => _moveInput.Value = Vector2.zero;
    private void OnLiftPerformed(InputAction.CallbackContext ctx) => _liftInput.Value = ctx.ReadValue<float>();
    private void OnLiftCanceled(InputAction.CallbackContext ctx) => _liftInput.Value = 0f;
    private void OnMastPerformed(InputAction.CallbackContext ctx) => _mastInput.Value = ctx.ReadValue<float>();
    private void OnMastCanceled(InputAction.CallbackContext ctx) => _mastInput.Value = 0f;
    private void OnBrakePerformed(InputAction.CallbackContext ctx) => _brakeHeld.Value = true;
    private void OnBrakeCanceled(InputAction.CallbackContext ctx) => _brakeHeld.Value = false;

}
