using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
인스펙터창에서 DDOL 컨트롤 가능합니다
True -> 파괴
False -> 유지

대신 파괴될때 null처리 되므로 주의해주세요
오버라이딩 가능하니까 따로 처리하셔도 됩니다

*/
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    [SerializeField] protected bool isDestroyOnLoad = false;

    public static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Object.FindFirstObjectByType<T>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        if (instance == null)
        {
            instance = (T)this;

            if (!isDestroyOnLoad)
            {
                if (transform.parent != null) transform.SetParent(null);
                DontDestroyOnLoad(this);
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            Dispose();
        }
    }

    protected virtual void Dispose()
    {
        instance = null;
    }
}
