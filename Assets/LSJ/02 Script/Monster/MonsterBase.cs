using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterBase : EntityStateMachine, IDamageable, IPoolable2
{
    [SerializeField] protected MonsterBaseStatsSO _baseStats;
    [SerializeField] protected GameObject _damageTextPrefab;

    protected Animator _anim;
    protected SpriteRenderer _sr;
    protected Collider2D _col;
    public string Name { get; protected set; }
    public BigNumber CurrentHP {  get; protected set; }
    public BigNumber CurrentAtk { get; protected set; }
    public BigNumber CurrentDef { get; protected set; } 
    public MonsterIdleState IdleState { get; protected set; }
    public MonsterDeadState DeadState { get; protected set; }
    public Animator Animator => _anim;
    public SpriteRenderer SpriteRenderer => _sr;
    public Collider2D Collider => _col;

    protected virtual void Awake()
    {
        _anim = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
        _col = GetComponent<Collider2D>();

        Name = _baseStats.monsterName;

        IdleState = new MonsterIdleState(this);
        DeadState = new MonsterDeadState(this);
    }
    public void OnSpawn()
    {
        _col.enabled = true;
        CurrentHP = MonsterStatCorrection(_baseStats.baseMaxHP);
        CurrentAtk = MonsterStatCorrection(_baseStats.baseAttackPower);
        CurrentDef = MonsterStatCorrection(_baseStats.baseDefensivePower);

        ChangeState(IdleState);
    }
    public void OnDespawn()
    {

    }
    public void TakeDamage(BigNumber amount, bool isCritical = false)
    {
        if (amount <= new BigNumber(0)) return;

        BigNumber finalDamage = amount - CurrentDef;
        CurrentHP -= finalDamage;

        Color color = isCritical ? new Color(1f, 0.4f, 0.2f) : Color.white;

        GameObject dmgObj = PoolManager2.Instance.Get(
            _damageTextPrefab,
            transform.position,
            Quaternion.identity,
            transform
        );

        // 초기화
        if (dmgObj.TryGetComponent<DamageText>(out var dmgText))
        {
            dmgText.Initialize(finalDamage, color, transform.position);
        }
        if (CurrentHP <= new BigNumber(0))
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        StageManager.Instance.OnMonsterDeath();
        ChangeState(DeadState);
    }

    // 스테이지에 따른 스탯 수치 보정
    protected BigNumber MonsterStatCorrection(float stats)
    {
        BigNumber bn = new BigNumber(stats) *
            new BigNumber(Mathf.Pow(StageManager.Instance.CurrentMainNumber, 5)) *
            new BigNumber((StageManager.Instance.CurrentSubNumber + StageManager.Instance.CurrentMainNumber - 2) * 2);

        if (bn <= new BigNumber(0)) return new BigNumber(stats);
        else return bn;
    }
}
