using UnityEngine;
using UnityEngine.AddressableAssets;
using DG.Tweening;
using Zenject;

public class CargoAnimator : MonoBehaviour
{
    [Header("Spawn Animation")]
    [SerializeField] private float spawnHeight = 10f;
    [SerializeField] private float spawnDuration = 5f;

    [Header("Launch Animation")]
    [SerializeField] private float launchHeight = 20f;
    [SerializeField] private float launchDuration = 5f;

    [Inject] private CargoFactory _cargoFactory;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;

        Vector3 targetPosition = transform.position;
        transform.position = targetPosition + Vector3.up * spawnHeight;

        transform.DOMove(targetPosition, spawnDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => _rb.isKinematic = false);

        transform.DORotate(new Vector3(0, 360, 0), spawnDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);
    }

    public void StartLaunchAnimation()
    {
        _rb.isKinematic = true;

        transform.DOMove(transform.position + Vector3.up * launchHeight, launchDuration)
            .SetEase(Ease.InExpo);

        transform.DORotate(new Vector3(360, 360, 360), launchDuration * 0.7f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetDelay(launchDuration * 0.3f)
            .OnComplete(async () =>
            {
                await _cargoFactory.SpawnCargo();
                Destroy(gameObject);
            });
    }
}