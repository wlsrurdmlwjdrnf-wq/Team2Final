using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesModel
{
    public static event Action<ResourceType, BigNumber> OnResourceChanged;

    public static void TriggerResourceChange(ResourceType type, BigNumber bn)
    {
        OnResourceChanged?.Invoke(type, bn);
    }
}
