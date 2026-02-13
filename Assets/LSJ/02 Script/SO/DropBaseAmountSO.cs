using UnityEngine;

[CreateAssetMenu(menuName = "GameData/Drop/AmountData",fileName = "DropBaseAmountData")]
public class DropBaseAmountSO : ScriptableObject
{
    public float baseExpAmount = 10f;
    public float baseGoldAmount = 10f;
    public float baseEnhancementCubeAmount = 10f;
}
