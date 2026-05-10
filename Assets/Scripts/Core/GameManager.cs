using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    [Inject] private CargoFactory _cargoFactory;
    [Inject] private ForkController _forkController;

    public void SpawnCargo()
    {
        GameObject cargo = _cargoFactory.Create(spawnPoint.position);

        SlotTrigger[] slots = cargo.GetComponentsInChildren<SlotTrigger>();
        foreach (var slot in slots)
            slot.SetForkController(_forkController);

        CargoTrigger cargoTrigger = cargo.GetComponentInChildren<CargoTrigger>();
        if (cargoTrigger != null)
            cargoTrigger.SetForkController(_forkController);
    }
}