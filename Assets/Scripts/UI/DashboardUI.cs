using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using Zenject;

public class DashboardUI : MonoBehaviour
{
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private TMP_Text fuelPercent;
    [SerializeField] private TMP_Text controlsText;

    [Inject] private ForkliftController _forkliftController;

    private void Start()
    {
        controlsText.text =
            "УПРАВЛЕНИЕ:\n" +
            "T - двигатель " +
            "WASD - движение\n" +
            "Q/E - вилки " +
            "C - камера";

        _forkliftController.FuelStream
            .Subscribe(fuel =>
            {
                fuelSlider.value = fuel / _forkliftController.MaxFuel;
                fuelPercent.text = $"Топлива осталось:\n{fuel / _forkliftController.MaxFuel * 100f:F0}%";
            })
            .AddTo(this);
    }
}