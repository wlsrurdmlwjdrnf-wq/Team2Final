using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PowerUpData
{
    public int atkLevel;
    public int hpLevel;
    public int hpRegenLevel;
    public int criDmgLevel;
    public int criRateLevel;
}
[System.Serializable]
public class GrowUpData
{
    public int strLevel;
    public int hpLevel;
    public int vitLevel;
    public int criLevel;
    public int lukLevel;
}

public class CharacterUpgradeSaveManager : Singleton<CharacterUpgradeSaveManager>
{
    // 세이브나 로드할 데이터 담아둘 변수
    private PowerUpModel _puModel;
    private GrowUpModel _guModel;

    public PowerUpModel PowerUpModel 
    {
        get { return _puModel; }
        private set { _puModel = value; } 
    }
    public GrowUpModel GrowUpModel
    {
        get { return _guModel; }
        private set { _guModel = value; }
    }

    // 강화레벨 저장 및 로드, 성장레벨 저장 및 로드
    public string SavePowerUpData()
    {
        _puModel = PowerUpPresenter.GetModel();
        PowerUpData saveData = new PowerUpData
        {
            atkLevel = _puModel.AtkLevel,
            hpLevel = _puModel.HpLevel,
            hpRegenLevel = _puModel.HpRegenLevel,
            criDmgLevel = _puModel.CriDmgLevel,
            criRateLevel = _puModel.CriRateLevel
        };
        string json = JsonUtility.ToJson(saveData);
        return json;
    }
    public bool LoadPowerUpData(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;

        try
        {
            PowerUpData loadedData = JsonUtility.FromJson<PowerUpData>(json);

            _puModel = new PowerUpModel(
                loadedData.atkLevel,
                loadedData.hpLevel,
                loadedData.hpRegenLevel,
                loadedData.criDmgLevel,
                loadedData.criRateLevel
                );

            Debug.Log("강화레벨 로드 성공!");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"강화레벨 로드 실패: {e.Message}");
            return false;
        }
    }
    public string SaveGrowUpData()
    {
        GrowUpModel = GrowUpPresenter.GetModel();
        GrowUpData saveData = new GrowUpData
        {
            strLevel = GrowUpModel.StrLevel,
            hpLevel = GrowUpModel.HpLevel,
            vitLevel = GrowUpModel.VitLevel,
            criLevel = GrowUpModel.CriLevel,
            lukLevel = GrowUpModel.LukLevel
        };
        string json = JsonUtility.ToJson(saveData);
        return json;
    }
    public bool LoadGrowUpData(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return false;

        try
        {
            GrowUpData loadedData = JsonUtility.FromJson<GrowUpData>(json);

            _puModel = new PowerUpModel(
                loadedData.strLevel,
                loadedData.hpLevel,
                loadedData.vitLevel,
                loadedData.criLevel,
                loadedData.lukLevel
                );

            Debug.Log("성장레벨 로드 성공!");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"성장레벨 로드 실패: {e.Message}");
            return false;
        }
    }
}
