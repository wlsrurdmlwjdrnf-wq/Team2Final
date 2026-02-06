using System.Collections.Generic;
using UnityEngine;

//°¡Ã­°á°ú¹°
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

public class GachaSystem : MonoBehaviour
{
    public int GachaCost = 50;
    private int _weaaponGachaLvl = 0;
    private int _accessoriesGachaLvl = 0;
    private float _min = 0;
    private float _max = 100;

    //°¡Ã­ °á°ú ¸®½ºÆ®
    public List<ItemCard> gachaResults = new List<ItemCard>();

    //¹«±â & ¾Ç¼¼ Grade È®·ü
    private Dictionary<GradeType, float[]> _itemGradeChanceTable = new Dictionary<GradeType, float[]>
    {
        { GradeType.Normal,    new float[]{ 68.58f, 54.2f, 33.1f, 10.56f, 7.2f, 5.08f, 4.41f, 2.68f, 0.11f, 0.01f } },
        { GradeType.Advanced,  new float[]{ 25.5f, 32.8f, 43.5f, 55.84f, 45f, 31.2f, 21f, 18f, 4.2f, 0.5f } },
        { GradeType.Rare,      new float[]{ 5.4f, 11.2f, 18.4f, 23.5f, 28.5f, 40.05f, 36.5f, 29f, 18f, 9.49f } },
        { GradeType.Heroic,    new float[]{ 0.5149f, 1.7197f, 4.7982f, 9.59f, 18.575f, 22.618f, 36.5f, 48.2f, 73.57f, 82.85f } },
        { GradeType.Legendary, new float[]{ 0.005f, 0.08f, 0.2f, 0.5f, 0.7f, 1.02f, 1.54f, 2.05f, 4.02f, 7f } },
        { GradeType.Mythical,  new float[]{ 0.0001f, 0.0003f, 0.0018f, 0.01f, 0.025f, 0.032f, 0.05f, 0.07f, 0.1f, 0.15f } }
    };
    //½ºÅ³ È®·ü
    private Dictionary<GradeType, float[]> _skillGradeChanceTable = new Dictionary<GradeType, float[]>
    {
        { GradeType.Normal,    new float[]{ 40f } },
        { GradeType.Advanced,  new float[]{ 30f } },
        { GradeType.Rare,      new float[]{ 20f } },
        { GradeType.Heroic,    new float[]{ 8f } },
        { GradeType.Legendary, new float[]{ 1f } },
        { GradeType.Mythical,  new float[]{ 1f } }
    };
    //¹«±â & ¾Ç¼¼ Tier È®·ü
    private int[] _ItemTierChanceTable = { 40, 30, 20, 10 };
    private void Start()
    {
        InventorySystem.Instance.Initialize();
        Initialize();
        TestUIManager.Instance.Initialize();
    }
    public void Initialize()
    {
        //Å×½ºÆ®
        DrawGacha(EDataType.Weapon,11);
        foreach (var gachaResult in gachaResults) 
        {
            Debug.Log($"{gachaResult.Type}{gachaResult.Grade}{gachaResult.Tier}"); 
        }
        DrawGacha(EDataType.Accessories, 11);
        foreach (var gachaResult in gachaResults)
        {
            Debug.Log($"{gachaResult.Type}{gachaResult.Grade}{gachaResult.Tier}");
        }
        DrawGacha(EDataType.Skill, 11);
        foreach (var gachaResult in gachaResults)
        {
            Debug.Log($"{gachaResult.Type}{gachaResult.Grade}{gachaResult.Tier}");
        }
        InventorySystem.Instance.SortInventory(EDataType.Weapon);
        InventorySystem.Instance.SortInventory(EDataType.Accessories);
        InventorySystem.Instance.SortInventory(EDataType.Skill);

        InventorySystem.Instance.PrintInventory(EDataType.Weapon);
        InventorySystem.Instance.PrintInventory(EDataType.Accessories);
        InventorySystem.Instance.PrintInventory(EDataType.Skill);

        InventorySystem.Instance.Equip(EDataType.Weapon, 0);
        InventorySystem.Instance.Equip(EDataType.Accessories, 0);
        InventorySystem.Instance.Equip(EDataType.Skill, 0);
        InventorySystem.Instance.Equip(EDataType.Skill, 1);
        InventorySystem.Instance.Equip(EDataType.Skill, 2);
        InventorySystem.Instance.Equip(EDataType.Skill, 3);

        TotalStats stat = InventorySystem.Instance.CalculateStats();
        Debug.Log($"TotalATK:{stat.ATK},HP{stat.HP}");
        SkillManager.Instance.Initialize();
    }
    //°¡Ã­½ÇÇà
    public void TestPull(EDataType gachaType) 
    {
        DrawGacha(gachaType, 11);
    }
    public void DrawGacha(EDataType gachaType, int count = 1)
    {
        gachaResults.Clear();
        for (int i = 0; i < count; i++)
        {
            gachaResults.Add(DrawOnce(gachaType));
        }

        foreach (var card in gachaResults)
        {
            switch (gachaType)
            {
                case EDataType.Weapon:
                    InventorySystem.Instance.AddItem(ItemSkillDataManager.Instance.GetItemData(card));
                    break;
                case EDataType.Accessories:
                    InventorySystem.Instance.AddItem(ItemSkillDataManager.Instance.GetItemData(card));
                    break;
                case EDataType.Skill:
                    InventorySystem.Instance.AddSkill(ItemSkillDataManager.Instance.GetSkillData(card));
                    break;
            }
        }
    }
    private ItemCard DrawOnce(EDataType gachaType) 
    {
        //°¡Ã­ºñ¿ë Â÷°¨
        switch (gachaType)
        {
            case EDataType.Weapon:
                return new ItemCard(
                    gachaType,
                    DrawRarity(_itemGradeChanceTable, _weaaponGachaLvl),
                    DrawGrade()
                    );
            case EDataType.Accessories:
                return new ItemCard(
                  gachaType,
                  DrawRarity(_itemGradeChanceTable, _accessoriesGachaLvl),
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
    //Èñ±Íµµ ÃßÃ· > ÀÏ¹Ý, ·¹¾î, ½ÅÈ­ µîµî...
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
    //µî±Þ ÃßÃ· > 4, 3, 2, 1
    private int DrawGrade() 
    {
        float randomValue = Random.Range(_min, _max);
        float cumulative = 0;

        for (int i = 0; i < _ItemTierChanceTable.Length; i++) 
        {
            cumulative += _ItemTierChanceTable[i];
            if (randomValue <= cumulative)
            {
                return _ItemTierChanceTable.Length - i;
            }
        }
        return _ItemTierChanceTable.Length;
    }
}

