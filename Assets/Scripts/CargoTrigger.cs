using UnityEngine;

public class CargoTrigger : MonoBehaviour
{
    [SerializeField] private ForkController forkController;

    public void SetForkController(ForkController controller) => forkController = controller;

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ForkAssembly"))
            forkController.OnForkAssemblyExitCargo();
    }
}