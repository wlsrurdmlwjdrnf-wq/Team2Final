using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterContainer : MonoBehaviour
{
    [SerializeField] private List<GameObject> monsterPrefabs;
    [SerializeField] private int spawnCount = 10;
    [SerializeField] private float spawnAreaWidth = 20f;
    [SerializeField] private float minDistance = 2.5f;
    [SerializeField] private float yFixedPosition = 0.2f; // ∞Ì¡§

    private void OnEnable()
    {
        SpawnMonsters();
    }

    private void SpawnMonsters()
    {
        float startX = transform.position.x;
        float endX = startX + spawnAreaWidth;

        List<float> xPositions = GetValidXPositions(spawnCount, startX, endX);

        for (int i = 0; i < spawnCount && i < xPositions.Count; i++)
        {
            float x = xPositions[i];

            // ∑£¥˝ «¡∏Æ∆’ º±≈√
            GameObject prefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Count)];
            PoolManager2.Instance.Get(prefab, new Vector3(x, yFixedPosition, 0), Quaternion.identity, transform);
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
            } while (positions.Exists(p => Mathf.Abs(p - candidate) < minDistance) && tries < 50);

            positions.Add(candidate);
        }
        positions.Sort();
        return positions;
    }
}