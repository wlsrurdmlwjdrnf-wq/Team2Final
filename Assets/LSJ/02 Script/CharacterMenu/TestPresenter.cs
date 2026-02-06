using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPresenter : MonoBehaviour
{
    [SerializeField] private TestView testView;

    private void Awake()
    {
        testView.OnAddGoldClicked += AddGoldButtonClick;
        testView.OnSpendGoldClicked += SpendGoldButtonClick;
        testView.OnAtkPowerUpClicked += AtkPowerUpButtonClick;
        testView.OnCritRateUpClicked += CritRateUpButtonClick;
    }
    public void AddGoldButtonClick()
    {
        BigNumber amount = new BigNumber(77777);
        PlayerResourceManager.Instance.AddResource(ResourceType.Gold, amount);
    }
    public void SpendGoldButtonClick()
    {
        BigNumber amount = new BigNumber(44444);
        PlayerResourceManager.Instance.SpendResource(ResourceType.Gold, amount);
    }
    public void AtkPowerUpButtonClick()
    {
        StatModifier modifier = new StatModifier(StatType.AttackPower, Operation.Multiply, 2);
        PlayerStatManager.Instance.AddModifier(modifier);
    }
    public void CritRateUpButtonClick()
    {
        StatModifier modifier = new StatModifier(StatType.CritRate, Operation.Add, 0.1f);
        PlayerStatManager.Instance.AddModifier(modifier);
    }
}
