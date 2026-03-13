using System.Collections.Generic;
using UnityEngine;

//가챠결과물
public struct ItemCard : IGameEventPayload
{
    public EDataType Type;
    public GradeType Grade;
    public int Tier;
    public ItemCard(EDataType type, GradeType rarity, int grade = 0) 
    {
        this.Type = type;
        this.Grade = rarity;
        this.Tier = grade;
    }
}

public class GachaSystem : Singleton<GachaSystem> 
{
    public int GachaCost = 50;
    private int _weaponGachaLvl = 0;
    private int _accessoryGachaLvl = 0;
    private int _weaponGachaCount = 0;
    private int _accessoryGachaCount = 0;
    private float _min = 0;
    private float _max = 100;

    //가챠 결과 리스트
    public List<ItemCard> gachaResults = new List<ItemCard>();
    [SerializeField] private GameEventChannelSO _eventChannel;
    [SerializeField] private GachaDataSO _gachaData;
    private void OnEnable()
    {
        _eventChannel.OnEventRaised += HandleEvent;
    }
    private void OnDisable()
    {
        _eventChannel.OnEventRaised -= HandleEvent;
    }
    private void HandleEvent(EGameEventType type, IGameEventPayload payload)
    {
        switch (type)
        {
            case EGameEventType.GachaRequest:
                if (payload is GachaRequestPayload request)
                {
                    DrawGacha(request.Type, request.Count);
                }
                break;
        }
    }
    //가챠실행
    public void DrawGacha(EDataType gachaType, int count = 1)
    {
        int totalCost = GachaCost * (count - count/11);
        //가챠비용 체크&차감
        if (!PlayerResourceManager.Instance.SpendResource(ResourceType.Diamond, new BigNumber(totalCost))) return;
        gachaResults.Clear();
        for (int i = 0; i < count; i++)
        {
            gachaResults.Add(DrawOnce(gachaType));
        }
        foreach (var card in gachaResults)
        {
            _eventChannel.RaiseEvent(EGameEventType.GachaPull, card);
        }
        _eventChannel.RaiseEvent(EGameEventType.GachaRequestEnd);
    }
    private ItemCard DrawOnce(EDataType gachaType) 
    {
        switch (gachaType)
        {
            case EDataType.Weapon:
                _weaponGachaCount++;
                if (_weaponGachaLvl < _gachaData.gachaLevelTable.Length - 1
                    && _weaponGachaCount >= _gachaData.gachaLevelTable[_weaponGachaLvl])
                {
                    _weaponGachaLvl++;
                    _weaponGachaCount = 0;
                }
                _eventChannel.RaiseEvent(EGameEventType.GachaProgressUpdate,
                    new GachaProgressPayload(EDataType.Weapon, _weaponGachaCount,
                        _gachaData.gachaLevelTable[_weaponGachaLvl], _weaponGachaLvl));

                return new ItemCard(
                    gachaType,
                    DrawRarity(_gachaData.itemGradeChanceTable, _weaponGachaLvl),
                    DrawGrade()
                );

            case EDataType.Accessories:
                _accessoryGachaCount++;
                if (_accessoryGachaLvl < _gachaData.gachaLevelTable.Length - 1
                    && _accessoryGachaCount >= _gachaData.gachaLevelTable[_accessoryGachaLvl])
                {
                    _accessoryGachaLvl++;
                    _accessoryGachaCount = 0;
                }
                _eventChannel.RaiseEvent(EGameEventType.GachaProgressUpdate,
                    new GachaProgressPayload(EDataType.Accessories, _accessoryGachaCount,
                        _gachaData.gachaLevelTable[_accessoryGachaLvl], _accessoryGachaLvl));

                return new ItemCard(
                    gachaType,
                    DrawRarity(_gachaData.itemGradeChanceTable, _accessoryGachaLvl),
                    DrawGrade()
                );

            case EDataType.Skill:
                return new ItemCard(
                    gachaType,
                    DrawRarity(_gachaData.skillGradeChanceTable)
                );

            default:
                return new ItemCard(EDataType.Weapon, GradeType.Normal);
        }

    }
    //희귀도 추첨 > 일반, 레어, 신화 등등...
    private GradeType DrawRarity(GachaDataSO.GradeChance[] gachaTable, int gachaLvl = 0)
    {
        float randomValue = Random.Range(_min, _max);
        float cumulative = 0;

        var order = new List<GradeType> {
            GradeType.Normal, GradeType.Advanced, GradeType.Rare,
            GradeType.Heroic, GradeType.Legendary, GradeType.Mythical };

        foreach (var grde in order)
        {
            var chance = System.Array.Find(gachaTable, g => g.gradeType == grde).chances[gachaLvl];
            cumulative += chance;
            if (randomValue <= cumulative)
            {
                return grde;
            }
        }
        return GradeType.Normal;
    }

    //등급 추첨 > 4, 3, 2, 1
    private int DrawGrade() 
    {
        float randomValue = Random.Range(_min, _max);
        float cumulative = 0;

        for (int i = 0; i < _gachaData.itemTierChanceTable.Length; i++)
        {
            cumulative += _gachaData.itemTierChanceTable[i];
            if (randomValue <= cumulative)
            {
                return _gachaData.itemTierChanceTable.Length - i;
            }
        }
        return _gachaData.itemTierChanceTable.Length;

    }
}
