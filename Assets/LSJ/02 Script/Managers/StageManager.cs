using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    private StageRoot _currentStage;
    public int CurrentMainNumber { get; private set; }
    public int CurrentSubNumber { get; private set; }
    public int MonsterCount { get; set; }


    // 현재 스테이지 문자열 리턴
    public string CurrentStageToString()
    {
        string curStage = $"{CurrentMainNumber} - {CurrentSubNumber}";
        return curStage;
    }
    public void SetStage(StageRoot stage)
    {
        _currentStage = stage;
        CurrentMainNumber = stage.MainNumber;
        CurrentSubNumber = stage.SubNumber;

        //PoolManager2.Instance.Get(stage.gameObject);
    }
}
