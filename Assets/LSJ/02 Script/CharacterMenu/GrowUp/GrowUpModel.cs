using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowUpModel
{
    public int StrLevel { get; private set; }
    public int HpLevel { get; private set; }
    public int VitLevel { get; private set; }
    public int CriLevel { get; private set; }
    public int LukLevel { get; private set; }
    public GrowUpModel()
    {
        // TODO : CharacterUpgradeSaveManager에서 데이터 불러오기
        // 데이터 없으면 초기화
        Initialize();
    }
    private void Initialize()
    {
        StrLevel = 0;
        HpLevel = 0;
        VitLevel = 0;
        CriLevel = 0;
        LukLevel = 0;
    }
    public void AddStrLevel()
    {
        StrLevel++;
    }
    public void AddHpLevel()
    {
        HpLevel++;
    }
    public void AddVitLevel()
    {
        VitLevel++;
    }
    public void AddCriLevel()
    {
        CriLevel++;
    }
    public void AddLukLevel()
    {
        LukLevel++;
    }

}
