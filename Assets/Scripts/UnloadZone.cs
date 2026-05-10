using UnityEngine;

public class UnloadZone : MonoBehaviour
{
    [SerializeField] private ForkController forkController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Forklift"))
        {
            forkController.SetInUnloadZone(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.CompareTag("Forklift"))
        {
            forkController.SetInUnloadZone(false);
        }
    }
}