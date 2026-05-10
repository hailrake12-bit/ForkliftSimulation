using UnityEngine;
using Zenject;

public enum SlotSide { North, South, West, East }

public class SlotTrigger : MonoBehaviour
{
    [SerializeField] private SlotSide side;

    [Inject] private ForkController _forkController;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ForkTip"))
            _forkController.OnForkEnterSlot(side, isTip: true);
        else if (other.CompareTag("ForkBase"))
            _forkController.OnForkEnterSlot(side, isTip: false);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ForkTip"))
            _forkController.OnForkExitSlot(side, isTip: true);
        else if (other.CompareTag("ForkBase"))
            _forkController.OnForkExitSlot(side, isTip: false);
    }
}