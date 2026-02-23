using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    public List<IDamageable> enemies = new List<IDamageable>();

    public IDamageable GetClosestEnemy(Vector2 position)
    {
        IDamageable closest = null;
        float minDist = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            float dist = (enemy.Transform.position - (Vector3)position).sqrMagnitude; // 제곱 거리 사용 (Vector3.Distance보다 성능상 좋다고 함)
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }
        return closest;
    }
}
