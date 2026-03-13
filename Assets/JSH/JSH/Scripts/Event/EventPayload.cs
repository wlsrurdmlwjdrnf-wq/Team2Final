public interface IGameEventPayload { }
[System.Serializable]
public class InventoryEventPayload : IGameEventPayload
{
    public EDataType Type;
    public int SlotIndex;
    public object ExtraData;

    public InventoryEventPayload(EDataType type, int slotIndex, object extraData)
    {
        Type = type;
        SlotIndex = slotIndex;
        ExtraData = extraData;
    }
}

public class GachaRequestPayload : IGameEventPayload
{
    public EDataType Type;
    public int Count;

    public GachaRequestPayload(EDataType type, int count) 
    {
        Type = type;
        Count = count;
    }
}

public class GachaProgressPayload : IGameEventPayload
{
    public EDataType Type;
    public int CurrCount;
    public int LevelUpCount;
    public int Level;

    public GachaProgressPayload(EDataType type, int currCount, int levelUpCount, int level)
    {
        Type = type;
        CurrCount = currCount;
        LevelUpCount = levelUpCount;
        Level = level;
    }
}

public class EDataTypePayload : IGameEventPayload
{
    public EDataType Type;
    public EDataTypePayload(EDataType type) { Type = type; }
}
public class SlotPayload : IGameEventPayload
{
    public InventorySlot Slot;
    public SlotPayload(InventorySlot slot) { Slot = slot; }
}

public class VolumeUpdatePayload : IGameEventPayload
{
    public float volume;
}

public class MutePayload : IGameEventPayload
{
    public bool isMuted;
}