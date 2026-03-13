using UnityEngine;

[CreateAssetMenu(fileName = "GachaData", menuName = "GameData/GachaData")]
public class GachaDataSO : ScriptableObject
{
    [System.Serializable]
    public class GradeChance
    {
        public GradeType gradeType;
        public float[] chances;
    }

    [Header("¹«±â & ¾Ç¼¼ Grade È®·ü")]
    public GradeChance[] itemGradeChanceTable;

    [Header("½ºÅ³ Grade È®·ü")]
    public GradeChance[] skillGradeChanceTable;

    [Header("¹«±â & ¾Ç¼¼ Tier È®·ü")]
    public int[] itemTierChanceTable;

    [Header("°¡Ã­ ·¹º§¾÷ Å×ÀÌºí")]
    public int[] gachaLevelTable;
}

