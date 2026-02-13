
using UnityEngine;

public interface IUpgradable 
{
    int Level { get; }

    public void Upgrade() { }
}
public interface ISkillEffect 
{
    void Apply(Collider2D[] enemies, float damageDuplicator);
}
public interface IPoolable 
{
    void SetPool(IPool pool);
    void ReturnPool();
}
public interface IPool 
{
    void Enqueue(IPoolable instance);
}