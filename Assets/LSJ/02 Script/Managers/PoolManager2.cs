using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public interface IPoolable2
{
    void OnSpawn();
    void OnDespawn();
}

public class PoolManager2 : Singleton<PoolManager2>
{
    // 프리팹 -> 풀
    private readonly Dictionary<GameObject, IObjectPool<GameObject>> _pools = new();

    // 활성 인스턴스 -> 자신이 속한 풀
    private readonly Dictionary<GameObject, IObjectPool<GameObject>> _activeObjectsToPool = new();

    // 인스펙터에서 미리 풀 설정
    [SerializeField] private List<PoolConfig> _poolConfigs = new List<PoolConfig>();

    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        public int defaultCapacity = 20;
        public int maxSize = 100;
        public bool collectionCheck = false; // double-release 방지 (개발 중엔 true / 빌드 전에 false로 바꿀 것)
    }

    protected override void Init()
    {
        base.Init();

        foreach (var config in _poolConfigs)
        {
            if (config.prefab == null) continue;
            CreateOrGetPool(
                config.prefab,
                config.defaultCapacity,
                config.maxSize,
                config.collectionCheck
            );
        }
    }

    private IObjectPool<GameObject> CreateOrGetPool(
        GameObject prefab,
        int defaultCapacity,
        int maxSize,
        bool collectionCheck = true)
    {
        if (_pools.TryGetValue(prefab, out var existingPool))
            return existingPool;

        var pool = new UnityEngine.Pool.ObjectPool<GameObject>(
            () => CreatePooledItem(prefab),
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            collectionCheck: collectionCheck,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        _pools[prefab] = pool;
        return pool;
    }

    private GameObject CreatePooledItem(GameObject prefab)
    {
        var instance = Instantiate(prefab);
        instance.name = prefab.name;  // 디버깅 편의 (Clone 붙지 않게)
        instance.SetActive(false);
        instance.transform.SetParent(transform); // 풀 매니저 아래로
        return instance;
    }

    private void OnTakeFromPool(GameObject obj)
    {
        obj.SetActive(true);
        if (obj.TryGetComponent<IPoolable2>(out var poolable))
        {
            poolable.OnSpawn();
        }
    }

    private void OnReturnedToPool(GameObject obj)
    {
        if (obj.TryGetComponent<IPoolable2>(out var poolable))
        {
            poolable.OnDespawn();
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform, worldPositionStays: true);
    }

    private void OnDestroyPoolObject(GameObject obj)
    {
        Destroy(obj);
    }

    public GameObject Get(
        GameObject prefab,
        Vector3 position = default,
        Quaternion rotation = default,
        Transform parent = null)
    {
        var pool = CreateOrGetPool(prefab, 20, 100, collectionCheck: false);

        var obj = pool.Get();

        obj.transform.SetPositionAndRotation(position, rotation);

        if (parent != null)
        {
            obj.transform.SetParent(parent, worldPositionStays: true);
        }
        else
        {
            obj.transform.SetParent(transform, worldPositionStays: true);
        }

        _activeObjectsToPool[obj] = pool; 

        return obj;
    }

    public void Release(GameObject obj)
    {
        if (obj == null) return;

        if (_activeObjectsToPool.TryGetValue(obj, out var pool))
        {
            pool.Release(obj);
            _activeObjectsToPool.Remove(obj);
        }
        else
        {
            // 추적되지 않은 오브젝트 -> 경고 후 파괴
            Debug.LogWarning($"Trying to release an object not managed by pool: {obj.name}", obj);
            Destroy(obj);
        }
    }

    public void ClearAllPools()
    {
        foreach (var pool in _pools.Values)
        {
            pool.Clear();
        }
        _pools.Clear();
        _activeObjectsToPool.Clear();
    }

    // 특정 프리팹의 풀만 정리
    public void ClearPool(GameObject prefab)
    {
        if (_pools.TryGetValue(prefab, out var pool))
        {
            pool.Clear();
            _pools.Remove(prefab);
        }
    }
}