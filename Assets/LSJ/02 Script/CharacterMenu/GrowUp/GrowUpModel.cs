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

    public int MaxStrLevel { get; private set; } = 1000;
    public int MaxHpLevel { get; private set; } = 1000;
    public int MaxVitLevel { get; private set; } = 1000;
    public int MaxCriLevel { get; private set; } = 200;
    public int MaxLukLevel { get; private set; } = 1000;
    public GrowUpModel()
    {
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
    public GrowUpModel(GrowUpModel model)
    {
        StrLevel = model.StrLevel;
        HpLevel = model.HpLevel;
        VitLevel = model.VitLevel;
        CriLevel = model.CriLevel;
        LukLevel = model.LukLevel;
    }
    public GrowUpModel(int a, int b, int c, int d, int e)
    {
        StrLevel = a;
        HpLevel = b;
        VitLevel = c;
        CriLevel = d;
        LukLevel = e;
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
