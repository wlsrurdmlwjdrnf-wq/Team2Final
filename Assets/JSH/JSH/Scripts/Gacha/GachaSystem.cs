using System.Collections.Generic;
using UnityEngine;

//가챠결과물
public struct ItemCard 
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

    //무기 & 악세 Grade 확률
    private Dictionary<GradeType, float[]> _itemGradeChanceTable = new Dictionary<GradeType, float[]>
    {
        { GradeType.Normal,    new float[]{ 68.58f, 54.2f, 33.1f, 10.56f, 7.2f, 5.08f, 4.41f, 2.68f, 0.11f, 0.01f } },
        { GradeType.Advanced,  new float[]{ 25.5f, 32.8f, 43.5f, 55.84f, 45f, 31.2f, 21f, 18f, 4.2f, 0.5f } },
        { GradeType.Rare,      new float[]{ 5.4f, 11.2f, 18.4f, 23.5f, 28.5f, 40.05f, 36.5f, 29f, 18f, 9.49f } },
        { GradeType.Heroic,    new float[]{ 0.5149f, 1.7197f, 4.7982f, 9.59f, 18.575f, 22.618f, 36.5f, 48.2f, 73.57f, 82.85f } },
        { GradeType.Legendary, new float[]{ 0.005f, 0.08f, 0.2f, 0.5f, 0.7f, 1.02f, 1.54f, 2.05f, 4.02f, 7f } },
        { GradeType.Mythical,  new float[]{ 0.0001f, 0.0003f, 0.0018f, 0.01f, 0.025f, 0.032f, 0.05f, 0.07f, 0.1f, 0.15f } }
    };
    //스킬 확률
    private Dictionary<GradeType, float[]> _skillGradeChanceTable = new Dictionary<GradeType, float[]>
    {
        { GradeType.Normal,    new float[]{ 40f } },
        { GradeType.Advanced,  new float[]{ 30f } },
        { GradeType.Rare,      new float[]{ 20f } },
        { GradeType.Heroic,    new float[]{ 8f } },
        { GradeType.Legendary, new float[]{ 1f } },
        { GradeType.Mythical,  new float[]{ 1f } }
    };
    //무기 & 악세 Tier 확률
    private int[] _itemTierChanceTable = { 40, 30, 20, 10 };
    //가챠레벨업테이블
    private int[] _gachaLevelTable = { 100, 250, 1000, 4000, 12000, 25000, 27500, 55000, 88000, 100000 };
    private void OnEnable()
    {
        _eventChannel.OnEventRaised += HandleEvent;
    }
    private void OnDisable()
    {
        _eventChannel.OnEventRaised -= HandleEvent;
    }
    public void Initialize()
    {
        //테스트
        DrawGacha(EDataType.Weapon, 11);
        DrawGacha(EDataType.Accessories, 11);   
        DrawGacha(EDataType.Skill, 11);


        InventorySystem.Instance.SortInventory(EDataType.Weapon);
        InventorySystem.Instance.SortInventory(EDataType.Accessories);
        InventorySystem.Instance.SortInventory(EDataType.Skill);

        //InventorySystem.Instance.PrintInventory(EDataType.Weapon);
        //InventorySystem.Instance.PrintInventory(EDataType.Accessories);
        //InventorySystem.Instance.PrintInventory(EDataType.Skill);

        SkillManager.Instance.RefreshSlots();
    }
    private void HandleEvent(EGameEventType type, object payload)
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
        int totalCost = GachaCost * count;
        if (!PlayerResourceManager.Instance.SpendResource(ResourceType.Diamond, new BigNumber(totalCost))) return;
        Debug.Log($"{gachaType}:{count}, Cost:{GachaCost * count}");
        gachaResults.Clear();
        for (int i = 0; i < count; i++)
        {
            gachaResults.Add(DrawOnce(gachaType));
        }
        foreach (var card in gachaResults)
        {
            _eventChannel.RaiseEvent(EGameEventType.GachaPull, card);
        }
    }
    private ItemCard DrawOnce(EDataType gachaType) 
    {
        //가챠비용 차감
        switch (gachaType)
        {
            case EDataType.Weapon:
                _weaponGachaCount++;
                if ( _weaponGachaLvl < _gachaLevelTable.Length - 1 
                    && _weaponGachaCount >= _gachaLevelTable[_weaponGachaLvl]) 
                { 
                    _weaponGachaLvl++; 
                    _weaponGachaCount = 0; 
                }
                _eventChannel.RaiseEvent(EGameEventType.GachaProgressUpdate, new GachaProgressPayload(
                    EDataType.Weapon, _weaponGachaCount, _gachaLevelTable[_weaponGachaLvl], _weaponGachaLvl));
                return new ItemCard(
                    gachaType,
                    DrawRarity(_itemGradeChanceTable, _weaponGachaLvl),
                    DrawGrade()
                    );
            case EDataType.Accessories:
                _accessoryGachaCount++;
                if ( _accessoryGachaLvl < _gachaLevelTable.Length - 1 
                    && _accessoryGachaCount >= _gachaLevelTable[_accessoryGachaLvl]) 
                { 
                    _accessoryGachaLvl++; 
                    _accessoryGachaCount = 0;
                }
                _eventChannel.RaiseEvent(EGameEventType.GachaProgressUpdate, new GachaProgressPayload(
                  EDataType.Accessories, _accessoryGachaCount, _gachaLevelTable[_accessoryGachaLvl], _accessoryGachaLvl));
                return new ItemCard(
                  gachaType,
                  DrawRarity(_itemGradeChanceTable, _accessoryGachaLvl),
                  DrawGrade()
                  );
            case EDataType.Skill:
                return new ItemCard(
                  gachaType,
                  DrawRarity(_skillGradeChanceTable)
                  );
            default:
                return new ItemCard(EDataType.Weapon, GradeType.Normal);
        } 
    }
    //희귀도 추첨 > 일반, 레어, 신화 등등...
    private GradeType DrawRarity( Dictionary<GradeType, float[]> gachaTable, int gachaLvl = 0) 
    {
        float randomValue = Random.Range(_min, _max);
        float cumulative = 0;

        var order = new List<GradeType> {
            GradeType.Normal, GradeType.Advanced, GradeType.Rare,
            GradeType.Heroic, GradeType.Legendary, GradeType.Mythical };

        foreach (var grde in order) 
        {
            cumulative += gachaTable[grde][gachaLvl];
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

        for (int i = 0; i < _itemTierChanceTable.Length; i++) 
        {
            cumulative += _itemTierChanceTable[i];
            if (randomValue <= cumulative)
            {
                return _itemTierChanceTable.Length - i;
            }
        }
        return _itemTierChanceTable.Length;
    }
}

