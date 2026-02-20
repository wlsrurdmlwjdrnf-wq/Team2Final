
using UnityEngine;

public class FixedCamera : MonoBehaviour
{
    [Header("해상도 조절")]
    [SerializeField] private float targetWidth = 720f;    // 목표 해상도 너비
    [SerializeField] private float targetHeight = 1280f;  // 목표 해상도 높이

    private Camera cam;               
    private int lastScreenWidth;      
    private int lastScreenHeight;     

    void Awake()
    {
        cam = GetComponent<Camera>();

        // 검은색 여백 만들기 (인스펙터창 설정)
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;

        // 기본 화면 크기 저장
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        // 초기 비율 설정
        UpdateAspect();
    }

    void Update()
    {
        // 창 크기 변경 감지
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            // 해상도 변경시 값 갱신
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;

            // 비율 재계산
            UpdateAspect();
        }
    }

 
    // 카메라 화면 비율 조정
    void UpdateAspect()
    {
        float targetAspect = targetWidth / targetHeight;             // 목표 비율
        float windowAspect = (float)Screen.width / Screen.height;    // 현재 창 비율

        float scaleHeight = windowAspect / targetAspect;             // 높이 비율 계산

        Rect rect = new Rect(0, 0, 1, 1);                            // 기본 전체 화면 Rect (인스펙터창 설정)

        if (scaleHeight < 1f)
        {
            // 현재 창이 목표보다 세로로 길면 위아래 여백 추가
            rect.height = scaleHeight;
            rect.y = (1f - scaleHeight) / 2f;
        }
        else
        {
            // 현재 창이 목표보다 가로로 길면 좌우 여백 추가
            float scaleWidth = 1f / scaleHeight;
            rect.width = scaleWidth;
            rect.x = (1f - scaleWidth) / 2f;
        }

        // 카메라 뷰포트 적용
        cam.rect = rect;
    }
}