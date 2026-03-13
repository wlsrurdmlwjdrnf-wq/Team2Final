using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [Header("필수 컴포넌트")]
    [SerializeField] private Slider _slider;           // World Space Slider (0~1)

    [Header("옵션")]
    [SerializeField] private Image _fillImage;         // Fill 색상 변경용
    [SerializeField] private CanvasGroup _canvasGroup; // alpha 제어용
    [SerializeField] private bool showFullWhenOverflow = true; // 지수 너무 크면 100% 표시할지

    private void Awake()
    {
        if (_slider == null)
            _slider = GetComponentInChildren<Slider>();

        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        HideImmediately();
    }

    /// <summary>
    /// HP 값이 변경될 때마다 호출
    /// </summary>
    public void UpdateHP(BigNumber current, BigNumber max)
    {
        if (_slider == null) return;

        // 0 이하 -> 숨김 & 0%
        if (current <= new BigNumber(0))
        {
            _slider.value = 0f;
            HideImmediately();
            return;
        }

        // max가 0 이하 -> 비정상 -> 숨김
        if (max <= new BigNumber(0))
        {
            HideImmediately();
            return;
        }

        float normalized;

        // BigNumber -> double 변환 (안전하게)
        double curDouble = current.ToDoubleSafe();
        double maxDouble = max.ToDoubleSafe();

        if (double.IsPositiveInfinity(maxDouble) || double.IsPositiveInfinity(curDouble))
        {
            // 무한대 처리
            normalized = showFullWhenOverflow ? 1f : 0.999f; // 거의 꽉 차게 or 살짝 덜 차게
        }
        else if (double.IsNaN(curDouble) || double.IsNaN(maxDouble))
        {
            normalized = 0f;
        }
        else
        {
            normalized = (float)(curDouble / maxDouble);
            normalized = Mathf.Clamp01(normalized);
        }

        _slider.value = normalized;

        // Fill 색상 업데이트
        if (_fillImage != null)
        {
            _fillImage.color = GetHPColor(normalized);
        }

        // HP가 0 초과면 보이게
        Show();
    }

    private Color GetHPColor(float norm)
    {
        if (norm >= 0.70f) return new Color(0.15f, 0.85f, 0.15f); // 진한 초록
        if (norm >= 0.40f) return new Color(0.95f, 0.95f, 0.20f); // 노랑
        if (norm >= 0.15f) return new Color(1.00f, 0.50f, 0.10f); // 주황
        return new Color(0.90f, 0.15f, 0.15f);                    // 빨강
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