using System.Collections.Generic;
using UnityEngine;

public class TestUIManager : MonoBehaviour
{
    public static TestUIManager Instance { get; private set; }
    public TestSlotUI SlotUIPrefab;
    public Transform WeaponContent;
    public Transform AccessoryContent;
    public Transform SkillContent;
    //²ô°íÅ°´Â¿ë
    public List<GameObject> Inventories = new List<GameObject>();
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        PoolManager.Instance.CreatePool(SlotUIPrefab, 120, null);
      
    }
    public void Initialize()
    {
        var weaponInventory = InventorySystem.Instance.GetInventory(EDataType.Weapon);
        foreach (var slot in weaponInventory) 
        {
            TestSlotUI slotUI = PoolManager.Instance.GetFromPool(SlotUIPrefab);
            if (slotUI == null) Debug.LogWarning("Slot Null!!");
            slotUI.SetUp(slot);
        }
        var accessoryInventory = InventorySystem.Instance.GetInventory(EDataType.Accessories);
        foreach (var slot in accessoryInventory)
        {
            TestSlotUI slotUI = PoolManager.Instance.GetFromPool(SlotUIPrefab);
            slotUI.SetUp(slot);
        }
        var skillInventory = InventorySystem.Instance.GetInventory(EDataType.Skill);
        foreach (var slot in skillInventory)
        {
            TestSlotUI slotUI = PoolManager.Instance.GetFromPool(SlotUIPrefab);
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
