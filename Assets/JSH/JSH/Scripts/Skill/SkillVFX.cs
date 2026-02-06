using UnityEngine;

public class SkillVFX : MonoBehaviour
{
    public float LifeTime = 2f;

    private void OnEnable()
    {
        Invoke("ReturnPool", LifeTime);
    }

    private void ReturnPool() 
    {
        PoolManager.Instance.ReturnPool(this);
    }
}
