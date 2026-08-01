using UnityEngine;

public class SpeedometerNeedle : MonoBehaviour
{
    [SerializeField] private ForkliftMovement forkliftMovement;
    [SerializeField] private Transform needlePivot;
    [SerializeField] private Transform gaugeFace;

    [Header("Шкала")]
    [SerializeField] private float maxSpeedKmh = 20f;
    [SerializeField] private float minAngle = 120f;
    [SerializeField] private float maxAngle = -120f;

    [Header("Плавность")]
    [SerializeField] private float smoothSpeed = 8f;

    private float _currentAngle;
    private Quaternion _restLocalRotation;
    private Vector3 _localSpinAxis;

    private void Start()
    {
        _restLocalRotation = needlePivot.localRotation;

        Transform parent = needlePivot.parent;
        _localSpinAxis = parent != null
            ? parent.InverseTransformDirection(gaugeFace.forward)
            : gaugeFace.forward;
    }

    private void Update()
    {
        if (forkliftMovement == null || needlePivot == null || gaugeFace == null) return;

        float speedKmh = Mathf.Clamp(forkliftMovement.CurrentSpeedKmh, 0f, maxSpeedKmh);
        float t = speedKmh / maxSpeedKmh;
        float targetAngle = Mathf.Lerp(minAngle, maxAngle, t);

        _currentAngle = Mathf.LerpAngle(_currentAngle, targetAngle, Time.deltaTime * smoothSpeed);
        needlePivot.localRotation = Quaternion.AngleAxis(_currentAngle, _localSpinAxis) * _restLocalRotation;
    }
}