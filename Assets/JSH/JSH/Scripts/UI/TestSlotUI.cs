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
    public Image CoolTimeImage;
    public TextMeshProUGUI NameTxt;
    public TextMeshProUGUI LevelTxt;
    public TextMeshProUGUI TierTxt;
    public TextMeshProUGUI StackTxt;
    public Slider StackSlider;
    public GameObject EquipDot;
    public bool IsUnlocked = false;

    [SerializeField] private bool _IsEquipSlot = false;
    [SerializeField] private EDataType _equipType;
    private SkillInstance _skillInstance;
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

    private void HandleEvent(EGameEventType type, IGameEventPayload payload)
    {
        switch (type) 
        {
            case EGameEventType.SlotUpdated:
            case EGameEventType.EquipChanged:
                if (payload is SlotPayload slotPayload)
                {
                    InventorySlot updatedSlot = slotPayload.Slot;
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
                break;
        }      
    }
    private void Update()
    {
        if (_skillInstance != null && _IsEquipSlot)
        {
            float progress = _skillInstance.GetCooldownProgress();

            if (_skillInstance.baseData.TriggerCount > 0)
            {
                CoolTimeImage.fillAmount = 1f - progress;
            }
            else 
            {
                CoolTimeImage.fillAmount = 1f - progress;
            }
        }
    }
    public void OnClick() 
    {
        if (_IsEquipSlot && _equipType == EDataType.Skill)
        {
            if (Slot == null)
            {
                _eventChannel.RaiseEvent(EGameEventType.RequestAddSkillSlot);
            }
            else if (InventorySystem.instance._OnSkillChange) 
            {
                _eventChannel.RaiseEvent(EGameEventType.RequestChangeSkill, new SlotPayload(Slot));
            }
            else
            {
                _eventChannel.RaiseEvent(EGameEventType.RequestSkillUse, new SlotPayload(Slot));
            }
        }
        else 
        {
            _eventChannel.RaiseEvent(EGameEventType.SlotClicked, new SlotPayload(Slot));
        }
    }
    public void SetEmpty() 
    {
        Slot = null;
        Frame.color = Color.gray;
        Image.color = Color.black;
        Image.sprite = null;
        NameTxt.text = "";
        LevelTxt.text = "";
        TierTxt.text = "";
        StackTxt.text = $"0/{PublicConst.UpgradeStack}";
        StackSlider.value = 0f;
        EquipDot.SetActive(false);
        if (CoolTimeImage != null)
        CoolTimeImage.fillAmount = 0;
    }
    public void SetUp(InventorySlot slot)
    {
        Slot = slot;
        _skillInstance =SkillManager.Instance.GetSkillInstance(Slot);
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
            TierTxt.text = $"{tier}";
        }
        else if (Slot.BaseData is SkillDataSO Skill) 
        {
            name = Skill.Name;
            grade = Skill.Grade;
            _ = LoadIcon(Enum.GetName(typeof(ESkillEffectType), Skill.SkillType));
            level = Slot.Level;
            stack = Slot.Stack;
            IsUnlocked = Slot.Unlocked;
            TierTxt.text = " ";
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
                case GradeType.Immortal: Frame.color = Color.black; break;
            }
        }
        
        EquipDot.SetActive(Slot.IsEquipped);
        NameTxt.text = name;
        LevelTxt.text = $"+{level}";
        StackTxt.text = $"{stack}/{PublicConst.UpgradeStack}";
        StackSlider.value = Mathf.Clamp((float)stack/PublicConst.UpgradeStack, 0f, 1f);
        CoolTimeImage.fillAmount = 0;
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
