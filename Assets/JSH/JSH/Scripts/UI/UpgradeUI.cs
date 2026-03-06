using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO _eventChannel;
    [Header("Upgrade")]
    [SerializeField] private TextMeshProUGUI _equipStatsTxt;
    [SerializeField] private TextMeshProUGUI _passiveStatsTxt;
    [SerializeField] private TextMeshProUGUI _costTxt;
    [SerializeField] private Button _upgradePanelButton;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _equipButton;
    [SerializeField] private TextMeshProUGUI _equipButtonTxt;
    [Header("Combine")]
    [SerializeField] private Button _combinePanelButton;
    [SerializeField] private Button _combineButton;
    [SerializeField] private GameObject _combinePanel;
    [SerializeField] private TestSlotUI _beforeSlotUI;
    [SerializeField] private TestSlotUI _afterSlotUI;
    [SerializeField] private Slider _combineSlider;

    [SerializeField] private TestSlotUI _presentSlotUI;
    private InventorySlot _currSlot;
    private Color _tempColor;
    private void Awake()
    {
        _eventChannel.OnEventRaised += HandleEvent;
        gameObject.SetActive(false);
        _tempColor = _equipButton.image.color;
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
        if (!slot.Unlocked) return;
        _combinePanel.SetActive(false);
        if (slot.GetDataType() == EDataType.Skill) { return; }
        _currSlot = slot;
        gameObject.SetActive(true);

        _upgradeButton.onClick.RemoveAllListeners();
        _upgradeButton.onClick.AddListener(() => {
            _eventChannel.RaiseEvent(EGameEventType.UpgradeRequest, new SlotPayload(_currSlot));
        });

        RefreshText(slot);
    }
    private void RefreshUI(InventorySlot slot)
    {
        if(_currSlot != null && slot.Id != _currSlot.Id) return;
        RefreshText(slot);
    }
    private void RefreshText(InventorySlot slot) 
    {
        float beforeActive = slot.ActiveEffectValue;
        float beforePassive = slot.PassiveEffectValue;

        float afterActive = slot.CalculateEffect(0, slot.Level + 1);
        float afterPassive = slot.CalculateEffect(1, slot.Level + 1);

        _presentSlotUI.SetEmpty();
        if (_currSlot != null) _presentSlotUI.SetUp(_currSlot);

        if (slot.BaseData is ItemDataSO item)
        {
            _equipStatsTxt.text = $"장착효과\n{item.EquipStat}{beforeActive} > {afterActive}";
            _passiveStatsTxt.text = $"보유효과\n{item.PassiveStat}{beforePassive} > {afterPassive}";
        }
        else if (slot.BaseData is SkillDataSO skill)
        {
            _equipStatsTxt.text = $"스킬배율\n{beforeActive} > {afterActive}";
            _passiveStatsTxt.text = $"장착패시브\n{beforePassive} > {afterPassive}";
        }
        if (_currSlot != null && _currSlot.IsEquipped)
        {
            _equipButtonTxt.text = "해제";
            _equipButton.image.color = Color.gray;
            _equipButton.onClick.RemoveAllListeners();
            _equipButton.onClick.AddListener(() =>
                _eventChannel.RaiseEvent(EGameEventType.UnEquipRequest, new SlotPayload(_currSlot)));
        }
        else
        {
            _equipButtonTxt.text = "장착";
            _equipButton.image.color = _tempColor;
            _equipButton.onClick.RemoveAllListeners();
            _equipButton.onClick.AddListener(() =>
                _eventChannel.RaiseEvent(EGameEventType.EquipRequest, new SlotPayload(_currSlot)));
        }
        int cost = slot.GetUpgradeCost();
        _costTxt.text = $"{cost}";
    }
    public void OpenCombinePanel() 
    { 
        _combinePanel.SetActive(true);
        
        _beforeSlotUI.SetEmpty();
        _beforeSlotUI.SetUp(_currSlot);
        _afterSlotUI.SetEmpty();
        _afterSlotUI.SetUp(InventorySystem.instance.GetNextSlot(_currSlot));

        _combineSlider.maxValue = _currSlot.Stack / PublicConst.UpgradeStack;
        _combineSlider.value = 0f;

        Color temp = _combinePanelButton.image.color;
        _combinePanelButton.image.color = _upgradePanelButton.image.color;
        _upgradePanelButton.image.color = temp;
    }
    public void DoCombine() 
    { 
        InventorySystem.Instance.CombineCount((int)_combineSlider.value, _currSlot);
        OpenCombinePanel();
    }
    public void CloseCombinePanel() 
    { 
        _combinePanel.SetActive(false);
        Color temp = _combinePanelButton.image.color;
        _combinePanelButton.image.color = _upgradePanelButton.image.color;
        _upgradePanelButton.image.color = temp;
    }
    public void Close() { gameObject.SetActive(false); }
}
