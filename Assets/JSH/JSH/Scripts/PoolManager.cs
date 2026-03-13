using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private static Dictionary<string, IPool> pools = new Dictionary<string, IPool>();

    private void Awake()
    {
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreatePool<T>(T prefab, int generateCount, Transform parent = null) where T : MonoBehaviour, IPoolable
    {
        if (prefab == null) return;

        string key = prefab.GetInstanceID().ToString();
        if (pools.ContainsKey(key)) return;

        pools.Add(key, new ObjectPool<T>(prefab, generateCount, parent));
    }

    public T GetFromPool<T>(T prefab) where T : MonoBehaviour, IPoolable
    {
        if (prefab == null) return null;

        string key = prefab.GetInstanceID().ToString();
        if (!pools.TryGetValue(key, out var box)) return null;

        return (box as ObjectPool<T>)?.Dequeue();
    }
}

