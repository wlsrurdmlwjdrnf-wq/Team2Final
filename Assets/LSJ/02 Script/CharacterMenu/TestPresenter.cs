using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TestPresenter : MonoBehaviour
{
    [SerializeField] private TestView testView;

    private StageManager _sm;
    private void Awake()
    {
        _sm = StageManager.Instance;
        testView.OnAddGoldClicked += AddGoldButtonClick;
        testView.OnSpendGoldClicked += SpendGoldButtonClick;
        testView.OnAtkPowerUpClicked += AtkPowerUpButtonClick;
        testView.OnCritRateUpClicked += CritRateUpButtonClick;
        testView.OnLevelUpClicked += LevelUpButtonClick;
        testView.OnBossStageClicked += BossStageButtonClick;
        testView.OnTierStageClicked += TierStageButtonClick;
    }
    public void AddGoldButtonClick()
    {
        BigNumber amount = new BigNumber(12300000000000);
        PlayerResourceManager.Instance.AddResource(ResourceType.Gold, amount);
        testView.UpdateView();
    }
    public void SpendGoldButtonClick()
    {
        BigNumber amount = new BigNumber(3210000000000);
        PlayerResourceManager.Instance.SpendResource(ResourceType.Gold, amount);
        testView.UpdateView();
    }
    public void AtkPowerUpButtonClick()
    {
        StatModifier modifier = new StatModifier(StatType.AttackPower, Operation.Multiply, 1000);
        PlayerStatManager.Instance.AddModifier(modifier);
    }
    public void CritRateUpButtonClick()
    {
        StatModifier modifier = new StatModifier(StatType.CritRate, Operation.Add, 0.1f);
        PlayerStatManager.Instance.AddModifier(modifier);
    }
    public void LevelUpButtonClick()
    {
        PlayerLevelUpSystem.TryPlayerLevelUp();
        testView.UpdateView();
    }
    public void BossStageButtonClick()
    {
        _sm.ApplyStage(_sm.GetStageData(_sm.CurrentMainNumber, _sm.CurrentSubNumber, true));
    }
    public void TierStageButtonClick()
    {
        _sm.ApplyStage(_sm.GetStageData(Tier.Bronze));
    }
}
