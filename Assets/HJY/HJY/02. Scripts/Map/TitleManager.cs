
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("로그인 화면")]
    public GameObject loginButton;     // 로그인 버튼
    public GameObject loginPanel;      // 로그인 패널
    public GameObject yesButton;       // 테스트용 버튼

    [Header("로딩 바")]
    public GameObject loadingBar;      // 로딩바 오브젝트
    public Image fillImage;            // 로딩바 이미지
    public TextMeshProUGUI perText;    // 퍼센트 텍스트
    public TextMeshProUGUI checkText;  // 상태 텍스트

    [Header("로딩 화면")]
    public GameObject loadingPanel;     // 로딩 패널
    public TextMeshProUGUI loadingText; // 로딩 텍스트

    // 로그인 버튼을 클릭하면 로그인 패널 띄우기
    public void OnLoginButtonClick()
    {
        loginPanel.SetActive(true);
    }

    // 로그인 성공 가정하기
    public void OnConfirmLogin()
    {
        loginButton.SetActive(false);   // 로그인 버튼 숨김
        loginPanel.SetActive(false);    // 로그인 패널 숨김
        StartCoroutine(LoginLoadingCoroutine());
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

    // 로그인 후 로딩 애니메이션
    IEnumerator LoginLoadingCoroutine()
    {
        // 로딩 바 켜기
        loadingBar.SetActive(true); 
        UpdateLoadingUI(0f, "로그인 중...");

        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * 0.5f; // 2초 정도
            UpdateLoadingUI(progress);
            yield return null;
        }

        UpdateLoadingUI(1f); // 100% 고정
        yield return new WaitForSeconds(0.5f);

        // 로딩바 끄고 로딩 패널 켜기
        loadingBar.SetActive(false);
        loadingPanel.SetActive(true);
        loadingText.text = "Loading...";

        // 일정 시간 대기 후 로비 씬으로 전환
        yield return new WaitForSeconds(1f);
        StartCoroutine(LoadLobbyScene());
    }

    // 로비 씬 비동기 로딩
    IEnumerator LoadLobbyScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("Lobby");
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            // 로딩 패널에 띄울 텍스트
            if (loadingText != null)
                loadingText.text = "Loading...";

            if (op.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f);
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}