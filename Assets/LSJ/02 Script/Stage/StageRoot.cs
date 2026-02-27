using UnityEngine;
using System.Collections;

public class StageRoot : MonoBehaviour
{
    private Coroutine _knockBackCo;
    private StatModifier _knockBackModifier;
    private WaitForSeconds _knockBackDuration = new WaitForSeconds(0.6f); // 넉백애니메이션 길이

    private StatModifier _stopMoveModifier;
    private bool _isStop = false; // 멈춤 상태인지

    private Vector2 _initPosition;

    private void OnEnable()
    {
        _initPosition = transform.position;

        Player.OnAttack += HandleStopMove;
        Player.OnKnockBack += HandleKnockBack;
        Player.OnNoAttack += HandleResumeMove;
        Player.OnDead += HandleStopMove;

        if (StageManager.Instance == null) return;
        StageManager.Instance.OnStageChanged += HandleResumeMove;
        StageManager.Instance.OnStageChanged += InitPos;
    }
    private void OnDisable()
    {
        Player.OnAttack -= HandleStopMove;
        Player.OnKnockBack -= HandleKnockBack;
        Player.OnNoAttack -= HandleResumeMove;
        Player.OnDead -= HandleStopMove;

        if (StageManager.Instance == null) return;
        StageManager.Instance.OnStageChanged -= HandleResumeMove;
        StageManager.Instance.OnStageChanged -= InitPos;
    }

    private void Update()
    {
        // 전체 이동 (자식인 StageBase와 MonsterContainer가 같이 움직임)
        transform.Translate(Vector3.left * PlayerStatManager.Instance.MoveSpeed * Time.deltaTime);
    }

    // 넉백 이벤트 핸들러
    private void HandleKnockBack()
    {
        if (_knockBackCo != null) StopCoroutine(_knockBackCo);

        _knockBackCo = StartCoroutine(KnockBackRoutine());
    }

    private IEnumerator KnockBackRoutine()
    {
        HandleResumeMove();
        _knockBackModifier = new StatModifier(StatType.MoveSpeed, Operation.Multiply, -2.5f);
        PlayerStatManager.Instance.AddModifier(_knockBackModifier);
        yield return _knockBackDuration;
        PlayerStatManager.Instance.RemoveModifier(_knockBackModifier);
        _knockBackCo = null;
    }
    // 플레이어 공격 시 멈춤 이벤트 핸들러
    private void HandleStopMove()
    {
        if (_isStop) return;
        _stopMoveModifier = new StatModifier(StatType.MoveSpeed, Operation.Multiply, 0f);
        PlayerStatManager.Instance.AddModifier(_stopMoveModifier);
        _isStop = true;
    }
    // 재개
    private void HandleResumeMove()
    {
        if (_stopMoveModifier == null) return;
        PlayerStatManager.Instance.RemoveModifier(_stopMoveModifier);
        _isStop = false;
    }
    // 위치 초기화
    private void InitPos()
    {
        if(_initPosition == null) return;
        transform.position = _initPosition;
    }
}