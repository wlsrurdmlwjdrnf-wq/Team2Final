using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class TestSlotUI : MonoBehaviour, IPoolable
{
    public InventorySlot Slot;
    public Image Image;
    public Image Frame;
    public TextMeshProUGUI NameTxt;
    public TextMeshProUGUI LevelTxt;
    public TextMeshProUGUI TierTxt;
    public TextMeshProUGUI StackTxt;
    public Slider StackSlider;
    public bool IsUnlocked = false;

    [SerializeField] private bool _IsEquipSlot = false;
    [SerializeField] private EDataType _equipType;

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
            if (Slot == null && _IsEquipSlot)
            {
                if (_equipType == updatedSlot.GetDataType() && updatedSlot.IsEquipped)
                {
                    Slot = updatedSlot;
                }
            }
            else if (Slot == null && !_IsEquipSlot) { Slot = updatedSlot; }
            else if (Slot.Id == updatedSlot.Id) { SetUp(updatedSlot); }
        }
    }
    public void OnClick() 
    {
        _eventChannel.RaiseEvent(EGameEventType.SlotClicked, Slot);
    }
    public void SetEmpty() 
    {
        Slot = null;
        Frame.color = Color.gray;
        Image.color = Color.gray;
        Image.sprite = null;
        NameTxt.text = "Empty";
        LevelTxt.text = "+0";
        TierTxt.text = "0";
        StackTxt.text = $"0/{PublicConst.UpgradeStack}";
        StackSlider.value = 0f;
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
            _ = LoadIcon(Item.Name);
            IsUnlocked = Slot.Unlocked;
        }
        else if (Slot.BaseData is SkillDataSO Skill) 
        {
            name = Skill.Name;
            grade = Skill.Grade;
            _ = LoadIcon(Enum.GetName(typeof(ESkillEffectType), Skill.SkillType));
            level = Slot.Level;
            stack = Slot.Stack;
            IsUnlocked = Slot.Unlocked;
        }
        if (!Slot.Unlocked)
        {
            Frame.color = Color.gray;
            Image.color = Color.gray;
        }
        else 
        {
            Image.color = Color.white;
            switch (grade)
            {
                case GradeType.Normal: Frame.color = Color.white; break;
                case GradeType.Advanced: Frame.color = Color.green; break;
                case GradeType.Rare: Frame.color = Color.blue; break;
                case GradeType.Heroic: Frame.color = new Color(1f, 0f, 1f); break;
                case GradeType.Legendary: Frame.color = Color.yellow; break;
                case GradeType.Mythical: Frame.color = Color.red; break;
            }
        }
  
        NameTxt.text = name;
        LevelTxt.text = $"+{level}";
        TierTxt.text = $"{tier}";
        StackTxt.text = $"{stack}/{PublicConst.UpgradeStack}";
        StackSlider.value = Mathf.Clamp((float)stack/PublicConst.UpgradeStack, 0f, 1f);
    }

    private async Task LoadIcon(string address) 
    {
        AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(address);

        Sprite sprite = await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Image.sprite = sprite;
        }
        else
        {
            Debug.Log($"IconLoadFailed{address}");
        }
    }

    public void SetEquipSlot(EDataType type) 
    { 
        _IsEquipSlot = true;
        _equipType = type;
    }
    public void SetPool(IPool pool) { _pool = pool; }
    public void ReturnPool() { _pool.Enqueue(this); }
}
