using UnityEngine;

public class TestSkill_1 : ISkillEffect 
{
    private SkillVFX _vfx;

    public TestSkill_1(SkillVFX vfx)
    {
        _vfx = vfx;
    }
    public void Apply() 
    {
        var effect = PoolManager.Instance.GetFromPool(_vfx);
        effect.transform.position = Vector3.zero;
        Debug.Log("TestSkill_1");
    }
}
public class TestSkill_2 : ISkillEffect
{
    private SkillVFX _vfx;

    public TestSkill_2(SkillVFX vfx)
    {
        _vfx = vfx;
    }
    public void Apply()
    {
        var effect = PoolManager.Instance.GetFromPool(_vfx);
        effect.transform.position = Vector3.zero;
        Debug.Log("TestSkill_2");
    }
}
public class TestSkill_3 : ISkillEffect
{
    private SkillVFX _vfx;

    public TestSkill_3(SkillVFX vfx)
    {
        _vfx = vfx;
    }
    public void Apply()
    {
        var effect = PoolManager.Instance.GetFromPool(_vfx);
        effect.transform.position = Vector3.zero;
        Debug.Log("TestSkill_3");
    }
}
public class TestSkill_4 : ISkillEffect
{
    private SkillVFX _vfx;

    public TestSkill_4(SkillVFX vfx)
    {
        _vfx = vfx;
    }
    public void Apply()
    {
        var effect = PoolManager.Instance.GetFromPool(_vfx);
        effect.transform.position = Vector3.zero;
        Debug.Log("TestSkill_4");
    }
}
