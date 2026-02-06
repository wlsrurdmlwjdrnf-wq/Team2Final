
public static class SkillFactory
{
    static SkillVFXDatabaseSO _vfxDB = ItemSkillDataManager.Instance.SkillVFXDatabase;
    public static SkillInstance CreateInstance(SkillDataSO data) 
    {
        ISkillEffect effect = null;
        switch (data.SkillType) 
        {
            //юс╫ц
            case ESkillEffectType.S1:
                effect = new TestSkill_1(_vfxDB.GetVFX(ESkillEffectType.S1));
                break;
            case ESkillEffectType.S2:
                effect = new TestSkill_2(_vfxDB.GetVFX(ESkillEffectType.S2));
                break;
            case ESkillEffectType.S3:
                effect = new TestSkill_3(_vfxDB.GetVFX(ESkillEffectType.S3));
                break;
            case ESkillEffectType.S4:
                effect = new TestSkill_4(_vfxDB.GetVFX(ESkillEffectType.S4));
                break;
            default:
                effect = new TestSkill_1(_vfxDB.GetVFX(ESkillEffectType.S1));
                break;
        }
        return new SkillInstance(data, effect);
    }
}
