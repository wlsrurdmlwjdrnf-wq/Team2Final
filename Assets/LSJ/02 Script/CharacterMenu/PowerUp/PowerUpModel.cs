using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpModel
{
    public int AtkLevel { get; private set; }
    public int HpLevel { get; private set; }
    public int HpRegenLevel { get; private set; }
    public int CriDmgLevel { get; private set; }
    public int CriRateLevel { get; private set; }
    public PowerUpModel()
    {
        // TODO : CharacterUpgradeSaveManager에서 데이터 불러오기
        // 데이터 없으면 초기화
        Initialize();
    }
    private void Initialize()
    {
        AtkLevel = 0;
        HpLevel = 0;
        HpRegenLevel = 0;
        CriDmgLevel = 0;
        CriRateLevel = 0;
    }
    public void AddAtkLevel()
    {
        AtkLevel++;
    }
    public void AddHpLevel()
    {
        HpLevel++;
    }
    public void AddHpRegenLevel()
    {
        HpRegenLevel++;
    }
    public void AddCriDmgLevel()
    {
        CriDmgLevel++;
    }
    public void AddCriRateLevel()
    {
        CriRateLevel++;
    }

}
