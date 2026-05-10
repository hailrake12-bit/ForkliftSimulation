using UnityEngine;
using Zenject;

public class CargoFactory : MonoBehaviour
{
    [SerializeField] private GameObject cargoPrefab;
    [SerializeField] private Transform spawnPoint;

    private DiContainer _container;

    [Inject]
    public void Construct(DiContainer container)
    {
        _container = container;
    }

    public GameObject Create(Vector3 position)
    {
        return _container.InstantiatePrefab(cargoPrefab, position, Quaternion.identity, null);
    }

    public void SpawnCargo()
    {
        _container.InstantiatePrefab(cargoPrefab, spawnPoint.position, Quaternion.identity, null);
    }
}