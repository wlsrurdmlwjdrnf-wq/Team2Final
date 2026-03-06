using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpPresenter : MonoBehaviour
{
    [SerializeField] private PowerUpView view;

    private static PowerUpModel _model;

    private string _tmpText; // 강화 수치 표기 ( 현재 -> 다음)
    private BigNumber _tmpCost;

    private void Awake()
    {
        view.OnAtkUpButtonClicked += AtkUpButtonClick;
        view.OnHpUpButtonClicked += HpUpButtonClick;
        view.OnHpRegenUpButtonClicked += HpRegenUpButtonClick;
        view.OnCriDmgUpButtonClicked += CriDmgUpButtonClick;
        view.OnCriRateUpButtonClicked += CriRateUpButtonClick;
    }
    private void Start()
    {
        _model = new PowerUpModel(CharacterUpgradeSaveManager.Instance.PowerUpModel);
        UpdateView();
    }
    public void AtkUpButtonClick()
    {
        _tmpCost = CalculateCost(_model.AtkLevel);

        if (!TryLevelUp(_tmpCost)) return;

        SoundManager.Instance.PlaySFX(ESFXType.StatUpButton);
        _model.AddAtkLevel();
        _tmpCost = CalculateCost(_model.AtkLevel);
        _tmpText = ShowNumber(StatType.AttackPower, _model.AtkLevel);
        view.UpdateAtk(_tmpText, _model.AtkLevel.ToString(), BigNumberFormatter.ToFormatted(_tmpCost));
        AddModifier(StatType.AttackPower, _model.AtkLevel);
    }
    public void HpUpButtonClick()
    {
        _tmpCost = CalculateCost(_model.HpLevel);

        if (!TryLevelUp(_tmpCost)) return;

        SoundManager.Instance.PlaySFX(ESFXType.StatUpButton);
        _model.AddHpLevel();
        _tmpCost = CalculateCost(_model.HpLevel);
        _tmpText = ShowNumber(StatType.MaxHP, _model.HpLevel);
        view.UpdateHp(_tmpText, _model.HpLevel.ToString(), BigNumberFormatter.ToFormatted(_tmpCost));
        AddModifier(StatType.MaxHP, _model.HpLevel);
    }
    public void HpRegenUpButtonClick()
    {
        _tmpCost = CalculateCost(_model.HpRegenLevel);

        if (!TryLevelUp(_tmpCost)) return;

        SoundManager.Instance.PlaySFX(ESFXType.StatUpButton);
        _model.AddHpRegenLevel();
        _tmpCost = CalculateCost(_model.HpRegenLevel);
        _tmpText = ShowNumber(StatType.HPRegenPerSec, _model.HpRegenLevel);
        view.UpdateHpRegen(_tmpText, _model.HpRegenLevel.ToString(), BigNumberFormatter.ToFormatted(_tmpCost));
        AddModifier(StatType.HPRegenPerSec, _model.HpRegenLevel);
    }
    public void CriDmgUpButtonClick()
    {
        if(_model.MaxCriDmgLevel == _model.CriDmgLevel) return;
        _tmpCost = CalculateCost(_model.CriDmgLevel);

        if (!TryLevelUp(_tmpCost)) return;

        SoundManager.Instance.PlaySFX(ESFXType.StatUpButton);
        _model.AddCriDmgLevel();
        _tmpCost = CalculateCost(_model.CriDmgLevel);
        _tmpText = ShowNumber(StatType.CritDamage, _model.CriDmgLevel);
        view.UpdateCriDmg(_tmpText, _model.CriDmgLevel.ToString(), BigNumberFormatter.ToFormatted(_tmpCost));
        AddModifier(StatType.CritDamage, _model.CriDmgLevel);
    }
    public void CriRateUpButtonClick()
    {
        if(_model.MaxCriRateLevel == _model.CriRateLevel) return;
        _tmpCost = CalculateCost(_model.CriRateLevel);

        if (!TryLevelUp(_tmpCost)) return;

        SoundManager.Instance.PlaySFX(ESFXType.StatUpButton);
        _model.AddCriRateLevel();
        _tmpCost = CalculateCost(_model.CriRateLevel);
        _tmpText = ShowNumber(StatType.CritRate, _model.CriRateLevel);
        view.UpdateCriRate(_tmpText, _model.CriRateLevel.ToString(), BigNumberFormatter.ToFormatted(_tmpCost));
        AddModifier(StatType.CritRate, _model.CriRateLevel);
    }
    
    // View 전체 업데이트
    private void UpdateView()
    {
        view.UpdateAtk(
            ShowNumber(StatType.AttackPower, _model.AtkLevel),
            _model.AtkLevel.ToString(),
            BigNumberFormatter.ToFormatted(CalculateCost(_model.AtkLevel))
            );
        view.UpdateHp(
            ShowNumber(StatType.MaxHP, _model.HpLevel),
            _model.HpLevel.ToString(),
            BigNumberFormatter.ToFormatted(CalculateCost(_model.HpLevel))
            );
        view.UpdateHpRegen(
            ShowNumber(StatType.HPRegenPerSec, _model.HpRegenLevel),
            _model.HpRegenLevel.ToString(),
            BigNumberFormatter.ToFormatted(CalculateCost(_model.HpRegenLevel))
            );
        view.UpdateCriDmg(
            ShowNumber(StatType.CritDamage, _model.CriDmgLevel),
            _model.CriDmgLevel.ToString(),
            BigNumberFormatter.ToFormatted(CalculateCost(_model.CriDmgLevel))
            );
        view.UpdateCriRate(
            ShowNumber(StatType.CritRate, _model.CriRateLevel),
            _model.CriRateLevel.ToString(),
            BigNumberFormatter.ToFormatted(CalculateCost(_model.CriRateLevel))
            );
    }

    // 업그레이드 비용 계산
    private BigNumber CalculateCost(int level)
    {
        if (level > 1000)
        {
            BigNumber cost = new BigNumber(1);
            int tmpLevelCount = level / 1000;
            for(int i = 0; i <= tmpLevelCount; i++)
            {
                if (i == tmpLevelCount) cost *= new BigNumber(1 + level) * new BigNumber(Mathf.Pow(1.05f, level - 1000 * tmpLevelCount));
                else cost *= new BigNumber(Mathf.Pow(1.05f, 1000));
            }
            return cost;
        }
        else
        {
            BigNumber cost = new BigNumber(1 + level) * new BigNumber(Mathf.Pow(1.05f, level));
            return cost;
        }
    }
    // 업그레이드 가능 여부 반환 및 골드 소모
    private bool TryLevelUp(BigNumber cost)
    {
        return PlayerResourceManager.Instance.SpendResource(ResourceType.Gold, cost);
    }
    // 강화 수치 표기 반환
    private string ShowNumber(StatType type, int level)
    {
        int bonus = level / 1000 + 1;
        switch (type)
        {
            case StatType.AttackPower:
                return $"{level * 2 * bonus} → {(level + 1) * 2 * bonus}";
            case StatType.MaxHP:
                return $"{level * 5 * bonus} → {(level + 1) * 5 * bonus}";
            case StatType.HPRegenPerSec:
                return $"{level * bonus} → {(level + 1) * bonus}";
            case StatType.CritDamage:
                return $"{level * bonus}% → {(level + 1) * bonus}%";
            case StatType.CritRate:
                return (0.1f * level).ToString("N1")+"% → "+ (0.1f * (level + 1)).ToString("N1") + "%";
        }
        return "";
    }
    // 모디파이어 추가
    private void AddModifier(StatType type, int level)
    {
        switch (type)
        {
            case StatType.AttackPower:
                PlayerStatManager.Instance.RemoveModifier(new StatModifier(type, Operation.Add, (level - 1) * 2));
                PlayerStatManager.Instance.AddModifier(new StatModifier(type, Operation.Add, level * 2));
                if (level != 0 && level % 1000 == 0)
                {
                    PlayerStatManager.Instance.RemoveModifier(new StatModifier(type, Operation.Multiply, level / 1000));
                    PlayerStatManager.Instance.AddModifier(new StatModifier(type, Operation.Multiply, level / 1000 + 1));
                }
                break;
            case StatType.MaxHP:
                PlayerStatManager.Instance.RemoveModifier(new StatModifier(type, Operation.Add, (level - 1) * 5));
                PlayerStatManager.Instance.AddModifier(new StatModifier(type, Operation.Add, level * 5));
                if (level != 0 && level % 1000 == 0)
                {
                    PlayerStatManager.Instance.RemoveModifier(new StatModifier(type, Operation.Multiply, level / 1000));
                    PlayerStatManager.Instance.AddModifier(new StatModifier(type, Operation.Multiply, level / 1000 + 1));
                }
                break;
            case StatType.HPRegenPerSec:
                PlayerStatManager.Instance.RemoveModifier(new StatModifier(type, Operation.Add, level - 1));
                PlayerStatManager.Instance.AddModifier(new StatModifier(type, Operation.Add, level));
                if (level != 0 && level % 1000 == 0)
                {
                    PlayerStatManager.Instance.RemoveModifier(new StatModifier(type, Operation.Multiply, level / 1000));
                    PlayerStatManager.Instance.AddModifier(new StatModifier(type, Operation.Multiply, level / 1000 + 1));
                }
                break;
            case StatType.CritDamage:
                PlayerStatManager.Instance.RemoveModifier(new StatModifier(type, Operation.Add, 0.01f * (level - 1)));
                PlayerStatManager.Instance.AddModifier(new StatModifier(type, Operation.Add, 0.01f * level));
                if (level != 0 && level % 1000 == 0)
                {
                    PlayerStatManager.Instance.RemoveModifier(new StatModifier(type, Operation.Multiply, level / 1000));
                    PlayerStatManager.Instance.AddModifier(new StatModifier(type, Operation.Multiply, level / 1000 + 1));
                }
                break;
            case StatType.CritRate:
                PlayerStatManager.Instance.RemoveModifier(new StatModifier(type, Operation.Add, 0.001f * (level - 1)));
                PlayerStatManager.Instance.AddModifier(new StatModifier(type, Operation.Add, 0.001f * level));
                break;
        }
    }
    private void OnDestroy()
    {
        if (view == null) return;
        view.OnAtkUpButtonClicked -= AtkUpButtonClick;
        view.OnHpUpButtonClicked -= HpUpButtonClick;
        view.OnHpRegenUpButtonClicked -= HpRegenUpButtonClick;
        view.OnCriDmgUpButtonClicked -= CriDmgUpButtonClick;
        view.OnCriRateUpButtonClicked -= CriRateUpButtonClick;
    }

    // CharacterUpgradeSaveManager에 넘겨서 저장
    public static PowerUpModel GetModel()
    {
        return _model;
    }
}
