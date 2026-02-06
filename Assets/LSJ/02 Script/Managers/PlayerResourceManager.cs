using System;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    EXP,
    StatPoint,
    Gold,
    Emerald,
    Diamond,
    EnhancementCube,
    FireStone,
    WaterStone,
    WindStone,
    EarthStone,
}

public class PlayerResourceManager : Singleton<PlayerResourceManager>
{
    private readonly Dictionary<ResourceType, BigNumber> _resources = new();

    private void Awake()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            _resources[type] = new BigNumber(0);
        }

        // 시작 재화 테스트
        _resources[ResourceType.Gold] = new BigNumber(1000);
        _resources[ResourceType.Diamond] = new BigNumber(123456789101112);
        _resources[ResourceType.EXP] = new BigNumber(MathF.Pow(10,10));
    }

    public void AddResource(ResourceType type, BigNumber amount)
    {
        if (amount.mantissa == 0) return;
        _resources[type] = _resources[type] + amount;
        Debug.Log($"[{type}] +{amount} (현재: {GetFormatted(type)})");
    }

    public bool SpendResource(ResourceType type, BigNumber amount)
    {
        BigNumber current = _resources[type];

        if (current < amount)
        {
            Debug.LogWarning($"[{type}] 부족! 필요: {amount}, 보유: {current}");
            return false;
        }

        _resources[type] = current - amount;
        Debug.Log($"[{type}] -{amount} (남은: {GetFormatted(type)})");
        return true;
    }

    // 플레이어 자원량 가져오기
    public BigNumber GetResource(ResourceType type) => _resources[type];

    public string GetFormatted(ResourceType type)
    {
        BigNumber bn = GetResource(type);
        return BigNumberFormatter.ToFormatted(bn);
    }

    // JSON 저장용 직렬화 클래스
    [System.Serializable]
    private class ResourceSaveData
    {
        public List<ResourceEntry> entries = new List<ResourceEntry>();
    }

    [System.Serializable]
    private class ResourceEntry
    {
        public string type;
        public double mantissa;
        public long exponent;
    }

    public string GetSaveJson()
    {
        var saveData = new ResourceSaveData();

        foreach (var kvp in _resources)
        {
            saveData.entries.Add(new ResourceEntry
            {
                type = kvp.Key.ToString(),
                mantissa = kvp.Value.mantissa,
                exponent = kvp.Value.exponent
            });
        }

        return JsonUtility.ToJson(saveData, true);
    }

    public bool LoadFromJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;

        try
        {
            var saveData = JsonUtility.FromJson<ResourceSaveData>(json);

            if (saveData?.entries == null) return false;

            _resources.Clear();

            foreach (var entry in saveData.entries)
            {
                if (Enum.TryParse<ResourceType>(entry.type, out var type))
                {
                    _resources[type] = new BigNumber(entry.mantissa, entry.exponent);
                }
            }

            Debug.Log("자원 로드 성공!");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"자원 로드 실패: {e.Message}");
            return false;
        }
    }

    // 디버그용
    [ContextMenu("Log All Resources")]
    private void LogAll()
    {
        foreach (var kvp in _resources)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value} ({GetFormatted(kvp.Key)})");
        }
    }
}