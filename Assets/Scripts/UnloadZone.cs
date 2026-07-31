using UnityEngine;
using Zenject;

public class UnloadZone : MonoBehaviour
{
    private string _cargoTag = "Cargo";
    private string _forkliftTag = "Forklift";
    private float _checkDelay = 0.3f;
    private GameObject _currentCargo;
    private bool _forkliftInside;

    [Inject] private CargoAnimator _cargoAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_cargoTag))
        {
            _currentCargo = other.gameObject;
        }
        else if (other.CompareTag(_forkliftTag))
        {
            _forkliftInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_forkliftTag))
        {
            _forkliftInside = false;

            if (_currentCargo != null)
            {
                Invoke(nameof(TryDeliver), _checkDelay);
            }
        }

        if (other.CompareTag(_cargoTag) && other.gameObject == _currentCargo)
        {

            _currentCargo = null;
        }
    } 

    private void TryDeliver()
    {
        if (_currentCargo != null && !_forkliftInside) _cargoAnimator.AnimateRemoval(_currentCargo);
    }
}