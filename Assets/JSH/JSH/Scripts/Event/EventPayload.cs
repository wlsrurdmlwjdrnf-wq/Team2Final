using System.Collections.Generic;

[System.Serializable]
public class InventoryEventPayload 
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

public class GachaRequestPayload 
{
    public EDataType Type;
    public int Count;

    public GachaRequestPayload(EDataType type, int count) 
    {
        Type = type;
        Count = count;
    }
}

public class GachaProgressPayload 
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