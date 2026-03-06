using System.Collections;
using UnityEngine;

public class ObjectFlash : MonoBehaviour
{
    [Header("플래시 설정")]
    [SerializeField, Tooltip("플래시 지속 시간 (초)")]
    private float flashDuration = 0.12f;

    [SerializeField, Tooltip("밝게 만들 때 사용할 HDR 컬러 (인스펙터에서 Intensity 조절 가능)")]
    [ColorUsage(true, true)]
    private Color flashTint = new Color(1.3f, 1.3f, 1.3f, 1f);

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private MaterialPropertyBlock propertyBlock;
    private WaitForSeconds _flashDrt;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            enabled = false;
            return;
        }

        originalColor = spriteRenderer.color;
        propertyBlock = new MaterialPropertyBlock();
        _flashDrt = new WaitForSeconds(flashDuration * 0.4f);
    }

    public void Flash()
    {
        if (spriteRenderer == null) return;

        // 새로운 블록 생성 (매번 새로 만드는 게 안전)
        var block = new MaterialPropertyBlock();

        // 스프라이트 텍스처 유지 (하얀 덩어리 방지)
        if (spriteRenderer.sprite != null)
        {
            block.SetTexture("_MainTex", spriteRenderer.sprite.texture);
        }

        // 플래시 색상 적용
        block.SetColor("_Color", flashTint);

        spriteRenderer.SetPropertyBlock(block);

        StartCoroutine(ResetFlashRoutine());
    }

    private IEnumerator ResetFlashRoutine()
    {
        // 순간 유지 시간
        yield return _flashDrt;

        // 복귀
        var block = new MaterialPropertyBlock();

        if (spriteRenderer.sprite != null)
        {
            block.SetTexture("_MainTex", spriteRenderer.sprite.texture);
        }

        block.SetColor("_Color", originalColor);

        spriteRenderer.SetPropertyBlock(block);

    }

    private void OnEnable()
    {
        // 색상 및 프로퍼티 블록 초기화
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
            spriteRenderer.SetPropertyBlock(null);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}

/**********************************************************
일반적으로 renderer.material.color = 빨강; 이렇게 하면
→ Unity가 Material의 복사본(인스턴스)을 자동으로 만들어줌
→ 복사본이 생기면 GPU Instancing(배칭)이 깨져서 Draw Call이 폭발적으로 늘어나고, 프레임이 떨어짐 (방치형 게임에서 몬스터 100마리만 돼도 문제)
MaterialPropertyBlock을 쓰면
→ Material 자체는 하나만 유지하면서
→ Renderer마다 "추가 프로퍼티 값"만 GPU에 따로 전달
→ 복사본 생성 X → Instancing 유지 → Draw Call 적음 → 성능 ↑
**********************************************************/

