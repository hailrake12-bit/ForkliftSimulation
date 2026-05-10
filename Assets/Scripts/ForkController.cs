using UnityEngine;

public class ForkController : MonoBehaviour
{
    [SerializeField] private Transform forkAssembly;
    [SerializeField] private ForkliftController forkliftController;
    [SerializeField] private float unloadMinHeight = 0.219f;

    private GameObject _cargoOnFork;
    private Rigidbody _cargoRb;
    private Collider[] _cargoColliders;
    private CargoAnimator _detachedCargo;
    private float _detachTimer = 0f;
    private bool _waitingForLaunch = false;
    private bool _isCargoAttached = false;
    private bool _isInUnloadZone = false;
    private bool _wasDelivered = false;
    private SlotSide? _tipSlot = null;
    private SlotSide? _baseSlot = null;

    public void OnForkEnterSlot(SlotSide side, bool isTip)
    {
        if (_isCargoAttached) return;
        if (isTip) _tipSlot = side;
        else _baseSlot = side;
        CheckAttach();
    }

    public void OnForkExitSlot(SlotSide side, bool isTip)
    {
        if (_isCargoAttached) return;
        if (isTip && _tipSlot == side) _tipSlot = null;
        else if (!isTip && _baseSlot == side) _baseSlot = null;
    }

    public void OnForkAssemblyExitCargo()
    {
        if (!_wasDelivered) return;
        _waitingForLaunch = true;
        _detachTimer = 0f;
        _wasDelivered = false;
    }

    private void CheckAttach()
    {
        if (_tipSlot == null || _baseSlot == null) return;

        bool isOpposite =
            (_tipSlot == SlotSide.North && _baseSlot == SlotSide.South) ||
            (_tipSlot == SlotSide.South && _baseSlot == SlotSide.North) ||
            (_tipSlot == SlotSide.West && _baseSlot == SlotSide.East) ||
            (_tipSlot == SlotSide.East && _baseSlot == SlotSide.West);

        if (isOpposite) AttachCargo();
    }

    private void AttachCargo()
    {
        GameObject cargo = null;
        foreach (var slot in FindObjectsByType<SlotTrigger>(FindObjectsSortMode.None))
        {
            cargo = slot.transform.root.gameObject;
            break;
        }

        if (cargo == null) return;

        _cargoOnFork = cargo;
        _cargoRb = cargo.GetComponent<Rigidbody>();
        _cargoColliders = cargo.GetComponentsInChildren<Collider>();

        foreach (var col in _cargoColliders)
            col.enabled = false;

        _cargoRb.linearVelocity = Vector3.zero;
        _cargoRb.angularVelocity = Vector3.zero;
        _cargoRb.isKinematic = true;
        _cargoOnFork.transform.SetParent(forkAssembly);
        _isCargoAttached = true;
        _tipSlot = null;
        _baseSlot = null;
        forkliftController.SetCargoAttached(true);
    }

    public void DetachCargo()
    {
        if (!_isCargoAttached) return;

        _detachedCargo = _cargoOnFork.GetComponent<CargoAnimator>();
        _cargoOnFork.transform.SetParent(null);
        _cargoRb.isKinematic = false;

        foreach (var col in _cargoColliders)
            col.enabled = true;

        _cargoOnFork = null;
        _cargoRb = null;
        _cargoColliders = null;
        _isCargoAttached = false;
        _wasDelivered = true;
        forkliftController.SetCargoAttached(false);
    }

    private void Update()
    {
        if (_isCargoAttached && _isInUnloadZone)
        {
            if (forkAssembly.localPosition.y <= unloadMinHeight)
                DetachCargo();
        }

        if (_waitingForLaunch)
        {
            _detachTimer += Time.deltaTime;
            if (_detachTimer >= 2f)
            {
                _waitingForLaunch = false;
                _detachedCargo?.StartLaunchAnimation();
            }
        }
    }

    public void SetInUnloadZone(bool value)
    {
        _isInUnloadZone = value;
    }
}