
public static class SkillFactory
{
    static SkillVFXDatabaseSO _vfxDB = ItemSkillDataManager.Instance.SkillVFXDatabase;

    public static SkillInstance CreateInstance(InventorySlot slot)
    {
        ISkillEffect effect = null;
        ESkillEffectType skillType = ESkillEffectType.FireExplosion;
        if (slot.BaseData is SkillDataSO skill) skillType = skill.SkillType;
        switch (skillType)
        {
            //юс╫ц
            case ESkillEffectType.FireExplosion:
                effect = new FireExplosion(_vfxDB.GetVFX(ESkillEffectType.FireExplosion), ESkillEffectType.FireExplosion);
                break;
            case ESkillEffectType.EarthGrow:
                effect = new EarthGrow(_vfxDB.GetVFX(ESkillEffectType.EarthGrow), ESkillEffectType.EarthGrow);
                break;
            case ESkillEffectType.WindGust:
                effect = new WindGust(_vfxDB.GetVFX(ESkillEffectType.WindGust), ESkillEffectType.WindGust);
                break;
            case ESkillEffectType.IceSlash:
                effect = new IceSlash(_vfxDB.GetVFX(ESkillEffectType.IceSlash), ESkillEffectType.IceSlash);
                break;
            case ESkillEffectType.Lightning:
                effect = new Lightning(_vfxDB.GetVFX(ESkillEffectType.Lightning), ESkillEffectType.Lightning);
                break;
            default:
                effect = new FireExplosion(_vfxDB.GetVFX(ESkillEffectType.FireExplosion), ESkillEffectType.FireExplosion);
                break;
        }
        return new SkillInstance(slot, effect);
    }
}
