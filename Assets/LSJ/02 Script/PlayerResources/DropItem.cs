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

    [Header("착지 후 흔들림 (선택)")]
    [SerializeField] private bool doLandingShake = true;
    [SerializeField] private float shakeDuration = 0.18f;
    [SerializeField] private float shakeStrength = 0.07f;

    [Header("반납 타이밍")]
    [SerializeField] private float autoReturnDelayAfterLanding = 0.8f; // 착지 후 몇 초 뒤에 사라질지

    private void PlayDropAnimation()
    {
        // 랜덤 방향 & 거리
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float randomDist = Random.Range(spreadRadius * 0.4f, spreadRadius);
        Vector3 targetPos = (Vector3)(randomDir * randomDist);

        // 약간의 지연 (몇 개 동시에 떨어지면 더 자연스러움)
        float randomDelay = Random.Range(staggerMin, staggerMax);

        // 회전 방향 랜덤화
        float spinDir = randomizeSpinDirection ? (Random.value > 0.5f ? 1f : -1f) : 1f;
        float finalRotation = spinDir * (spinAmount + Random.Range(-80f, 80f));

        Sequence seq = DOTween.Sequence();

        // 살짝 위로 띄우고 → 점프하며 목표지점으로
        seq.AppendInterval(randomDelay);

        seq.Append(
            transform.DOJump(targetPos, jumpPower, 1, duration)
                .SetEase(Ease.OutQuad)
        );

        // 동시에 회전
        seq.Join(
            transform.DORotate(new Vector3(0, 0, finalRotation), duration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutSine)
        );

        // 착지 후 살짝 흔들림 (선택)
        if (doLandingShake)
        {
            seq.AppendCallback(() =>
            {
                transform.DOShakePosition(shakeDuration, shakeStrength, 12, 80f, false, false)
                    .SetEase(Ease.OutSine);
            });
        }

        // 연출 모두 끝난 후 풀에 반납
        seq.AppendInterval(autoReturnDelayAfterLanding);
        seq.AppendCallback(() =>
        {
            PoolManager2.Instance.Release(gameObject);
        });
    }

    public void OnSpawn()
    {
        PlayDropAnimation();
    }

    public void OnDespawn()
    {
        // 풀에 돌아갈 때 DOTween 중지 (안전장치)
        transform.DOKill(true);
    }
}