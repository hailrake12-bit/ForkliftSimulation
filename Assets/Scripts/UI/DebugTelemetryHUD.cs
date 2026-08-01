using UnityEngine;

public class DebugTelemetryHUD : MonoBehaviour
{
    public static DebugTelemetryHUD Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Rigidbody forkliftRb;
    [SerializeField] private Rigidbody cargoRb;

    [Header("Logging")]
    [SerializeField] private float logInterval = 0.25f;
    [SerializeField] private float slipLogThreshold = 0.03f; 
    [SerializeField] private bool enableConsoleLog = true;

    [SerializeField] private WheelCollider steerWheelRef; 
    [SerializeField] private ForkliftMovement forkliftController; 

    private Vector3 _lastVelocity;
    private float _currentAccel;
    private float _lastForwardSpeed;
    private float _logTimer;
    private bool _wasSlipping;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _lastForwardSpeed = Vector3.Dot(forkliftRb.linearVelocity, forkliftRb.transform.forward);
    }

    private void FixedUpdate()
    {
        float forwardSpeed = Vector3.Dot(forkliftRb.linearVelocity, forkliftRb.transform.forward);
        _currentAccel = (forwardSpeed - _lastForwardSpeed) / Time.fixedDeltaTime;
        _lastForwardSpeed = forwardSpeed;

        if (enableConsoleLog)
            HandleLogging();
    }

    private void HandleLogging()
    {
        float speedKmh = Mathf.Abs(_lastForwardSpeed) * 3.6f;
        float gForce = _currentAccel / 9.81f;
        float slipZ = 0f;
        bool cargoPresent = cargoRb != null;

        if (cargoPresent)
        {
            Vector3 localSlip = forkliftRb.transform.InverseTransformDirection(cargoRb.linearVelocity - forkliftRb.linearVelocity);
            slipZ = localSlip.z;
        }

        _logTimer += Time.fixedDeltaTime;
        if (_logTimer >= logInterval)
        {
            _logTimer = 0f;

            string steerInfo = "";
            if (steerWheelRef != null)
            {
                float inputX = forkliftController != null ? forkliftController.CurrentMoveInput.x : float.NaN;
                float yawRate = forkliftRb.angularVelocity.y * Mathf.Rad2Deg;
                steerInfo = $",steerInput={inputX:F2},steerAngleActual={steerWheelRef.steerAngle:F2},yawRate={yawRate:F1}deg/s";
            }

            Debug.Log($"TELEMETRY,{Time.time:F2},{speedKmh:F1},{_currentAccel:F2},{gForce:F2},{(cargoPresent ? slipZ.ToString("F3") : "N/A")}" + steerInfo);
        }

        bool isSlipping = cargoPresent && Mathf.Abs(slipZ) > slipLogThreshold;
        if (isSlipping && !_wasSlipping)
        {
            Debug.Log($"SLIP_START,{Time.time:F2},speed={speedKmh:F1}km/h,accel={_currentAccel:F2}m/s2,slipZ={slipZ:F3}m/s");
        }
        else if (!isSlipping && _wasSlipping)
        {
            Debug.Log($"SLIP_END,{Time.time:F2},speed={speedKmh:F1}km/h");
        }
        _wasSlipping = isSlipping;
    }

    public void RegisterCargo(Rigidbody newCargo)
    {
        cargoRb = newCargo;
        if (enableConsoleLog)
            Debug.Log($"CARGO_REGISTERED,{Time.time:F2},name={newCargo.name}");
    }

    public void UnregisterCargo(Rigidbody cargoToRemove)
    {
        if (cargoRb == cargoToRemove)
        {
            cargoRb = null;
            if (enableConsoleLog)
                Debug.Log($"CARGO_UNREGISTERED,{Time.time:F2}");
        }
    }

    private void OnGUI()
    {
        float speedKmh = Mathf.Abs(_lastForwardSpeed) * 3.6f;
        float gForce = _currentAccel / 9.81f;

        float width = 400f;
        float lineHeight = 20f;
        float margin = 10f;
        int lines = 2 + (steerWheelRef != null ? 1 : 0) + (cargoRb != null ? 1 : 0);
        float startY = Screen.height - margin - lines * lineHeight;
        float x = Screen.width - width - margin;
        int line = 0;

        GUI.Label(new Rect(x, startY + line++ * lineHeight, width, lineHeight), $"Скорость: {speedKmh:F1} км/ч");
        GUI.Label(new Rect(x, startY + line++ * lineHeight, width, lineHeight), $"Ускорение: {_currentAccel:F2} м/с² ({gForce:F2} g)");

        if (steerWheelRef != null)
        {
            float inputX = forkliftController != null ? forkliftController.CurrentMoveInput.x : float.NaN;
            GUI.Label(new Rect(x, startY + line++ * lineHeight, width, lineHeight), $"Steer input: {inputX:F2}  |  Реальный угол: {steerWheelRef.steerAngle:F2}");
        }

        if (cargoRb != null)
        {
            Vector3 localSlip = forkliftRb.transform.InverseTransformDirection(cargoRb.linearVelocity - forkliftRb.linearVelocity);
            GUI.Label(new Rect(x, startY + line++ * lineHeight, width, lineHeight), $"Груз скользит по Z: {localSlip.z:F2} м/с");
        }
    }
}