using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BestStageData
{
    public int bestMainNumber;
    public int bestSubNumber;
    public int bestAdventureNumber;
}

public class StageRecordDataManager : Singleton<StageRecordDataManager>
{
    private int _bestMainNumber = 1;
    private int _bestSubNumber = 1;
    private int _bestAdventureNumber = 0;

    public int MainNumber {  get { return _bestMainNumber; } set { _bestMainNumber = value; }}
    public int SubNumber { get { return _bestSubNumber; } set { _bestSubNumber = value; }}
    public int AdventureNumber { get {return _bestAdventureNumber; } set {_bestAdventureNumber = value; }}

    public string SaveStageData()
    {
        BestStageData saveData = StageManager.Instance.GetRecord();

        string json = JsonUtility.ToJson(saveData);
        return json;
    }
    public bool LoadStageData(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;

        try
        {
            BestStageData loadedData = JsonUtility.FromJson<BestStageData>(json);

            _bestMainNumber = loadedData.bestMainNumber;
            _bestSubNumber = loadedData.bestSubNumber;
            _bestAdventureNumber = loadedData.bestAdventureNumber;

            Debug.Log("스테이지 데이터 로드 성공!");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"스테이지 데이터 로드 실패: {e.Message}");
            return false;
        }
    }
}
