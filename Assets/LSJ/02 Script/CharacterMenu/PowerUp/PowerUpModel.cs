using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public PowerUpModel(PowerUpModel model)
    {
        AtkLevel = model.AtkLevel;
        HpLevel = model.HpLevel;
        HpRegenLevel= model.HpRegenLevel;
        CriDmgLevel = model.CriDmgLevel;
        CriRateLevel= model.CriRateLevel;
    }
    public PowerUpModel(int a, int b, int c, int d, int e)
    {
        AtkLevel= a;
        HpLevel= b;
        HpRegenLevel= c;
        CriDmgLevel= d;
        CriRateLevel= e;
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
