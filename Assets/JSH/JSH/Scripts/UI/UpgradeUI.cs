using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO _eventChannel;
    [SerializeField] private TextMeshProUGUI _beforeStatsTxt;
    [SerializeField] private TextMeshProUGUI _afterStatsTxt;
    [SerializeField] private TextMeshProUGUI _costTxt;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _equipButton;

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
            }
        }
    }
    private void OpenPopUp(InventorySlot slot) 
    {
        _currSlot = slot;
        gameObject.SetActive(true);

        float beforeActive = slot.CalculateEffect(0, slot.Level);
        float beforePassive = slot.CalculateEffect(1, slot.Level);

        float afterActive = slot.CalculateEffect(0, slot.Level + 1);
        float afterPassive = slot.CalculateEffect(1, slot.Level + 1);

        _beforeStatsTxt.text = $"active:{beforeActive}, passive:{beforePassive}";
        _afterStatsTxt.text = $"active:{afterActive}, passive:{afterPassive}";

        int cost = slot.GetUpgradeCost();
        _costTxt.text = $"Cost:{cost}";

        _upgradeButton.onClick.RemoveAllListeners();
        _equipButton.onClick.RemoveAllListeners();

        _upgradeButton.onClick.AddListener(() => 
            _eventChannel.RaiseEvent(EGameEventType.UpgradeRequest, _currSlot));

        _equipButton.onClick.AddListener(() => 
            _eventChannel.RaiseEvent(EGameEventType.EquipRequest, _currSlot));
    }
    private void RefreshUI(InventorySlot slot)
    {
    float beforeActive = slot.CalculateEffect(0, slot.Level);
    float beforePassive = slot.CalculateEffect(1, slot.Level);

    float afterActive = slot.CalculateEffect(0, slot.Level + 1);
    float afterPassive = slot.CalculateEffect(1, slot.Level + 1);

    _beforeStatsTxt.text = $"active:{beforeActive}, passive:{beforePassive}";
    _afterStatsTxt.text = $"active:{afterActive}, passive:{afterPassive}";

    int cost = slot.GetUpgradeCost();
    _costTxt.text = $"Cost:{cost}";
    }

    public void Close() { gameObject.SetActive(false); }
}
