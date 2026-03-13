using UnityEngine;
using DG.Tweening;

public class DropItem : MonoBehaviour, IPoolable2
{
    [Header("물리/움직임 설정")]
    [SerializeField] private float spreadRadius = 1.4f;         // 중심에서 퍼지는 최대 반경
    [SerializeField] private float jumpPower = 1.6f;            // 점프 높이
    [SerializeField] private float duration = 0.55f;            // 전체 점프 시간
    [SerializeField] private float staggerMin = 0.00f;          // 첫 아이템 지연 최소
    [SerializeField] private float staggerMax = 0.12f;          // 마지막 아이템 지연 최대

    [Header("회전 & 이펙트")]
    [SerializeField] private float spinAmount = 360f * 2f;      // 몇 도 회전할지 (랜덤 방향)
    [SerializeField] private bool randomizeSpinDirection = true;

    [Header("착지")]
    [SerializeField] private float yFixedPosition = 0f;
    [SerializeField] private bool doLandingShake = true;
    [SerializeField] private float shakeDuration = 0.18f;
    [SerializeField] private float shakeStrength = 0.07f;

    [Header("반납 타이밍")]
    [SerializeField] private float autoReturnDelayAfterLanding = 0.8f; // 착지 후 몇 초 뒤에 사라질지

    [Header("페이드 아웃")]
    [SerializeField] private float fadeOutDuration = 0.4f;  // 투명해지는 데 걸리는 시간
    [SerializeField] private Ease fadeEase = Ease.InQuad;   // 서서히 사라지게

    private void PlayDropAnimation()
    {

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float randomDist = Random.Range(spreadRadius * 0.4f, spreadRadius);

        // 목표 위치 → y는 무조건 고정
        Vector3 targetPos = new Vector3(
            randomDir.x * randomDist,
            yFixedPosition,
            randomDir.y * randomDist
        );

        float randomDelay = Random.Range(staggerMin, staggerMax);

        float spinDir = randomizeSpinDirection ? (Random.value > 0.5f ? 1f : -1f) : 1f;
        float finalRotation = spinDir * (spinAmount + Random.Range(-80f, 80f));

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(randomDelay);

        seq.Append(
            transform.DOJump(
                targetPos,
                jumpPower,     // 최대 상승 높이 (상대적)
                1,             // 점프 횟수 (1이면 한 번 튀는 느낌)
                duration
            )
            .SetEase(Ease.OutQuad)   
        );

        // 회전은 전체 시간 동안 같이
        seq.Join(
            transform.DORotate(new Vector3(0, 0, finalRotation), duration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutSine)
        );

        // 착지 시점에 흔들림 넣기
        if (doLandingShake)
        {
            seq.AppendCallback(() =>
            {
                transform.DOShakePosition(shakeDuration, shakeStrength, 12, 80f, false, false)
                    .SetEase(Ease.OutSine);
            });
        }

        // 착지 후 대기 시간 후에 페이드 아웃 시작
        seq.AppendInterval(autoReturnDelayAfterLanding);

        // 페이드 아웃 → 완료 후 풀 반납
        seq.AppendCallback(() =>
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.DOFade(0f, fadeOutDuration)
                  .SetEase(fadeEase)
                  .OnComplete(() => PoolManager2.Instance.Release(gameObject));
            }
            else
            {
                // SpriteRenderer가 없으면 바로 반납 (fallback)
                PoolManager2.Instance.Release(gameObject);
            }
        });
    }

    public void OnSpawn()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(1, 1, 1, 1);

        PlayDropAnimation();
    }

    public void OnDespawn()
    {
        // 풀에 돌아갈 때 DOTween 중지 (안전장치)
        transform.DOKill(true);
    }
}