using UnityEngine;
using Zenject;

public class CargoTrigger : MonoBehaviour
{
    [Inject] private ForkController _forkController;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ForkAssembly"))
            _forkController.OnForkAssemblyExitCargo();
    }
}