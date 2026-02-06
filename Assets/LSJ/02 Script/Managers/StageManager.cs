using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    public int CurrentMainNumber { get; private set; }
    public int CurrentSubNumber { get; private set; }

    public string CurrentStageToString()
    {
        string curStage = $"{CurrentMainNumber} - {CurrentSubNumber}";
        return curStage;
    }
    public void SetStage(StageRoot stage)
    {
        CurrentMainNumber = stage.MainNumber;
        CurrentSubNumber = stage.SubNumber;
    }
}
