
using UnityEngine;
using UnityEngine.UI;

public class LogoutManager : MonoBehaviour
{
    public GameObject quitPanel; // 종료 패널
    public Button logoutButton;  // 로그아웃 버튼
    public Button cancelButton;  // 취소 버튼

    void Start()
    {
        quitPanel.SetActive(false);

        logoutButton.onClick.AddListener(OnLogoutClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
    }

    void Update()
    {
        // ESC 키를 누르면
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 종료 패널을 띄워라.
            quitPanel.SetActive(true);
        }
    }

    // 로그아웃 버튼을 누르면
    void OnLogoutClicked()
    {
        // 로그아웃 리퀘스트 호출 -> 서버에 데이터 전달!!!!
        SendLogoutRequest();

        // 서버 호출 후 종료!!!
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서는 게임 종료하기
#else
        Application.Quit();                              // 빌드 파일에서는 창이 꺼지기
#endif
    }

    // 종료 패널을 닫아라.
    void OnCancelClicked()
    {
        quitPanel.SetActive(false);
    }

    // 로그아웃 리퀘스트
    void SendLogoutRequest()
    {
        // TODO: 서버에 로그아웃 요청 보내기
        // 재화, 인게임 데이터 등을 함께 전달
        Debug.Log("로그아웃 요청 전송됨");
    }
}