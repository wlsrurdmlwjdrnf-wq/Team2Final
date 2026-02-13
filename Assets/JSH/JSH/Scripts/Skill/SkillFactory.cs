
public static class SkillFactory
{
    static SkillVFXDatabaseSO _vfxDB = ItemSkillDataManager.Instance.SkillVFXDatabase;

    public static SkillInstance CreateInstance(InventorySlot slot)
    {
        ISkillEffect effect = null;
        ESkillEffectType skillType = ESkillEffectType.S1;
        if (slot.BaseData is SkillDataSO skill) skillType = skill.SkillType;
        switch (skillType)
        {
            //юс╫ц
            case ESkillEffectType.S1:
                effect = new TestSkill_1(_vfxDB.GetVFX(ESkillEffectType.S1), ESkillEffectType.S1);
                break;
            case ESkillEffectType.S2:
                effect = new TestSkill_2(_vfxDB.GetVFX(ESkillEffectType.S2), ESkillEffectType.S2);
                break;
            case ESkillEffectType.S3:
                effect = new TestSkill_3(_vfxDB.GetVFX(ESkillEffectType.S3), ESkillEffectType.S3);
                break;
            case ESkillEffectType.S4:
                effect = new TestSkill_4(_vfxDB.GetVFX(ESkillEffectType.S4), ESkillEffectType.S4);
                break;
            case ESkillEffectType.Lightning:
                effect = new Lightning(_vfxDB.GetVFX(ESkillEffectType.Lightning), ESkillEffectType.Lightning);
                break;
            default:
                effect = new TestSkill_1(_vfxDB.GetVFX(ESkillEffectType.S1), ESkillEffectType.S1);
                break;
        }
        return new SkillInstance(slot, effect);
    }
}
