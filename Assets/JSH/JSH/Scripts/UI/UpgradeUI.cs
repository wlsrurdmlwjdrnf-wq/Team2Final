using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO _eventChannel;
    [Header("Upgrade")]
    [SerializeField] private TextMeshProUGUI _beforeStatsTxt;
    [SerializeField] private TextMeshProUGUI _afterStatsTxt;
    [SerializeField] private TextMeshProUGUI _costTxt;
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

    private InventorySlot _currSlot;

    private void Awake()
    {
        _eventChannel.OnEventRaised += HandleEvent;
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        _eventChannel.OnEventRaised -= HandleEvent;
    }
    private void HandleEvent(EGameEventType type, object payload) 
    {
        if (payload is InventorySlot slot)
        {
            switch (type)
            {
                case EGameEventType.SlotClicked:
                    OpenPopUp(slot);
                    break;
                case EGameEventType.SlotUpdated:
                    RefreshUI(slot);
                    break;
                case EGameEventType.EquipChanged:
                    RefreshUI(slot);
                    break;
            }
        }
    }
    private void OpenPopUp(InventorySlot slot) 
    {
        if (!slot.Unlocked) return;
        CloseCombinePanel();
        if (slot.GetDataType() == EDataType.Skill) { _combinePanelButton.gameObject.SetActive(false); }
        else _combinePanelButton.gameObject.SetActive(true);
        _currSlot = slot;
        gameObject.SetActive(true);

        float beforeActive = slot.CalculateEffect(0, slot.Level);
        float beforePassive = slot.CalculateEffect(1, slot.Level);

        float afterActive = slot.CalculateEffect(0, slot.Level + 1);
        float afterPassive = slot.CalculateEffect(1, slot.Level + 1);

        if (slot.BaseData is ItemDataSO item)
        {
            _beforeStatsTxt.text = $"장착효과:{item.EquipStat} - {beforeActive}\n보유효과:{item.PassiveStat} - {beforePassive}";
            _afterStatsTxt.text = $"장착효과:{item.EquipStat} - {afterActive}\n보유효과:{item.PassiveStat} - {afterPassive}";
        }
        else if (slot.BaseData is SkillDataSO skill)
        {
            _beforeStatsTxt.text = $"데미지배율:{skill.Damage} - {beforeActive}\n패시브:{skill.Stat} - {beforePassive}";
            _afterStatsTxt.text = $"데미지배율:{skill.Damage} - {afterActive}\n패시브:{skill.Stat} - {afterPassive}";
        }

        int cost = slot.GetUpgradeCost();
        _costTxt.text = $"Cost:{cost}";

        _upgradeButton.onClick.RemoveAllListeners();
        _equipButton.onClick.RemoveAllListeners();

        _upgradeButton.onClick.AddListener(() => 
            _eventChannel.RaiseEvent(EGameEventType.UpgradeRequest, _currSlot));

        EquipButtonText();
    }
    private void RefreshUI(InventorySlot slot)
    {
        if(_currSlot != null && slot.Id != _currSlot.Id) return;

        float beforeActive = slot.CalculateEffect(0, slot.Level);
        float beforePassive = slot.CalculateEffect(1, slot.Level);

        float afterActive = slot.CalculateEffect(0, slot.Level + 1);
        float afterPassive = slot.CalculateEffect(1, slot.Level + 1);

        if (slot.BaseData is ItemDataSO item)
        {
            _beforeStatsTxt.text = $"장착효과:{item.EquipStat}{beforeActive}, 보유효과:{item.PassiveStat}{beforePassive}";
            _afterStatsTxt.text = $"장착효과:{item.EquipStat}{afterActive}, 보유효과:{item.PassiveStat}{afterPassive}";
        }
        else  if (slot.BaseData is SkillDataSO skill)
        {
            _beforeStatsTxt.text = $"데미지배율:{skill.Damage}{beforeActive}, 패시브:{skill.Stat}{beforePassive}";
            _afterStatsTxt.text = $"데미지배율:{skill.Damage}{afterActive}, 패시브:{skill.Stat}{afterPassive}";
        }

        int cost = slot.GetUpgradeCost();
        _costTxt.text = $"Cost:{cost}";

        EquipButtonText();
    }
    private void EquipButtonText() 
    {
        if (_currSlot != null && _currSlot.IsEquipped)
        {
            _equipButtonTxt.text = "Unequip";
            _equipButton.onClick.AddListener(() =>
                _eventChannel.RaiseEvent(EGameEventType.UnEquipRequest, _currSlot));
        }
        else
        {
            _equipButtonTxt.text = "Equip";
            _equipButton.onClick.AddListener(() =>
                _eventChannel.RaiseEvent(EGameEventType.EquipRequest, _currSlot));
        }
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
    }
    public void DoCombine() 
    { 
        InventorySystem.Instance.CombineCount((int)_combineSlider.value, _currSlot);
        OpenCombinePanel();
    }
    public void CloseCombinePanel() { _combinePanel.SetActive(false); }
    public void Close() { gameObject.SetActive(false); }
}
