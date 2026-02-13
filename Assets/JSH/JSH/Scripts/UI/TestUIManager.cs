using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TestUIManager : Singleton<TestUIManager>
{
    [SerializeField] private TestSlotUI _slotUIPrefab;
    [SerializeField] private Transform _weaponContent;
    [SerializeField] private Transform _accessoryContent;
    [SerializeField] private Transform _skillContent;
    [SerializeField] private Transform _equippedSkillContent;
    [SerializeField] private TestSlotUI _equippedWeaponSlot;
    [SerializeField] private TestSlotUI _equippedAccessorySlot;
    [SerializeField] private TextMeshProUGUI _weaponGachaText;
    [SerializeField] private TextMeshProUGUI _accessoryGachaText;
    private List<TestSlotUI> _skillSlots = new List<TestSlotUI>();

    [SerializeField] private GameEventChannelSO _eventChannel;
    //²ô°íÅ°´Â¿ë
    public List<GameObject> Inventories = new List<GameObject>();
    private void Start()
    {
        PoolManager.Instance.CreatePool(_slotUIPrefab, 120, null);
        Initialize();
    }
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
        var payloadData = payload; 
        switch (type) 
        {
            case EGameEventType.SlotUpdated:
                if (payloadData is InventorySlot updateSlot)
                {
                    var slotUIs = _weaponContent.GetComponentsInChildren<TestSlotUI>(true);
                    slotUIs = slotUIs.Concat(_accessoryContent.GetComponentsInChildren<TestSlotUI>(true)).ToArray();
                    slotUIs = slotUIs.Concat(_skillContent.GetComponentsInChildren<TestSlotUI>(true)).ToArray();

                    foreach (var ui in slotUIs)
                    {
                        if (ui.Slot.Id == updateSlot.Id)
                        {
                            ui.SetUp(updateSlot);
                            break;
                        }
                    }
                }
                break;
            case EGameEventType.EquipChanged:
            case EGameEventType.EquipRequest:
                var equippedWeapon = InventorySystem.Instance.GetEquippedWeapon();
                if (equippedWeapon != null) _equippedWeaponSlot.SetUp(equippedWeapon);
                else _equippedWeaponSlot.SetEmpty();

                var equippedAccessory = InventorySystem.Instance.GetEquippedAccessory();
                if (equippedAccessory != null) _equippedAccessorySlot.SetUp(equippedAccessory);
                else _equippedAccessorySlot.SetEmpty();

                RefreshEquippedSkills();
                break;
            case EGameEventType.GachaProgressUpdate:
                if (payloadData is GachaProgressPayload gachaData) 
                {
                    if (gachaData.Type == EDataType.Weapon)
                    {
                        _weaponGachaText.text = $"Lv.{gachaData.Level} {gachaData.CurrCount}/{gachaData.LevelUpCount}";
                    }
                    else if (gachaData.Type == EDataType.Accessories) 
                    {
                        _accessoryGachaText.text = $"Lv.{gachaData.Level} {gachaData.CurrCount}/{gachaData.LevelUpCount}";
                    }
                }
                break ;
        }
    }

    public void Initialize()
    {
        var weaponInventory = InventorySystem.Instance.GetInventory(EDataType.Weapon);
        foreach (var slot in weaponInventory) 
        {
            TestSlotUI slotUI = PoolManager.Instance.GetFromPool(_slotUIPrefab);
            slotUI.transform.SetParent(_weaponContent, false);
            slotUI.SetUp(slot);
        }
        var accessoryInventory = InventorySystem.Instance.GetInventory(EDataType.Accessories);
        foreach (var slot in accessoryInventory)
        {
            TestSlotUI slotUI = PoolManager.Instance.GetFromPool(_slotUIPrefab);
            slotUI.transform.SetParent(_accessoryContent, false);
            slotUI.SetUp(slot);
        }
        var skillInventory = InventorySystem.Instance.GetInventory(EDataType.Skill);
        foreach (var slot in skillInventory)
        {
            TestSlotUI slotUI = PoolManager.Instance.GetFromPool(_slotUIPrefab);
            slotUI.transform.SetParent(_skillContent, false);
            slotUI.SetUp(slot);
        }
        PopUp(Inventories[0]);
    }
    public void PopUp(GameObject popUp)
    {
        foreach (var i in Inventories) 
        {
            i.SetActive(false);
        }
        popUp.SetActive(true);
    }
    private void RefreshEquippedSkills() 
    {
        foreach (TestSlotUI slotUI in _skillSlots) slotUI.ReturnPool();

        var equippedSkills = InventorySystem.Instance.GetEquippedSkills();

        foreach (var slot in equippedSkills) 
        {
            TestSlotUI slotUI = PoolManager.Instance.GetFromPool(_slotUIPrefab);
            slotUI.transform.SetParent(_equippedSkillContent, false);
            slotUI.SetUp(slot);
            _skillSlots.Add(slotUI);
        }
    }
}
