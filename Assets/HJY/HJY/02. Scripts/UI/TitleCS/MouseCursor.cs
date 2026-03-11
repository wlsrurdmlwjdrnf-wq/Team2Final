
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    [Header("마우스 커서")]
    [SerializeField] private Texture2D[] cursorIcons;                 // 여러 커서 이미지 배열
    [SerializeField] private Vector2 basisPoint = Vector2.zero;       // 클릭 기준점
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto; // 소프트웨어 혹은 하드웨어 커서

    private int currentIndex = 0;                                     // 현재 커서 인덱스

    // 커서의 모양을 바꿔라.
    public void ChangeCursor()
    {
        if (currentIndex < cursorIcons.Length)
        {
            // 추가한 커서 이미지 사용
            Cursor.SetCursor(cursorIcons[currentIndex], basisPoint, cursorMode);
        }
        else
        {
            // 마지막 순서에서는 기본 커서로 되돌리기
            Cursor.SetCursor(null, Vector2.zero, cursorMode);
        }

        // 씬 전환에도 유지!!!
        DontDestroyOnLoad(gameObject);

        // 계속 클릭 시 인덱스가 증가하고 순서를 순환함
        currentIndex = (currentIndex + 1) % (cursorIcons.Length + 1);
    }
}