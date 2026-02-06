using System;
using UnityEngine;
using UnityEngine.UI;

public class TestView : MonoBehaviour
{
    [SerializeField] private Button testButton;
    [SerializeField] private Button testButton2;
    [SerializeField] private Button testButton3;
    [SerializeField] private Button testButton4;

    public event Action OnAddGoldClicked;
    public event Action OnSpendGoldClicked;
    public event Action OnAtkPowerUpClicked;
    public event Action OnCritRateUpClicked;

    private void Awake()
    {
        testButton.onClick.AddListener(() =>  OnAddGoldClicked?.Invoke());
        testButton2.onClick.AddListener(() =>  OnSpendGoldClicked?.Invoke());
        testButton3.onClick.AddListener(() =>  OnAtkPowerUpClicked?.Invoke());
        testButton4.onClick.AddListener(() =>  OnCritRateUpClicked?.Invoke());
    }
}
