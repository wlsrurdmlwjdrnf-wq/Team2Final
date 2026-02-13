using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestSlotUI : MonoBehaviour, IPoolable
{
    public InventorySlot Slot;
    public Image Image;
    public TextMeshProUGUI NameTxt;
    public TextMeshProUGUI LevelTxt;
    public TextMeshProUGUI TierTxt;
    public TextMeshProUGUI StackTxt;

    private IPool _pool;
    [SerializeField] private GameEventChannelSO _eventChannel;
    private void OnEnable()
    {
        _eventChannel.OnEventRaised += HandleEvent;
    }

    private void OnDisable()
    {
        _eventChannel.OnEventRaised -= HandleEvent;
    }

    private void HandleEvent(EGameEventType type, object payload)
    {
        if (type == EGameEventType.SlotUpdated && payload is InventorySlot updatedSlot)
        {
            if (Slot == null) 
            { 
                Slot = updatedSlot; 
            }
            else if (Slot.Id == updatedSlot.Id)
            {
                SetUp(updatedSlot);
            }
        }
    }
    public void OnClick() 
    {
        _eventChannel.RaiseEvent(EGameEventType.SlotClicked, Slot);
    }
    public void SetEmpty() 
    {
        Slot = null;
        Image.color = Color.gray;
        NameTxt.text = "Empty";
        LevelTxt.text = "+0";
        TierTxt.text = "0";
        StackTxt.text = $"0/{PublicConst.UpgradeStack}";
    }
    public void SetUp(InventorySlot slot)
    {
        Slot = slot;
   
        GradeType grade = GradeType.Normal;
        string name = "";
        int tier = 0;
        int level = 0;
        int stack = 0;  
  
        if (Slot.BaseData is ItemDataSO Item)
        {
            name = Item.Name;
            grade = Item.Grade;
            tier = Item.Tier;
            level = Slot.Level;
            stack = Slot.Stack;
        }
        else if (Slot.BaseData is SkillDataSO Skill) 
        {
            name = Skill.Name;
            grade = Skill.Grade;
            level = Slot.Level;
            stack = Slot.Stack;
        }
        if (!Slot.Unlocked)
        {
            Image.color = Color.gray;
        }
        else 
        {
            switch (grade)
            {
                case GradeType.Normal: Image.color = Color.white; break;
                case GradeType.Advanced: Image.color = Color.green; break;
                case GradeType.Rare: Image.color = Color.blue; break;
                case GradeType.Heroic: Image.color = new Color(1f, 0f, 1f); break;
                case GradeType.Legendary: Image.color = Color.yellow; break;
                case GradeType.Mythical: Image.color = Color.red; break;
            }
        }
        NameTxt.text = name;
        LevelTxt.text = $"+{level}";
        TierTxt.text = $"{tier}";
        StackTxt.text = $"{stack}/{PublicConst.UpgradeStack}";
    }

    public void SetPool(IPool pool) { _pool = pool; }
    public void ReturnPool() { _pool.Enqueue(this); }
}
