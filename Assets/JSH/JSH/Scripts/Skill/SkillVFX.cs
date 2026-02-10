using UnityEngine;

public class SkillVFX : MonoBehaviour, IPoolable
{
    public float LifeTime = 2f;
    private IPool _pool;

    private void OnEnable()
    {
        Invoke("ReturnPool", LifeTime);
    }

    public void SetPool(IPool pool) 
    {
        _pool = pool;
    }

    public void ReturnPool() 
    {
        _pool.Enqueue(this);
    }
}
