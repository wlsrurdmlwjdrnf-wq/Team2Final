using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterBase : EntityStateMachine, IDamageable
{
    [SerializeField] protected MonsterBaseStatsSO _baseStats;

    protected Animator _anim;
    protected SpriteRenderer _sr;
    public string Name { get; protected set; }
    public BigNumber CurrentHP {  get; protected set; }
    public BigNumber CurrentAtk { get; protected set; }
    public BigNumber CurrentDef { get; protected set; } 
    public MonsterIdleState IdleState { get; protected set; }
    public MonsterDeadState DeadState { get; protected set; }
    public Animator Animator => _anim;
    public SpriteRenderer SpriteRenderer => _sr;

    protected virtual void Awake()
    {
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();

        Name = _baseStats.monsterName;

        IdleState = new MonsterIdleState(this);
        DeadState = new MonsterDeadState(this);

        ChangeState(IdleState);
    }
    protected void OnEnable()
    {
        CurrentHP = MonsterStatCorrection(_baseStats.baseMaxHP);
        CurrentAtk = MonsterStatCorrection(_baseStats.baseAttackPower);
        CurrentDef = MonsterStatCorrection(_baseStats.baseDefensivePower);
    }
    public void TakeDamage(BigNumber amount)
    {
        if (amount <= new BigNumber(0)) return;

        CurrentHP -= amount - CurrentDef;

        if (CurrentHP <= new BigNumber(0))
        {
            Die();
        }
    }
    protected void Die()
    {
        ChangeState(DeadState);
    }

    // 스테이지에 따른 스탯 수치 보정
    protected BigNumber MonsterStatCorrection(float stats)
    {
        BigNumber bn = new BigNumber(stats) *
            new BigNumber(Mathf.Pow(StageManager.Instance.CurrentMainNumber, 10)) *
            new BigNumber((StageManager.Instance.CurrentSubNumber - 1) * 2);

        if (bn == new BigNumber(0)) return new BigNumber(stats);
        else return bn;
    }
}
