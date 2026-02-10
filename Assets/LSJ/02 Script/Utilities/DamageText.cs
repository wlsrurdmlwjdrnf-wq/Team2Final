using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class DamageText : MonoBehaviour, IPoolable2
{
    [SerializeField] private TextMeshProUGUI tmpText;

    [Header("애니메이션 설정")]
    [SerializeField] private float moveUpDistance = 1.8f;
    [SerializeField] private float duration = 1.2f;
    [SerializeField] private float fadeStartDelay = 0.5f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize(BigNumber damage, Color color, Vector3 worldPosition)
    {
        tmpText.text = BigNumberFormatter.ToFormatted(damage);
        tmpText.color = color;

        transform.position = worldPosition + Vector3.up * 0.8f;
        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one * 0.8f;  // 살짝 작게 시작
    }

    public void OnSpawn()
    {
        transform.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOMoveY(transform.position.y + moveUpDistance, duration)
            .SetEase(Ease.OutQuad));

        seq.Join(transform.DOScale(Vector3.one * 1.3f, duration * 0.5f)
            .SetEase(Ease.OutBack));

        seq.AppendInterval(fadeStartDelay);
        seq.Append(canvasGroup.DOFade(0f, duration - fadeStartDelay)
            .SetEase(Ease.InQuad));

        seq.OnComplete(() => PoolManager2.Instance.Release(gameObject));
    }
    public void OnDespawn()
    {
        transform.DOKill();
        canvasGroup.alpha = 0f;
    }
}
