using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public interface IPoolable
{
    void OnSpawn(); 
    void OnDespawn(); 
}

public class PoolManager2 : Singleton<PoolManager2>
{
    private readonly Dictionary<GameObject, IObjectPool<GameObject>> _pools = new();

    // 인스펙터에서 미리 풀 설정
    [SerializeField] private List<PoolConfig> _poolConfigs = new List<PoolConfig>();

    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        public int defaultCapacity = 20;
        public int maxSize = 100;
        public bool collectionCheck = false; // double-release 버그 (같은 오브젝트를 두 번 반납하는 실수)를 잡아주는 안전장치 역할 (false를 해야 성능적 이점이 있음)
    }

    protected override void Init()
    {
        base.Init();
        // 인스펙터에 등록된 풀들 미리 생성
        foreach (var config in _poolConfigs)
        {
            if (config.prefab == null) continue;
            CreateOrGetPool(config.prefab, config.defaultCapacity, config.maxSize, config.collectionCheck);
        }
    }

    // 풀 생성 또는 기존 풀 반환
    private IObjectPool<GameObject> CreateOrGetPool(GameObject prefab, int defaultCapacity, int maxSize, bool collectionCheck = false)
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
        instance.name = prefab.name; // 디버깅 편의
        instance.SetActive(false);
        instance.transform.SetParent(transform); // 초기 부모 설정
        return instance;
    }

    private void OnTakeFromPool(GameObject obj)
    {
        obj.SetActive(true);
        if (obj.TryGetComponent<IPoolable>(out var poolable))
        {
            poolable.OnSpawn();
        }
    }

    private void OnReturnedToPool(GameObject obj)
    {
        if (obj.TryGetComponent<IPoolable>(out var poolable))
        {
            poolable.OnDespawn();
        }
        obj.SetActive(false);
        obj.transform.SetParent(transform); // 풀 아래로 정리
    }

    private void OnDestroyPoolObject(GameObject obj)
    {
        Destroy(obj);
    }

    // 풀에서 가져오기
    public GameObject Get(GameObject prefab, Vector3 position = default, Quaternion rotation = default, Transform parent = null)
    {
        var pool = CreateOrGetPool(prefab, 20, 100); // 기본값으로 자동 생성
        var obj = pool.Get();

        obj.transform.SetPositionAndRotation(position, rotation);
        if (parent != null)
            obj.transform.SetParent(parent);

        return obj;
    }

    // 풀로 반납
    public void Release(GameObject obj)
    {
        if (obj == null) return;

        // 등록된 풀 중에서 매칭 (prefab 이름 기준으로 간단히 찾기)
        foreach (var kvp in _pools)
        {
            if (obj.name.StartsWith(kvp.Key.name))
            {
                kvp.Value.Release(obj);
                return;
            }
        }

        // 풀을 못 찾았을 경우 안전하게 파괴
        Debug.LogWarning($"Pool not found for {obj.name}. Destroying.", obj);
        Destroy(obj);
    }

    // 필요 시 모든 풀 비우기
    public void ClearAllPools()
    {
        foreach (var pool in _pools.Values)
        {
            pool.Clear();
        }
        _pools.Clear();
    }
}