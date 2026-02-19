using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI atkText;
    [SerializeField] private TextMeshProUGUI atkLvText;
    [SerializeField] private TextMeshProUGUI atkCostText;
    [SerializeField] private Button atkUpButton;

    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI hpLvText;
    [SerializeField] private TextMeshProUGUI hpCostText;
    [SerializeField] private Button hpUpButton;

    [SerializeField] private TextMeshProUGUI hpRegenText;
    [SerializeField] private TextMeshProUGUI hpRegenLvText;
    [SerializeField] private TextMeshProUGUI hpRegenCostText;
    [SerializeField] private Button hpRegenUpButton;

    [SerializeField] private TextMeshProUGUI criDmgText;
    [SerializeField] private TextMeshProUGUI criDmgLvText;
    [SerializeField] private TextMeshProUGUI criDmgCostText;
    [SerializeField] private Button criDmgUpButton;

    [SerializeField] private TextMeshProUGUI criRateText;
    [SerializeField] private TextMeshProUGUI criRateLvText;
    [SerializeField] private TextMeshProUGUI criRateCostText;
    [SerializeField] private Button criRateUpButton;

    public event Action OnAtkUpButtonClicked;
    public event Action OnHpUpButtonClicked;
    public event Action OnHpRegenUpButtonClicked;
    public event Action OnCriDmgUpButtonClicked;
    public event Action OnCriRateUpButtonClicked;

    private void Awake()
    {
        atkUpButton.onClick.AddListener(() => OnAtkUpButtonClicked?.Invoke());
        hpUpButton.onClick.AddListener(() => OnHpUpButtonClicked?.Invoke());
        hpRegenUpButton.onClick.AddListener(() => OnHpRegenUpButtonClicked?.Invoke());
        criDmgUpButton.onClick.AddListener(() => OnCriDmgUpButtonClicked?.Invoke());
        criRateUpButton.onClick.AddListener(() => OnCriRateUpButtonClicked?.Invoke());
    }
}
