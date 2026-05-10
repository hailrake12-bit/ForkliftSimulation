using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ForkController forkController;
    [SerializeField] private GameObject cargoPrefab;
    [SerializeField] private Transform spawnPoint;

    public void SpawnCargo()
    {
        GameObject cargo = Instantiate(cargoPrefab, spawnPoint.position, Quaternion.identity);

        SlotTrigger[] slots = cargo.GetComponentsInChildren<SlotTrigger>();
        foreach (var slot in slots)
            slot.SetForkController(forkController);

        CargoTrigger cargoTrigger = cargo.GetComponentInChildren<CargoTrigger>();
        if (cargoTrigger != null)
            cargoTrigger.SetForkController(forkController);
    }
}