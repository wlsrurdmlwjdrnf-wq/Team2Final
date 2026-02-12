using System.Collections.Generic;
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

            // ·£´ý ÇÁ¸®ÆÕ ¼±ÅÃ
            GameObject prefab = _stage.monsterPrefabs[Random.Range(0, _stage.monsterPrefabs.Count)];
            
            _monsters.Add(PoolManager2.Instance.Get
                (prefab,
                new Vector3(x, _stage.yFixedPosition, 0),
                Quaternion.identity, transform)
                );
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
    }
}