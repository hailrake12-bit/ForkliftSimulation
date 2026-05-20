using UnityEngine;
using UnityEngine.Rendering;

public class RearViewCameraUpdater : MonoBehaviour
{
    [Tooltip("Сколько раз в секунду обновляется зеркало")]
    [SerializeField] private float refreshRate = 20f;
    [SerializeField] private Camera rearCamera;

    private float _interval;
    private float _timer;

    private void Awake()
    {

        rearCamera.enabled = false;

        _interval = 1f / refreshRate;
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
            destination = rearCamera.targetTexture
        };

        if (RenderPipeline.SupportsRenderRequest(rearCamera, request))
            RenderPipeline.SubmitRenderRequest(rearCamera, request);
    }
}