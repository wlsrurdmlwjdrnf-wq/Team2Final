using UnityEngine;

public class ItemInstance : IUpgradable
{
    public ItemDataSO baseData;   
    public int currentLevel;      
    public float currentEffectValue = 10;
    public ItemInstance(ItemDataSO data)
    {
        baseData = data;
        currentLevel = data.Level; 
        //currentEffectValue = data.effectValue;
    }
    public int Level => currentLevel;
    /*
    public void Upgrade()
    {
        currentLevel++;
        currentEffectValue = CalculateEffect(baseData, currentLevel);
    }
    
    private float CalculateEffect(ItemDataSO data, int level)
    {
        return data.effectValue + level * 10f;
    }
    */
}
