using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestSlotUI : MonoBehaviour
{
    public InventorySlot Slot;
    public Image Image;
    public TextMeshProUGUI NameTxt;
    public TextMeshProUGUI LevelTxt;
    public TextMeshProUGUI TierTxt;
    public void SetUp(InventorySlot slot)
    {
        Slot = slot;
        object Data = null;
        GradeType grade = GradeType.Normal;
        string name = "";
        int tier = 0;
        int level = 0;
        if (Slot.instance is ItemInstance bi) { Data = bi.baseData; }
        else if (Slot.instance is SkillInstance bsi) { Data = bsi.baseData; }
        if (Data is ItemDataSO Item)
        {
            name = Item.Name;
            grade = Item.Grade;
            tier = Item.Tier;
            level = Item.Level;
        }
        else if (Data is SkillDataSO Skill) 
        {
            name = Skill.Name;
            grade = Skill.Grade;
            level = Skill.Level;
        }
        switch (grade)
        {
            case GradeType.Normal:
                Image.color = Color.white;
                break;
            case GradeType.Advanced:
                Image.color = Color.green;
                break;
            case GradeType.Rare:
                Image.color = Color.blue;
                break;
            case GradeType.Heroic:
                Image.color = new Color(1f, 0f, 1f);
                break;
            case GradeType.Legendary:
                Image.color = Color.yellow;
                break;
            case GradeType.Mythical:
                Image.color = Color.red;
                break;
        }
        NameTxt.text = name;
        LevelTxt.text = $"+{level}";
        TierTxt.text = $"{tier}";
    }

    public void Equip() 
    {
        if (Slot.instance is ItemInstance item) 
        {
            InventorySystem.Instance.Equip(item.baseData.Type, Slot);
        }
        
    }
}
