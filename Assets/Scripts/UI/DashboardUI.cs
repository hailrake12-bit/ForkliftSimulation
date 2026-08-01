using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using Zenject;
using System;

public class DashboardUI : MonoBehaviour
{
    [SerializeField] private Slider fuelSlider;
    [SerializeField] private TMP_Text fuelPercent;
    [SerializeField] private TMP_Text controlsText;

    [Inject] private ForkliftMovement _forkliftController;

    private void Start()
    {
        controlsText.text =
            "УПРАВЛЕНИЕ:\n" +
            "T - двигатель вкл/выкл, " +
            "WASD - движение,\n" +
            "Q/E - вилки поднять/опустить, " +
            "C - поменять камеру,\n" +
            "Z/X - наклонять мачту, " +
            "левый Shift - тормоз.";

        _forkliftController.FuelStream
            .Sample(TimeSpan.FromSeconds(0.5f)) 
            .Subscribe(fuel =>
            {
                fuelSlider.value = fuel / _forkliftController.MaxFuel;
                fuelPercent.text = $"{fuel / _forkliftController.MaxFuel * 100f:F0}%";
            })
            .AddTo(this);
    }
}