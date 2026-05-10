using UnityEngine;

public enum SlotSide { North, South, West, East }

public class SlotTrigger : MonoBehaviour
{
    [SerializeField] private SlotSide side;
    [SerializeField] private ForkController forkController;

    public void SetForkController(ForkController controller) => forkController = controller;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ForkTip"))
            forkController.OnForkEnterSlot(side, isTip: true);
        else if (other.CompareTag("ForkBase"))
            forkController.OnForkEnterSlot(side, isTip: false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ForkTip"))
            forkController.OnForkExitSlot(side, isTip: true);
        else if (other.CompareTag("ForkBase"))
            forkController.OnForkExitSlot(side, isTip: false);
    }
}