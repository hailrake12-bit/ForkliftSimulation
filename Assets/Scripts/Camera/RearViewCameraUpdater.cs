using UnityEngine;
using UnityEngine.Rendering;

public class RearViewCameraUpdater : MonoBehaviour
{
    [Tooltip("Сколько раз в секунду обновляется зеркало")]
    [SerializeField] private float _refreshRate = 20f;

    private Camera _camera;
    private float _interval;
    private float _timer;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        _camera.enabled = false;

        _interval = 1f / _refreshRate;
    }

    private void LateUpdate()
    {
        _timer += Time.deltaTime;

        if (_timer < _interval)
            return;

        _timer -= _interval;
        RenderMirror();
    }

    private void RenderMirror()
    {
        var request = new RenderPipeline.StandardRequest
        {
            destination = _camera.targetTexture
        };

        if (RenderPipeline.SupportsRenderRequest(_camera, request))
            RenderPipeline.SubmitRenderRequest(_camera, request);
    }
}