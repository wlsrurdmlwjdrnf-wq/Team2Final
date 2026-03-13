using UnityEngine;
using UnityEngine.UI;

public class MpBar : MonoBehaviour
{
    [Header("필수 컴포넌트")]
    [SerializeField] private Slider _slider;           // World Space Slider (0~1)

    [Header("옵션")]
    [SerializeField] private Image _fillImage;         // Fill 색상 변경용
    [SerializeField] private CanvasGroup _canvasGroup; // alpha 제어용

    private void Awake()
    {
        if (_slider == null)
            _slider = GetComponentInChildren<Slider>();

        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        HideImmediately();
    }

    /// <summary>
    /// MP 값이 변경될 때마다 호출
    /// </summary>
    public void UpdateMP(float current, float max)
    {
        if (_slider == null) return;

        if (max <= 0f)
        {
            HideImmediately();
            return;
        }

        float normalized = current / max;
        normalized = Mathf.Clamp01(normalized);

        _slider.value = normalized;

        // Fill 색상 업데이트
        if (_fillImage != null)
        {
            _fillImage.color = GetMPColor(normalized);
        }

        // MP가 0 이하면 숨기거나 0으로 유지
        if (current <= 0f)
        {
            _slider.value = 0f;
        }
        else
        {
            Show();
        }
    }

    private Color GetMPColor(float norm)
    {
        if (norm >= 0.6f) return new Color(0.2f, 0.6f, 1.0f);    // 밝은 파랑
        if (norm >= 0.3f) return new Color(0.1f, 0.4f, 0.9f);    // 중간 파랑
        return new Color(0.05f, 0.2f, 0.7f);                      // 어두운 파랑
    }

    public void Show()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void HideImmediately()
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}