using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanelUI : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO _eventChannel;
    [Header("Upgrade")]
    [SerializeField] private TextMeshProUGUI _costTxt;
    [SerializeField] private TextMeshProUGUI _nameTxt;
    [SerializeField] private TextMeshProUGUI _descriptionTxt;
    [SerializeField] private TextMeshProUGUI _mechanismTxt;
    [SerializeField] private TextMeshProUGUI _cooltimeTxt;
    [SerializeField] private TextMeshProUGUI _manaCostTxt;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _equipButton;
    [SerializeField] private TextMeshProUGUI _equipButtonTxt;
    [SerializeField] private Button _prevSlotButton;
    [SerializeField] private Button _nextSlotButton;
    [SerializeField] private TestSlotUI _presentSlotUI;
    private InventorySlot _currSlot;
    private Color _tempColor;
    private void Awake()
    {
        _eventChannel.OnEventRaised += HandleEvent;
        gameObject.SetActive(false);
        _tempColor = _equipButton.image.color;

        _prevSlotButton.onClick.AddListener(() =>
        {
            var prev = InventorySystem.instance.GetPrevSlot(_currSlot);
            if (prev != null) OpenPopUp(prev);
        });
        _nextSlotButton.onClick.AddListener(() =>
        {
            var next = InventorySystem.instance.GetNextSlot(_currSlot);
            if (next != null) OpenPopUp(next);
        });
    }
    private void OnDestroy()
    {
        _eventChannel.OnEventRaised -= HandleEvent;
    }
    private void HandleEvent(EGameEventType type, IGameEventPayload payload)
    {
        if (payload is SlotPayload slot)
        {
            switch (type)
            {
                case EGameEventType.SlotClicked:
                    OpenPopUp(slot.Slot);
                    break;
                case EGameEventType.SlotUpdated:
                    RefreshUI(slot.Slot);
                    break;
                case EGameEventType.EquipChanged:
                    RefreshUI(slot.Slot);
                    break;
            }
        }
        if (type == EGameEventType.CloseUpgradeUI) Close();
    }
    private void OpenPopUp(InventorySlot slot)
    {
        if (slot.GetDataType() != EDataType.Skill) { return; }
        _currSlot = slot;
        gameObject.SetActive(true);

        if (slot.Unlocked)
        {
            _upgradeButton.interactable = true;
            _upgradeButton.onClick.RemoveAllListeners();
            _upgradeButton.onClick.AddListener(() =>
            {
                _eventChannel.RaiseEvent(EGameEventType.UpgradeRequest, new SlotPayload(_currSlot));
            });
        }
        else
        {
            _upgradeButton.interactable = false;
        }

        RefreshText(slot);
    }
    private void RefreshUI(InventorySlot slot)
    {
        if (_currSlot != null && slot.Id != _currSlot.Id) return;

        RefreshText(slot);
        _prevSlotButton.interactable = InventorySystem.instance.GetPrevSlot(slot) != null;
        _nextSlotButton.interactable = InventorySystem.instance.GetNextSlot(slot) != null;
    }
    private void RefreshText(InventorySlot slot)
    {
        float beforeActive = slot.ActiveEffectValue;
        float beforePassive = slot.PassiveEffectValue;

        _presentSlotUI.SetEmpty();
        if (_currSlot != null) _presentSlotUI.SetUp(_currSlot);

        if (slot.BaseData is SkillDataSO skill)
        {
            if(ItemSkillDataManager.Instance.GradeMap.TryGetValue(skill.Grade, out string value))
            _nameTxt.text = $"[{value}]{skill.Name}";
            _descriptionTxt.text = skill.DescryptionTxt;

            if (skill.ModifyAmount <= 0) 
            {
                _mechanismTxt.text = string.Format(skill.MechanismTxtTemplate, beforeActive);
            }
            else
            {
                _mechanismTxt.text = string.Format(skill.MechanismTxtTemplate, beforePassive);
            }
            if (skill.CoolTime != 0) { _cooltimeTxt.text = $"ƒ≈∏¿”:{skill.CoolTime}√ "; }
            else { _cooltimeTxt.text = $"∆Ú≈∏:{skill.TriggerCount}»∏"; }
            _manaCostTxt.text = $"{skill.ManaCost}∏∂≥™";
        }
        if (_currSlot != null && _currSlot.Unlocked)
        {
            _equipButton.interactable = true;
            if (_currSlot.IsEquipped)
            {
                _equipButtonTxt.text = "«ÿ¡¶";
                _equipButton.image.color = Color.gray;
                _equipButton.onClick.RemoveAllListeners();
                _equipButton.onClick.AddListener(() =>
                    _eventChannel.RaiseEvent(EGameEventType.UnEquipRequest, new SlotPayload(_currSlot)));
            }
            else
            {
                _equipButtonTxt.text = "¿Â¬¯";
                _equipButton.image.color = _tempColor;
                _equipButton.onClick.RemoveAllListeners();
                _equipButton.onClick.AddListener(() =>
                    _eventChannel.RaiseEvent(EGameEventType.EquipRequest, new SlotPayload(_currSlot)));
            }
        }
        else
        {
            _equipButton.interactable = false;
            _equipButtonTxt.text = "¿·±Ë";
            _equipButton.image.color = Color.gray;
        }
        int cost = slot.GetUpgradeCost();
        _costTxt.text = $"{cost}";
    }
    public void Close() { gameObject.SetActive(false); }
}
