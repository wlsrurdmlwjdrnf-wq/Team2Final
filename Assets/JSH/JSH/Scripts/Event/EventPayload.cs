using System.Collections.Generic;
using Unity.VisualScripting;


public interface IGmeEventPayload { }
[System.Serializable]
public class InventoryEventPayload : IGmeEventPayload
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

public class GachaRequestPayload : IGmeEventPayload
{
    public EDataType Type;
    public int Count;

    public GachaRequestPayload(EDataType type, int count) 
    {
        Type = type;
        Count = count;
    }
}

public class GachaProgressPayload : IGmeEventPayload
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

public class EDataTypePayload : IGmeEventPayload 
{
    public EDataType Type;
    public EDataTypePayload(EDataType type) { Type = type; }
}
public class SlotPayload : IGmeEventPayload
{
    public InventorySlot Slot;
    public SlotPayload(InventorySlot slot) { Slot = slot; }
}