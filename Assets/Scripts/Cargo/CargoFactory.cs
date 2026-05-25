using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public class CargoFactory : MonoBehaviour
{
    [SerializeField] private AssetReference cargoPrefab;
    private AsyncOperationHandle<GameObject> _handle;
    private DiContainer _container;

    [Inject]
    public void Construct(DiContainer container)
    {
        _container = container;
    }

    private async void Start()
    {
        await SpawnCargo();
    }

    public async Task<GameObject> SpawnCargo()
    {
        if (!_handle.IsValid())
            _handle = cargoPrefab.LoadAssetAsync<GameObject>();

        await _handle.Task;
        return _container.InstantiatePrefab(_handle.Result, transform.position, Quaternion.identity, null);
    }

    private void OnDestroy()
    {
        if (_handle.IsValid())
            Addressables.Release(_handle);
    }
}