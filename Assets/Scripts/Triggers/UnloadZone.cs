using UnityEngine;
using Zenject;

public class UnloadZone : MonoBehaviour
{
    [Inject] private ForkController _forkController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Forklift"))
            _forkController.SetInUnloadZone(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Forklift"))
            _forkController.SetInUnloadZone(false);
    }
}