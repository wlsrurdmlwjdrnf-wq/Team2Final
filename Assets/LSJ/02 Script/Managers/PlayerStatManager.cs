using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 모디파이어(스탯에 영향을 주는 요소)
[System.Serializable]
public class StatModifier
{
    public StatType statType;
    public Operation operation;
    public float value;

    public StatModifier(StatType type, Operation op, float val)
    {
        statType = type;
        operation = op;
        value = val;
    }
}
[System.Serializable]
public class PlayerSaveData
{
    public int level;
    public Tier tier;

    // StatModifier를 직렬화 가능하게 만들기 위해
    // 간단한 직렬화용 클래스 사용 (enum은 string으로 저장)
    public List<SerializableModifier> modifiers = new List<SerializableModifier>();
}

[System.Serializable]
public class SerializableModifier
{
    public string statType;     // StatType을 string으로
    public string operation;    // Operation을 string으로
    public float value;

    // 생성자 (StatModifier -> SerializableModifier 변환용)
    public SerializableModifier(StatModifier mod)
    {
        statType = mod.statType.ToString();
        operation = mod.operation.ToString();
        value = mod.value;
    }

    // 역변환 메서드
    public StatModifier ToStatModifier()
    {
        if (!Enum.TryParse<StatType>(statType, out var type) ||
            !Enum.TryParse<Operation>(operation, out var op))
        {
            Debug.LogWarning($"잘못된 modifier 데이터: {statType} / {operation}");
            return null;
        }

        return new StatModifier(type, op, value);
    }
}

public class PlayerStatManager : Singleton<PlayerStatManager>
{
    [Header("기본 데이터")]
    [SerializeField] private PlayerBaseStatsSO _baseStats;

    // 현재 상태 (세이브 로드 필요)
    private int _currentLevel = 1;
    private Tier _currentTier = Tier.Stone;

    // BigNumber 캐시 (큰 값용)
    private Dictionary<StatType, BigNumber> _bigCachedValues = new Dictionary<StatType, BigNumber>();
    // float 캐시 (작은 범위 스탯용)
    private Dictionary<StatType, float> _floatCachedValues = new Dictionary<StatType, float>();

    private bool _isDirty = true;

    // 모든 모디파이어
    private readonly List<StatModifier> _modifiers = new List<StatModifier>();

    private void Awake()
    {
        if (_baseStats == null)
        {
            Debug.LogError("PlayerBaseStatsSO가 할당되지 않았습니다!");
            enabled = false;
            return;
        }

        MarkDirty();
    }

    public void MarkDirty()
    {
        _isDirty = true;
    }

    public void AddModifier(StatModifier modifier)
    {
        _modifiers.Add(modifier);
        MarkDirty();
    }

    public void RemoveModifier(StatModifier modifier)
    {
        _modifiers.Remove(modifier);
        MarkDirty();
    }
    // 모디파이어 객체 비교 쉽게 하는 거 추가하기

    public void ClearModifiers()
    {
        _modifiers.Clear();
        MarkDirty();
    }

    private void RecalculateIfNeeded()
    {
        if (!_isDirty) return;

        _bigCachedValues.Clear();
        _floatCachedValues.Clear();

        BigNumber tierBonus = new BigNumber(GetTierBonus(_currentTier));

        // BigNumber 스탯들
        _bigCachedValues[StatType.AttackPower] = CalculateBigStat(StatType.AttackPower, tierBonus, 1.0);
        _bigCachedValues[StatType.MaxHP] = CalculateBigStat(StatType.MaxHP, tierBonus, 10.0);
        _bigCachedValues[StatType.HPRegenPerSec] = CalculateBigStat(StatType.HPRegenPerSec, new BigNumber(1), 1.0);
        _bigCachedValues[StatType.GoldMultiplier] = CalculateBigStat(StatType.GoldMultiplier, new BigNumber(1), 1.0);
        _bigCachedValues[StatType.ExpMultiplier] = CalculateBigStat(StatType.ExpMultiplier, new BigNumber(1), 1.0);
        _bigCachedValues[StatType.CritDamage] = CalculateBigStat(StatType.CritDamage, new BigNumber(1), 1.0);

        // float 스탯들
        _floatCachedValues[StatType.CritRate] = CalculateFloatStat(StatType.CritRate, v => Mathf.Clamp01(v));
        _floatCachedValues[StatType.MaxMana] = CalculateFloatStat(StatType.MaxMana, v => Mathf.Max(1.0f, v));
        _floatCachedValues[StatType.ManaRegenPerSec] = CalculateFloatStat(StatType.ManaRegenPerSec, v => Mathf.Max(0f, v));
        _floatCachedValues[StatType.AttackSpeed] = CalculateFloatStat(StatType.AttackSpeed, v => Mathf.Max(0.1f, v));
        _floatCachedValues[StatType.MoveSpeed] = CalculateFloatStat(StatType.MoveSpeed, v => Mathf.Max(-10f, v));

        _isDirty = false;
    }

    // BigNumber 스탯 계산 헬퍼
    private BigNumber CalculateBigStat(StatType type, BigNumber baseMultiplier, double minValue)
    {
        BigNumber baseVal = new BigNumber(GetBaseValue(type));
        BigNumber final = baseVal * baseMultiplier;
        final += GetAdditiveBig(type);
        final *= GetMultiplicativeBig(type);
        return final >= new BigNumber(minValue) ? final : new BigNumber(minValue);
    }

    // float 스탯 계산 헬퍼
    private float CalculateFloatStat(StatType type, Func<float, float> clampFunc)
    {
        float baseVal = GetBaseValueFloat(type);
        float final = baseVal;
        final += GetAdditiveFloat(type);
        final *= GetMultiplicativeFloat(type);
        return clampFunc(final);
    }

    // 기본값 헬퍼 (BigNumber용)
    private double GetBaseValue(StatType type)
    {
        return type switch
        {
            StatType.AttackPower => _baseStats.baseAttackPower,
            StatType.MaxHP => _baseStats.baseMaxHP,
            StatType.HPRegenPerSec => _baseStats.baseHPRegenPerSec,
            StatType.GoldMultiplier => _baseStats.baseGoldMultiplier,
            StatType.ExpMultiplier => _baseStats.baseExpMultiplier,
            StatType.CritDamage => _baseStats.baseCritDamage,
            _ => 0.0
        };
    }

    // 기본값 헬퍼 (float용)
    private float GetBaseValueFloat(StatType type)
    {
        return type switch
        {
            StatType.CritRate => _baseStats.baseCritRate,
            StatType.MaxMana => _baseStats.baseMaxMana,
            StatType.ManaRegenPerSec => _baseStats.baseManaRegenPerSec,
            StatType.AttackSpeed => _baseStats.baseAttackSpeed,
            StatType.MoveSpeed => _baseStats.baseMoveSpeed,
            _ => 0f
        };
    }

    // 스탯 합연산, 곱연산
    private BigNumber GetAdditiveBig(StatType type)
    {
        BigNumber sum = new BigNumber(0);
        foreach (var m in _modifiers)
            if (m.statType == type && m.operation == Operation.Add)
                sum += new BigNumber(m.value);
        return sum;
    }

    private BigNumber GetMultiplicativeBig(StatType type)
    {
        BigNumber mul = new BigNumber(1);
        foreach (var m in _modifiers)
            if (m.statType == type && m.operation == Operation.Multiply)
                mul *= new BigNumber(m.value);
        return mul;
    }

    private float GetAdditiveFloat(StatType type)
    {
        float sum = 0f;
        foreach (var m in _modifiers)
            if (m.statType == type && m.operation == Operation.Add)
                sum += m.value;
        return sum;
    }

    private float GetMultiplicativeFloat(StatType type)
    {
        float mul = 1f;
        foreach (var m in _modifiers)
            if (m.statType == type && m.operation == Operation.Multiply)
                mul *= m.value;
        return mul;
    }

    // 외부 프로퍼티들
    public int PlayerLevel => _currentLevel;
    public Tier PlayerTier => _currentTier;

    // BigNumber 프로퍼티
    public BigNumber AttackPower => GetBig(StatType.AttackPower);
    public BigNumber MaxHP => GetBig(StatType.MaxHP);
    public BigNumber HPRegenPerSec => GetBig(StatType.HPRegenPerSec);
    public BigNumber GoldMultiplier => GetBig(StatType.GoldMultiplier);
    public BigNumber ExpMultiplier => GetBig(StatType.ExpMultiplier);
    public BigNumber CritDamage => GetBig(StatType.CritDamage);

    private BigNumber GetBig(StatType type)
    {
        RecalculateIfNeeded();
        _bigCachedValues.TryGetValue(type, out BigNumber value);
        return value ?? new BigNumber(0);
    }

    // float 프로퍼티
    public float MaxMana => GetFloat(StatType.MaxMana);
    public float ManaRegenPerSec => GetFloat(StatType.ManaRegenPerSec);
    public float CritRate => GetFloat(StatType.CritRate);
    public float AttackSpeed => GetFloat(StatType.AttackSpeed);
    public float MoveSpeed => GetFloat(StatType.MoveSpeed);

    private float GetFloat(StatType type)
    {
        RecalculateIfNeeded();
        _floatCachedValues.TryGetValue(type, out float value);
        return value;
    }

    // 레벨업 / 티어 승급 / 리셋 / JSON 저장·로드
    public void LevelUp()
    {
        _currentLevel++;
        MarkDirty();
    }

    public void PromoteTier(Tier newTier)
    {
        _currentTier = newTier;
        MarkDirty();
    }

    private float GetTierBonus(Tier tier)
    {
        switch (tier)
        {
            case Tier.Bronze: return 2f;
            case Tier.Silver: return 5f;
            case Tier.Gold: return 10f;
            case Tier.Platinum: return 20f;
            case Tier.Diamond: return 50f;
            case Tier.Amethyst: return 100f;
            case Tier.Ruby: return 300f;
            case Tier.Brilliance: return 1000f;
            default: return 1f;
        }
    }

    public void ResetToBase()
    {
        _currentLevel = _baseStats.baseLevel;
        _currentTier = _baseStats.baseTier;
        ClearModifiers();
        MarkDirty();
    }

    public string GetSaveJson()
    {
        var saveData = new PlayerSaveData
        {
            level = _currentLevel,
            tier = _currentTier
        };

        foreach (var mod in _modifiers)
        {
            if (mod.statType == StatType.MoveSpeed || mod.statType == StatType.AttackSpeed) continue; // 공속과 이속은 저장 x
            saveData.modifiers.Add(new SerializableModifier(mod));
        }

        return JsonUtility.ToJson(saveData, true);
    }

    public bool LoadFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogWarning("빈 JSON 문자열 → 로드 실패");
            return false;
        }

        try
        {
            var saveData = JsonUtility.FromJson<PlayerSaveData>(json);

            if (saveData == null)
            {
                Debug.LogWarning("JSON 파싱 실패: saveData가 null");
                return false;
            }

            _currentLevel = Mathf.Max(1, saveData.level);
            _currentTier = saveData.tier;

            _modifiers.Clear();

            foreach (var sMod in saveData.modifiers)
            {
                var mod = sMod.ToStatModifier();
                if (mod != null)
                {
                    _modifiers.Add(mod);
                }
                else
                {
                    Debug.LogWarning($"잘못된 모디파이어 무시됨: {sMod.statType} / {sMod.operation}");
                }
            }

            MarkDirty();

            Debug.Log($"JSON 로드 성공 → Lv.{_currentLevel} {_currentTier}, 모디파이어 {_modifiers.Count}개");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON 로드 중 예외 발생: {e.Message}\nJSON 내용:\n{json}");
            return false;
        }
    }

    // 스탯 표기 가져오기
    public string GetFormatted(StatType type)
    {
        if (_bigCachedValues.ContainsKey(type))
        {
            return BigNumberFormatter.ToFormatted(_bigCachedValues[type]);
        }
        else
        {
            return BigNumberFormatter.ToFormatted(_floatCachedValues[type]);
        }
    }
    // 디버그용
    [ContextMenu("Log All Stats")]
    private void LogStats()
    {
        RecalculateIfNeeded();
        foreach (var kvp in _bigCachedValues)
        {
            Debug.Log($"{kvp.Key} (Big): {GetFormatted(kvp.Key)}");
        }
        foreach (var kvp in _floatCachedValues)
        {
            Debug.Log($"{kvp.Key} (float): {GetFormatted(kvp.Key)}");
        }
    }
}
