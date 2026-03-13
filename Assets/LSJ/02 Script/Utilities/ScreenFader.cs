using UnityEngine;
using DG.Tweening;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadePanel;  // Inspector에서 위 Image의 CanvasGroup 드래그
    [SerializeField] private float fadeDuration = 1.2f;  // 어두워지는/밝아지는 시간
    [SerializeField] private float stayBlackTime = 0.5f; // 완전 어두운 상태 유지 시간 (옵션)

    private void Awake()
    {
        fadePanel.alpha = 1f;
        fadePanel.gameObject.SetActive(true); // 필요 시
    }
    private void OnEnable()
    {
        if(StageManager.Instance == null) return;
        StageManager.Instance.OnNeedScreenFader += FadeToBlackAndBack;
    }
    private void OnDisable()
    {
        if (StageManager.Instance == null) return;
        StageManager.Instance.OnNeedScreenFader -= FadeToBlackAndBack;
    }

    // 화면을 완전히 어두워졌다가 다시 밝아지게 하는 함수
    private void FadeToBlackAndBack()
    {
        // 기존 트윈 모두 종료 (중복 방지)
        DOTween.Kill(fadePanel);

        Sequence seq = DOTween.Sequence();

        // 1. Fade Out (어두워짐)
        seq.Append(fadePanel.DOFade(1f, fadeDuration).SetEase(Ease.InQuad));

        // 2. 완전 검정 상태에서 잠시 대기 (옵션)
        if (stayBlackTime > 0)
            seq.AppendInterval(stayBlackTime);

        // 3. Fade In (다시 밝아짐)
        seq.Append(fadePanel.DOFade(0f, fadeDuration).SetEase(Ease.OutQuad));

        seq.Play();
    }
}