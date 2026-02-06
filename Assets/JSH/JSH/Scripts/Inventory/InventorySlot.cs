
public class InventorySlot
{
    public int stack; 
    public IUpgradable instance;  //스킬, 아이템 인스턴스 이 안에 데이터 있음
    public bool unlocked;

    public InventorySlot(IUpgradable instance, int stack, bool unlocked = false)
    {
        this.instance = instance;
        this.stack = stack;
        this.unlocked = unlocked; 
    }

    public bool TryCombine(int requireStack, out IUpgradable newInstance)
    {
        newInstance = null;
        if (stack < requireStack) return false;
        if (instance is ItemInstance itemInstance)
        {
            var baseData = itemInstance.baseData;
            int tier = baseData.Tier;
            GradeType grade = baseData.Grade;

            if (tier > 1)
            {
                tier--;
            }
            else
            {
                grade = GetNextRarity(grade);
                tier = 4;
            }

            ItemCard card = new ItemCard
            {
                Type = baseData.Type,
                Grade = grade,
                Tier = tier
            };

            ItemDataSO newData = ItemSkillDataManager.Instance.GetItemData(card);
            if (newData != null)
            {
                newInstance = new ItemInstance(newData);
            }
        }
        else if (instance is SkillInstance skillInstance)
        {
            var baseData = skillInstance.baseData;
            GradeType rarity = GetNextRarity(baseData.Grade);

            ItemCard card = new ItemCard
            {
                Type = baseData.Type,
                Grade = rarity,
                Tier = 4
            };

            SkillDataSO newSkill = ItemSkillDataManager.Instance.GetSkillData(card);
            if (newSkill != null)
            {
                newInstance = SkillFactory.CreateInstance(newSkill);
            }
        }
        stack -= requireStack;
        return true;
    }
    private GradeType GetNextRarity(GradeType rarity)
    {
        switch (rarity)
        {
            case GradeType.Normal: return GradeType.Advanced;
            case GradeType.Advanced: return GradeType.Rare;
            case GradeType.Rare: return GradeType.Heroic;
            case GradeType.Heroic: return GradeType.Legendary;
            case GradeType.Legendary: return GradeType.Mythical;
            default: return GradeType.Mythical;
        }
    }
}