using UnityEngine;
using System.Collections;

public class CargoAnimator : MonoBehaviour
{
    [Header("Spawn Animation")]
    [SerializeField] private float spawnHeight = 10f;
    [SerializeField] private float spawnDuration = 5f;
    [SerializeField] private float rotationSpeed = 90f;

    private Vector3 _targetPosition;
    private Vector3 _startPosition;
    private float _elapsedTime = 0f;
    private bool _isAnimating = true;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _targetPosition = transform.position;
        _startPosition = _targetPosition + Vector3.up * spawnHeight;
        transform.position = _startPosition;
        _rb.isKinematic = true;
    }

    private void Update()
    {
        if (!_isAnimating) return;

        _elapsedTime += Time.deltaTime;
        float progress = _elapsedTime / spawnDuration;

        transform.position = Vector3.Lerp(_startPosition, _targetPosition, progress);
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

        if (progress >= 1f)
        {
            transform.position = _targetPosition;
            _isAnimating = false;
            _rb.isKinematic = false;
        }
    }

    public void StartLaunchAnimation()
    {
        StartCoroutine(LaunchCoroutine());
    }

    private IEnumerator LaunchCoroutine()
    {
        float elapsed = 0f;
        _rb.isKinematic = true;

        while (elapsed < spawnDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / spawnDuration;

            float upSpeed = Mathf.Lerp(0f, 20f, progress);
            transform.position += Vector3.up * upSpeed * Time.deltaTime;

            transform.Rotate(
                rotationSpeed * Time.deltaTime,
                rotationSpeed * Time.deltaTime,
                rotationSpeed * Time.deltaTime
            );

            yield return null;
        }

        FindFirstObjectByType<GameManager>().SpawnCargo();
        Destroy(gameObject);
    }
}