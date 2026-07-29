using UnityEngine;
using DG.Tweening;
using Zenject;

public class CargoAnimator : MonoBehaviour, IInitializable
{
    [Header("Spawn Animation")]
    [SerializeField] private float spawnHeight = 10f;
    [SerializeField] private float spawnDuration = 5f;

    [Header("Launch Animation")]
    [SerializeField] private float launchHeight = 20f;
    [SerializeField] private float launchDuration = 5f;

    [SerializeField] private Transform startZone;

    [Inject] private CargoFactory _cargoFactory;


    public void Initialize()
    {
        AnimateSpawn();
    }

    private async void AnimateSpawn()
    {
        GameObject cargo = await _cargoFactory.SpawnCargoAsync(startZone.position + Vector3.up * spawnHeight);
        Rigidbody _rb = cargo.GetComponent<Rigidbody>();
        _rb.isKinematic = true;

        cargo.transform.DOMove(startZone.position, spawnDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => _rb.isKinematic = false);

        cargo.transform.DORotate(new Vector3(0, 360, 0), spawnDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);
    }

    public void AnimateRemoval(GameObject cargo)
    {
        Rigidbody _rb = cargo.GetComponent<Rigidbody>();
        _rb.isKinematic = true;

        cargo.transform.DOMove(cargo.transform.position + Vector3.up * launchHeight, launchDuration)
            .SetEase(Ease.InExpo);

        cargo.transform.DORotate(new Vector3(360, 360, 360), launchDuration * 0.7f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetDelay(launchDuration * 1f)
            .OnComplete(async () =>
            {
                await _cargoFactory.SpawnCargoAsync(startZone.position + Vector3.up * spawnHeight);
                Destroy(cargo);
            });
    }

}