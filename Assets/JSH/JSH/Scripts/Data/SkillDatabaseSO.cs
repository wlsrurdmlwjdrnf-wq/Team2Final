using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDatabaseSO", menuName = "Database/SkillDatabaseSO")]
public class SkillDatabaseSO : ScriptableObject
{
    public List<SkillDataSO> skills;
}

