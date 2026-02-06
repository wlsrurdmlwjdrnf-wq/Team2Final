using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackState : IEntityState
{
    private readonly Boss _boss;

    public BossAttackState(Boss boss) => _boss = boss;
    public void OnEnter() 
    {
        _boss.Animator.speed = _boss.AttackSpeed;
        _boss.Animator.SetBool("IsAttacking", true);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            _boss.AttackPoint.position,
            _boss.AttackRange,
            _boss.PlayerLayer
            );

        if (hits.Length > 0)
        {
            PlayerHpMp target = hits[0].GetComponent<PlayerHpMp>();
            if (target != null)
            {
                BigNumber damage = _boss.CurrentAtk;
                TryKnockBackAttack();
                target.TakeDamage(damage);
                _boss.LastAttackTime = Time.time;
            }
        }

    }
    public void OnUpdate() 
    {
        if (_boss.CanAttack())
        {
            Collider2D hit = Physics2D.OverlapCircle(
                _boss.AttackPoint.position,
                _boss.AttackRange,
                _boss.PlayerLayer
            );

            if (hit == null)
                _boss.ChangeState(_boss.IdleState);
            else
                _boss.ChangeState(_boss.AttackState); // ¿Á¡¯¿‘
        }
    }
    public void OnFixedUpdate() { }
    public void OnExit() 
    {
        _boss.Animator.speed = 1f;
    }

    private void TryKnockBackAttack()
    {
        int rand = Random.Range(0, 100);
        if (rand < 30) Player.TriggerKnockBack();
    }
}
