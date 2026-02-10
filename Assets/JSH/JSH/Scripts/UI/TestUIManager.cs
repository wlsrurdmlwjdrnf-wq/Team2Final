using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestUIManager : MonoBehaviour
{
    public static TestUIManager Instance { get; private set; }
    public TestSlotUI SlotUIPrefab;
    public Transform WeaponContent;
    public Transform AccessoryContent;
    public Transform SkillContent;

    [SerializeField] private GameEventChannelSO _eventChannel;
    //²ô°íÅ°´Â¿ë
    public List<GameObject> Inventories = new List<GameObject>();
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        PoolManager.Instance.CreatePool(SlotUIPrefab, 120, null);
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
        if (type == EGameEventType.SlotUpdated && payload is InventorySlot updateSlot)
        {
            var slotUIs = WeaponContent.GetComponentsInChildren<TestSlotUI>(true);
            slotUIs = slotUIs.Concat(AccessoryContent.GetComponentsInChildren<TestSlotUI>(true)).ToArray();
            slotUIs = slotUIs.Concat(SkillContent.GetComponentsInChildren<TestSlotUI>(true)).ToArray();

            foreach (var ui in slotUIs)
            {
                if (ui.Slot.Id == updateSlot.Id)
                {
                    ui.SetUp(updateSlot);
                    break;
                }
            }
        }
    }

    public void Initialize()
    {
        var weaponInventory = InventorySystem.Instance.GetInventory(EDataType.Weapon);
        foreach (var slot in weaponInventory) 
        {
            TestSlotUI slotUI = PoolManager.Instance.GetFromPool(SlotUIPrefab);
            slotUI.transform.SetParent(WeaponContent, false);
            slotUI.SetUp(slot);
        }
        var accessoryInventory = InventorySystem.Instance.GetInventory(EDataType.Accessories);
        foreach (var slot in accessoryInventory)
        {
            TestSlotUI slotUI = PoolManager.Instance.GetFromPool(SlotUIPrefab);
            slotUI.transform.SetParent(AccessoryContent, false);
            slotUI.SetUp(slot);
        }
        var skillInventory = InventorySystem.Instance.GetInventory(EDataType.Skill);
        foreach (var slot in skillInventory)
        {
            TestSlotUI slotUI = PoolManager.Instance.GetFromPool(SlotUIPrefab);
            slotUI.transform.SetParent(SkillContent, false);
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
}
