using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterContainer : MonoBehaviour
{
    private List<GameObject> _monsters = new();
    private StageSO _stage;

    private void OnEnable()
    {
        if (StageManager.Instance == null) return;
        StageManager.Instance.OnStageChanged += SpawnMonsters;
        StageManager.Instance.OnNeedMonsterClear += ClearMonsters;
    }
    private void OnDisable()
    {
        if (StageManager.Instance == null) return;
        StageManager.Instance.OnStageChanged -= SpawnMonsters;
        StageManager.Instance.OnNeedMonsterClear -= ClearMonsters;
    }

    private void SpawnMonsters()
    {
        _stage = StageManager.Instance.CurrentStageData;

        float startX = transform.position.x;
        float endX = startX + _stage.spawnAreaWidth;

        List<float> xPositions = GetValidXPositions(_stage.spawnCount, startX, endX);

        for (int i = 0; i < _stage.spawnCount && i < xPositions.Count; i++)
        {
            float x = xPositions[i];

            // 랜덤 프리팹 선택
            GameObject prefab = _stage.monsterPrefabs[Random.Range(0, _stage.monsterPrefabs.Count)];

            // 풀에서 꺼낸 실제 인스턴스 저장
            GameObject monsterInstance = PoolManager2.Instance.Get(
                prefab,
                new Vector3(x, _stage.yFixedPosition, 0),
                Quaternion.identity,
                transform 
            );

            _monsters.Add(monsterInstance);  // 인스턴스 추가

            var damageable = monsterInstance.GetComponent<IDamageable>();
            if (damageable != null)
            {
                EnemyManager.Instance.enemies.Add(damageable);
            }
            else
            {
                Debug.LogWarning("IDamageable 컴포넌트 없음: " + monsterInstance.name);
            }
        }
    }

    private List<float> GetValidXPositions(int count, float start, float end)
    {
        List<float> positions = new List<float>();
        for (int i = 0; i < count; i++)
        {
            float candidate;
            int tries = 0;
            do
            {
                candidate = Random.Range(start, end);
                tries++;
            } while (positions.Exists(p => Mathf.Abs(p - candidate) < _stage.minDistance) && tries < 50);

            positions.Add(candidate);
        }
        positions.Sort();
        return positions;
    }
    private void ClearMonsters()
    {
        if (_monsters == null) return;
        foreach(var monster in _monsters)
        {
            if(monster.gameObject.activeSelf)
                PoolManager2.Instance.Release(monster);
        }
        _monsters.Clear();
        EnemyManager.Instance.enemies.Clear();
    }
}