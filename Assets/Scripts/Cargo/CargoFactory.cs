using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public class CargoFactory : MonoBehaviour, IInitializable, System.IDisposable
{
    [SerializeField] private AssetReferenceGameObject cargoPrefab;

    private AsyncOperationHandle<GameObject> _handle;
    private GameObject _cachedPrefab;


    public void Initialize()
    {
        LoadPrefabAsync();
    }

    private async void LoadPrefabAsync()
    {
        _handle = cargoPrefab.LoadAssetAsync<GameObject>();
        _cachedPrefab = await _handle.Task;
    }

    public async Task<GameObject> SpawnCargoAsync(Vector3 position)
    {
        if (_cachedPrefab == null)
            await _handle.Task;

        return Instantiate(_handle.Result, position, Quaternion.identity);
    }

    public void Dispose()
    {
        if (_handle.IsValid())
            Addressables.Release(_handle);
    }
}