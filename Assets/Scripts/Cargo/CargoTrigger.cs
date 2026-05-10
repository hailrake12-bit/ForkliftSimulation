using UnityEngine;
using Zenject;

public class CargoTrigger : MonoBehaviour
{
    [Inject] private ForkController _forkController;

    public void SetForkController(ForkController controller) => _forkController = controller;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ForkAssembly"))
            _forkController.OnForkAssemblyExitCargo();
    }
}