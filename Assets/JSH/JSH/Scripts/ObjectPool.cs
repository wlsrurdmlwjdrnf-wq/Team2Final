using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : IPool where T : MonoBehaviour, IPoolable
{
    private readonly Queue<T> poolQueue = new Queue<T>();
    private readonly T prefab;

    public Transform Root { get; private set; }

    public ObjectPool(T prefab, int count, Transform parent = null)
    {
        this.prefab = prefab;
        Root = new GameObject($"{prefab.name}_pool").transform;
        Object.DontDestroyOnLoad(Root.gameObject);

        if (parent != null)
        {
            Root.SetParent(parent, false);
        }

        for (int i = 0; i < count; i++)
        {
            var obj = GameObject.Instantiate(prefab, Root);
            obj.SetPool(this);
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
            obj.SetPool(this);
        }
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Enqueue(IPoolable instance)
    {
        var obj = instance as T;
        if (obj == null) return;

        obj.gameObject.SetActive(false);
        poolQueue.Enqueue(obj);
    }
}