using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(BigNumber amount, bool isCritical = false, ElementType elementType = ElementType.Normal);

    Transform Transform { get; }
}
