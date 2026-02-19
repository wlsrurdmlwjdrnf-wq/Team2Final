
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("인증 화면")]
    [SerializeField] private GameObject serverButton;     // 서버로 로그인하기 버튼
    [SerializeField] private GameObject authPanel;        // 로그인 및 회원가입 패널 
    [SerializeField] private GameObject loginPage;        // 로그인 페이지
    [SerializeField] private GameObject signUpPage;       // 회원가입 페이지
    [SerializeField] private TextMeshProUGUI signalText;  // 오류 및 성공 메시지

    [Header("버튼 연결")]
    [SerializeField] private Button loginButton;         // 로그인 버튼
    [SerializeField] private Button signUpButton;        // 회원가입 버튼
    [SerializeField] private Button callLoginBtn;        // 로그인하러 가기 버튼
    [SerializeField] private Button callSignUpBtn;       // 회원가입하러 가기 버튼

    [Header("로그인 로딩")]
    [SerializeField] private GameObject loadingBar;      // 로딩 바
    [SerializeField] private Image fillImage;            // 로딩 이미지
    [SerializeField] private TextMeshProUGUI perText;    // 로딩 퍼센트
    [SerializeField] private TextMeshProUGUI checkText;  // 로그인하는 중

    [Header("로딩 화면")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("색상 관리")]
    [SerializeField] private Color successColor; // 성공 시 안내 문구 색상
    [SerializeField] private Color failColor;    // 실패 시 안내 문구 색상


    [Header("서버가 꺼진 상태")]
    [SerializeField] private Button tabToStart; // 테스트용 버튼


    private void Start()
    {
        // 버튼 이벤트 연결
        callLoginBtn.onClick.AddListener(ShowLoginUI);
        callSignUpBtn.onClick.AddListener(ShowSignupUI);


        // 테스트용 버튼 이벤트 연결
        tabToStart.onClick.AddListener(OnTabToStartClick);
    }

    // 테스트용 버튼 클릭 시 로그인 없이 로딩 시작
    public void OnTabToStartClick()
    {
        serverButton.SetActive(false);
        authPanel.SetActive(false);
        StartCoroutine(LoginLoadingCoroutine());
    }


    // 인증 패널 켜기
    public void OnServerButtonClick()
    {
        authPanel.SetActive(true);
        ShowLoginUI();
    }

    // 로그인 UI 켜기
    public void ShowLoginUI()
    {
        loginPage.SetActive(true);
        signUpPage.SetActive(false);
        signalText.gameObject.SetActive(false);
    }

    // 회원가입 UI 켜기
    public void ShowSignupUI()
    {
        signUpPage.SetActive(true);
        loginPage.SetActive(false);
        signalText.gameObject.SetActive(false);
    }

    // 로그인 성공
    public void OnLoginSuccess()
    {
        serverButton.SetActive(false);
        authPanel.SetActive(false);
        StartCoroutine(LoginLoadingCoroutine());

        // 로그인 실패 후 재시도가 성공할 때 안내 문구를 끄기
        signalText.gameObject.SetActive(false); 
    }

    // 로그인 실패 및 서버 오류 안내 문구
    public void OnLoginFail(string message)
    {
        signalText.gameObject.SetActive(true);
        signalText.text = message;
        signalText.color = failColor;
    }

    // 회원가입 성공 및 실패 안내 문구
    public void OnSignupResult(bool success, string message)
    {
        signalText.gameObject.SetActive(true);
        signalText.text = message;

        if (success)
        {
            signalText.color = successColor;  // 파란색
        }
        else
        {
            signalText.color = failColor;     // 빨간색
        }


    }

    // 로딩 UI 갱신
    void UpdateLoadingUI(float progress, string status = null)
    {
        fillImage.fillAmount = progress;
        perText.text = $"{(progress * 100f):F0}%";
        if (!string.IsNullOrEmpty(status))
        {
            checkText.text = status;
        }
    }

    // 로그인 로딩 바
    IEnumerator LoginLoadingCoroutine()
    {
        loadingBar.SetActive(true);
        UpdateLoadingUI(0f, "로그인 중...");

        float progress = 0f;
        while (progress < 1f)
        {
            // 진행률 증가
            progress += Time.deltaTime * 0.5f;
            UpdateLoadingUI(progress);
            yield return null;
        }

        UpdateLoadingUI(1f);
        yield return new WaitForSeconds(0.5f);

        loadingBar.SetActive(false);
        loadingPanel.SetActive(true);
        loadingText.text = "Loading...";

        yield return new WaitForSeconds(1f);
        StartCoroutine(LoadLobbyScene());
    }

    // 로비 씬으로 가라.
    IEnumerator LoadLobbyScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("Lobby"); // 비동기 씬 로딩
        op.allowSceneActivation = false;                          // 씬 자동 활성화 막기

        while (!op.isDone)
        {
            if (loadingText != null)
                loadingText.text = "Loading...";

            // 로딩이 거의 완료된 시점
            if (op.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f); // 씬 전환 전에 대기
                op.allowSceneActivation = true;        // 로비 씬으로 전환
            }
            yield return null;
        }
    }
}