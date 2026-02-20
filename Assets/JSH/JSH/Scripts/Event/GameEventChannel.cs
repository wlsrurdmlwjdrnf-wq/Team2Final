using UnityEngine;
using UnityEngine.Events;

public enum EGameEventType 
{
    SlotUpdated,
    SlotClicked,
    UpgradeRequest,
    EquipRequest,
    UnEquipRequest,
    AutoCombine,
    GachaPull,
    SortInventory,
    CombineSlot,
    ButtonClicked,
    GachaRequest,
    EquipChanged,
    GachaProgressUpdate,
    GachaRequestEnd,
}

[CreateAssetMenu(fileName = "GameEventChannel", menuName = "Scriptable Objects/GameEventChannel")]
public class GameEventChannelSO : ScriptableObject
{
    public UnityAction<EGameEventType, object> OnEventRaised;

    public void RaiseEvent(EGameEventType type, object payload = null) 
    {
        OnEventRaised?.Invoke(type, payload);
    }

    public void RaiseButtonClicked() 
    {
        RaiseEvent(EGameEventType.ButtonClicked);
    }
}
