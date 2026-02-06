using System.Collections.Generic;
using UnityEngine;

public class PoolManager2 : Singleton<PoolManager2>
{
    private static Dictionary<string, object> pools = new Dictionary<string, object>();
    public void CreatePool<T>(T prefab, int generateCount, Transform parent = null) where T : MonoBehaviour
    {
        if (prefab == null) return;

        string key = typeof(T).Name + "_" + prefab.name;
        if (pools.ContainsKey(key)) return;

        pools.Add(key, new ObjectPool2<T>(prefab, generateCount, parent));
    }

    public T GetFromPool<T>(T prefab) where T : MonoBehaviour
    {
        if (prefab == null) return null;

        string key = typeof(T).Name + "_" + prefab.name;
        if (!pools.TryGetValue(key, out var box)) return null;

        return (box as ObjectPool2<T>)?.Dequeue();
    }

    public void ReturnPool<T>(T instance) where T : MonoBehaviour
    {
        if (instance == null) return;

        string key = typeof(T).Name + "_" + instance.name.Replace("(Clone)", "");
        if (!pools.TryGetValue(key, out var box))
        {
            Destroy(instance.gameObject);
            return;
        }

        (box as ObjectPool2<T>)?.Enqueue(instance);
    }
}
public class ObjectPool2<T> where T : MonoBehaviour
{
    private readonly Queue<T> poolQueue = new Queue<T>();
    private readonly T prefab;
    public string Key { get; private set; }

    public Transform Root { get; private set; }

    public ObjectPool2(T prefab, int count, Transform parent = null)
    {
        this.prefab = prefab;
        Key = typeof(T).Name + "_" + prefab.name;
        Root = new GameObject($"{prefab.name}_pool").transform;
        Object.DontDestroyOnLoad(Root.gameObject);

        if (parent != null)
        {
            Root.SetParent(parent, false);
        }

        for (int i = 0; i < count; i++)
        {
            var obj = GameObject.Instantiate(prefab, Root);
            obj.gameObject.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }

    public T Dequeue()
    {
        T obj;
        if (poolQueue.Count > 0)
        {
            obj = poolQueue.Dequeue();
        }
        else
        {
            obj = GameObject.Instantiate(prefab, Root);
        }
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Enqueue(T instance)
    {
        instance.gameObject.SetActive(false);
        poolQueue.Enqueue(instance);
    }
}