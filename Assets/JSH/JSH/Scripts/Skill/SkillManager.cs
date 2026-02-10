using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }
    private List<SkillInstance> _equippedSkills = new List<SkillInstance>();
    [SerializeField] private GameEventChannelSO _eventChannel;
    //나중에 수정 필요
    private PlayerStatManager _statManager;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        //나중에 수정 필요
        _statManager = PlayerStatManager.Instance;
    }
    private void OnEnable()
    {
        _eventChannel.OnEventRaised += HandleEvent;
    }
    private void OnDisable()
    {
        _eventChannel.OnEventRaised -= HandleEvent;
    }
    private void HandleEvent(EGameEventType eventType, object payload) 
    {
        if (eventType == EGameEventType.SlotUpdated || 
            eventType == EGameEventType.EquipRequest ||
            eventType == EGameEventType.EquipChanged) 
        {
            if(payload is InventorySlot slot && slot.BaseData is SkillDataSO)
                RefreshSlots();
        }
    }
    public void RefreshSlots() 
    {
        _equippedSkills.Clear();
        List<InventorySlot> equippedSlots = InventorySystem.Instance.GetEquippedSkills();

        foreach (var slot in equippedSlots) 
        {
            if (slot.BaseData is SkillDataSO skillData) 
            {
                SkillInstance instance = SkillFactory.CreateInstance(skillData);
                _equippedSkills.Add(instance);
            }
        }
    }
    private void Update()
    {
        foreach (var skill in _equippedSkills) 
        {
            //나중에 수정해야함
            if (skill.CanCast(_statManager.MaxMana)) 
            {
                skill.Cast(_statManager);
            }
        }
    }
    public void OnNormalAttack() 
    {
        foreach (var skill in _equippedSkills) { skill.OnNormalAttack(); }
    }
}
