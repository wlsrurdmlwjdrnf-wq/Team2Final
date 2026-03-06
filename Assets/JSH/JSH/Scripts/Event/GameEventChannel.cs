using UnityEngine;
using UnityEngine.Events;

public enum EGameEventType 
{
    SlotUpdated,
    SlotClicked,
    UpgradeRequest,
    EquipRequest,//
    UnEquipRequest,//
    AutoCombine,
    GachaPull,
    SortInventory,
    CombineSlot,
    ButtonClicked,
    GachaRequest,
    EquipChanged,//
    GachaProgressUpdate,
    GachaRequestEnd,
    SkillUsed,//
    DamageDealt,//
    VolumeBGMUpdate,//
    VolumeSFXUpdate,//
    VolumeBGMMuteToggle,//
    VolumeSFXMuteToggle,//
    RequestSkillUse,
    RequestAddSkillSlot,
    CloseUpgradeUI,
    RequestChangeSkill,
}

[CreateAssetMenu(fileName = "GameEventChannel", menuName = "Scriptable Objects/GameEventChannel")]
public class GameEventChannelSO : ScriptableObject
{
    public UnityAction<EGameEventType, IGameEventPayload> OnEventRaised;

    public void RaiseEvent(EGameEventType type, IGameEventPayload payload = null) 
    {
        OnEventRaised?.Invoke(type, payload);
    }

    public void RaiseButtonClicked() 
    {
        RaiseEvent(EGameEventType.ButtonClicked);
    }
}
