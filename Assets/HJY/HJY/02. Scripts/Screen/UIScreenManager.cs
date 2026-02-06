
using System.Collections.Generic;
using UnityEngine;

public class UIScreenManager : MonoBehaviour
{
    public static UIScreenManager Instance;

    [SerializeField] Transform screenRoot;

    // 화면 관리
    Stack<UIScreen> screenStack = new();

    void Awake()
    {
        Instance = this;
    }

    // 새로운 화면을 열기
    public async void OpenScreen(string addressKey)
    {
        // 기존 화면이 있으면 제거
        if (screenStack.Count > 0)
        {
            var top = screenStack.Pop();   // 스택에서 최상단 화면 꺼내기
            top.OnExit();                  // 화면 종료하기
            Destroy(top.gameObject);       // 삭제해라
        }

        // Addressables을 통해 화면 프리팹 켜기
        var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(addressKey);

        await handle.Task; // 비동기 로드 완료 대기

        // 로드된 프리팹을 인스턴스화하여 화면에 붙이기
        var go = Instantiate(handle.Result, screenRoot);
        var screen = go.GetComponent<UIScreen>();

        // 새 화면을 스택에 추가하기
        screenStack.Push(screen);
        screen.OnEnter(); // 화면 진입하기
    }
}