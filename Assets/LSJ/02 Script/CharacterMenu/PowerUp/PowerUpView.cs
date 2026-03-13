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
    public void UpdateAtk(string atk, string atkLv, string atkCost)
    {
        atkText.text = atk;
        atkLvText.text = "Lv." + atkLv;
        atkCostText.text = atkCost;
    }
    public void UpdateHp(string hp, string hpLv, string hpCost)
    {
        hpText.text = hp;
        hpLvText.text = "Lv." + hpLv;
        hpCostText.text = hpCost;
    }
    public void UpdateHpRegen(string hpRegen, string hpRegenLv, string hpRegenCost)
    {
        hpRegenText.text = hpRegen;
        hpRegenLvText.text = "Lv." + hpRegenLv;
        hpRegenCostText.text = hpRegenCost;
    }
    public void UpdateCriDmg(string criDmg, string criDmgLv, string criDmgCost)
    {
        criDmgText.text = criDmg;
        criDmgLvText.text = "Lv." + criDmgLv;
        criDmgCostText.text = criDmgCost;
    }
    public void UpdateCriRate(string criRate, string criRateLv, string criRateCost)
    {
        criRateText.text = criRate;
        criRateLvText.text = "Lv." + criRateLv;
        criRateCostText.text = criRateCost;
    }
}
