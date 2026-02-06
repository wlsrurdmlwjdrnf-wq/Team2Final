
public interface IUpgradable 
{
    int Level { get; }

    public void Upgrade() { }
}
public interface ISkillEffect 
{
    void Apply();
}