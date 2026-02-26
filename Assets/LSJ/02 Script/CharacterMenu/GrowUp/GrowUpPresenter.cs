using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowUpPresenter : MonoBehaviour
{
    [SerializeField] private GrowUpView view;

    private static GrowUpModel _model;

    private string _tmpText; // 성장 수치 표기 ( 현재 -> 다음)

    private void Awake()
    {
        view.OnLevelUpButtonClicked += LevelUpButtonClick;
        view.OnStrUpButtonClicked += StrUpButtonClick;
        view.OnHpUpButtonClicked += HpUpButtonClick;
        view.OnVitUpButtonClicked += VitUpButtonClick;
        view.OnCriUpButtonClicked += CriUpButtonClick;
        view.OnLukUpButtonClicked += LukUpButtonClick;
    }
    private void Start()
    {
        _model = new GrowUpModel(CharacterUpgradeSaveManager.Instance.GrowUpModel);
        UpdateView();
    }
    public void LevelUpButtonClick()
    {
        PlayerLevelUpSystem.TryPlayerLevelUp();
        view.UpdateLevel(PlayerStatManager.Instance.PlayerLevel.ToString(), PlayerLevelUpSystem.GetExpRequirement());
    }
    public void StrUpButtonClick()
    {
        if (!TryLevelUp() || _model.MaxStrLevel == _model.StrLevel) return;

        _model.AddStrLevel();
        _tmpText = ShowNumber(StatType.AttackPower, _model.StrLevel);
        view.UpdateStr(_tmpText, _model.StrLevel.ToString());
        AddModifier(StatType.AttackPower, _model.StrLevel);
    }
    public void HpUpButtonClick()
    {
        if (!TryLevelUp() || _model.MaxHpLevel == _model.HpLevel) return;

        _model.AddHpLevel();
        _tmpText = ShowNumber(StatType.MaxHP, _model.HpLevel);
        view.UpdateHp(_tmpText, _model.HpLevel.ToString());
        AddModifier(StatType.MaxHP, _model.HpLevel);
    }
    public void VitUpButtonClick()
    {
        if (!TryLevelUp() || _model.MaxVitLevel == _model.VitLevel) return;

        _model.AddVitLevel();
        _tmpText = ShowNumber(StatType.HPRegenPerSec, _model.VitLevel);
        view.UpdateVit(_tmpText, _model.VitLevel.ToString());
        AddModifier(StatType.HPRegenPerSec, _model.VitLevel);
    }
    public void CriUpButtonClick()
    {
        if (!TryLevelUp() || _model.MaxCriLevel == _model.CriLevel) return;

        _model.AddCriLevel();
        _tmpText = ShowNumber(StatType.CritDamage, _model.CriLevel);
        view.UpdateCri(_tmpText, _model.CriLevel.ToString());
        AddModifier(StatType.CritDamage, _model.CriLevel);
    }
    public void LukUpButtonClick()
    {
        if (!TryLevelUp() || _model.MaxLukLevel == _model.LukLevel) return;

        _model.AddLukLevel();
        _tmpText = ShowNumber(StatType.GoldMultiplier, _model.LukLevel);
        view.UpdateLuk(_tmpText, _model.LukLevel.ToString());
        AddModifier(StatType.GoldMultiplier, _model.LukLevel);
    }

    // View 전체 업데이트
    private void UpdateView()
    {
        view.UpdateLevel(
            PlayerStatManager.Instance.PlayerLevel.ToString(),
            PlayerLevelUpSystem.GetExpRequirement()
            );
        view.UpdateStr(
            ShowNumber(StatType.AttackPower, _model.StrLevel),
            _model.StrLevel.ToString()
            );
        view.UpdateHp(
            ShowNumber(StatType.MaxHP, _model.HpLevel),
            _model.HpLevel.ToString()
            );
        view.UpdateVit(
            ShowNumber(StatType.HPRegenPerSec, _model.VitLevel),
            _model.VitLevel.ToString()
            );
        view.UpdateCri(
            ShowNumber(StatType.CritDamage, _model.CriLevel),
            _model.CriLevel.ToString()
            );
        view.UpdateLuk(
            ShowNumber(StatType.GoldMultiplier, _model.LukLevel),
            _model.LukLevel.ToString()
            );
    }

    // 업그레이드 가능 여부 반환 및 스탯포인트 소모
    private bool TryLevelUp()
    {
        return PlayerResourceManager.Instance.SpendResource(ResourceType.StatPoint, new BigNumber(1.0));
    }
    // 성장 수치 표기 반환
    private string ShowNumber(StatType type, int level)
    {
        switch (type)
        {
            case StatType.AttackPower:
                return $"{level * 5} → {(level + 1) * 5}";
            case StatType.MaxHP:
                return $"{level * 10} → {(level + 1) * 10}";
            case StatType.HPRegenPerSec:
                return $"{level * 2} → {(level + 1) * 2}";
            case StatType.CritDamage:
                return $"{level * 2}% → {(level + 1) * 2}%";
            case StatType.GoldMultiplier:
                return $"{0.5f * level}% → {0.5f * (level + 1)}%";
        }
        return "";
    }
    // 모디파이어 추가
    private void AddModifier(StatType type, int level)
    {
        switch (type)
        {
            case StatType.AttackPower:
                PlayerStatManager.Instance.RemoveModifier(new StatModifier(type, Operation.Add, (level - 1) * 5));
                PlayerStatManager.Instance.AddModifier(new StatModifier(type, Operation.Add, level * 5));
                break;
            case StatType.MaxHP:
                PlayerStatManager.Instance.RemoveModifier(new StatModifier(type, Operation.Add, (level - 1) * 10));
                PlayerStatManager.Instance.AddModifier(new StatModifier(type, Operation.Add, level * 10));
                break;
            case StatType.HPRegenPerSec:
                PlayerStatManager.Instance.RemoveModifier(new StatModifier(type, Operation.Add, (level - 1) * 2));
                PlayerStatManager.Instance.AddModifier(new StatModifier(type, Operation.Add, level * 2));
                break;
            case StatType.CritDamage:
                PlayerStatManager.Instance.RemoveModifier(new StatModifier(type, Operation.Add, 0.02f * (level - 1)));
                PlayerStatManager.Instance.AddModifier(new StatModifier(type, Operation.Add, 0.02f * level));
                break;
            case StatType.GoldMultiplier:
                PlayerStatManager.Instance.RemoveModifier(new StatModifier(type, Operation.Add, 0.005f * (level - 1)));
                PlayerStatManager.Instance.AddModifier(new StatModifier(type, Operation.Add, 0.005f * level));
                break;
        }
    }
    private void OnDestroy()
    {
        if (view == null) return;
        view.OnLevelUpButtonClicked -= LevelUpButtonClick;
        view.OnStrUpButtonClicked -= StrUpButtonClick;
        view.OnHpUpButtonClicked -= HpUpButtonClick;
        view.OnVitUpButtonClicked -= VitUpButtonClick;
        view.OnCriUpButtonClicked -= CriUpButtonClick;
        view.OnLukUpButtonClicked -= LukUpButtonClick;
    }

    // CharacterUpgradeSaveManager에 넘겨서 저장
    public static GrowUpModel GetModel()
    {
        return _model;
    }

}
