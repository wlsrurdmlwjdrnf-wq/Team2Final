using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterBase : EntityStateMachine, IDamageable, IPoolable2
{
    [SerializeField] protected MonsterBaseStatsSO _baseStats;
    [SerializeField] protected GameObject _damageTextPrefab;
    [SerializeField] protected HpBar _hpBar;

    protected Animator _anim;
    protected SpriteRenderer _sr;
    protected Collider2D _col;
    protected BigNumber _maxHp;

    public Transform Transform => transform;
    public string Name { get; protected set; }
    public BigNumber CurrentHP {  get; protected set; }
    public BigNumber CurrentAtk { get; protected set; }
    public BigNumber CurrentDef { get; protected set; } 
    public MonsterIdleState IdleState { get; protected set; }
    public MonsterDeadState DeadState { get; protected set; }
    public Animator Animator => _anim;
    public SpriteRenderer SpriteRenderer => _sr;
    public Collider2D Collider => _col;
    public ElementType ElementType {  get; protected set; }

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
        _maxHp = MonsterStatCorrection(_baseStats.baseMaxHP);
        CurrentHP = MonsterStatCorrection(_baseStats.baseMaxHP);
        CurrentAtk = MonsterStatCorrection(_baseStats.baseAttackPower);
        CurrentDef = MonsterStatCorrection(_baseStats.baseDefensivePower);

        if (_hpBar != null)
        {
            _hpBar.UpdateHP(CurrentHP, _maxHp);
            _hpBar.Show();
        }

        ChangeState(IdleState);
    }
    public void OnDespawn()
    {
        if (_hpBar != null)
            _hpBar.HideImmediately();
    }
    public void TakeDamage(BigNumber amount, bool isCritical = false, ElementType elementType = ElementType.Normal)
    {
        if (amount <= new BigNumber(0)) return;

        // 모험모드일 경우 데미지가 더 들어가거나 덜 들어감
        if (StageManager.Instance.CurrentStageData.isAdventureStage)
        {
            CalculateElementalDamage(ref amount, elementType);
        }

        BigNumber finalDamage = amount - CurrentDef;
        if(finalDamage <= new BigNumber(0)) finalDamage = new BigNumber(0); // 최종 데미지가 0이하면 체력이 회복되지 않도록 0으로 스냅
        CurrentHP -= finalDamage;

        // hp 업데이트
        if (_hpBar != null)
        {
            _hpBar.UpdateHP(CurrentHP, _maxHp);

            if (CurrentHP <= new BigNumber(0))
                _hpBar.HideImmediately();
        }

        // 데미지 텍스트
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
            CurrentHP = new BigNumber(0);
            Die();
        }
    }
    protected virtual void Die()
    {
        StageManager.Instance.OnMonsterDeath();
        PlayerResourceDropSystem.Instance.TriggerDrop(transform.position);
        EnemyManager.Instance.enemies.Remove(this);
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

    // 몬스터 속성에 따른 데미지 적용
    protected BigNumber CalculateElementalDamage(ref BigNumber amount, ElementType element)
    {
        // Fire < Water < Wind < Earth < Fire
        switch (element)
        {
            case ElementType.Fire:
                if (_baseStats.elementType == ElementType.Earth) amount *= new BigNumber(2);
                if (_baseStats.elementType == ElementType.Water) amount *= new BigNumber(0.7);
                break;
            case ElementType.Water:
                if (_baseStats.elementType == ElementType.Fire) amount *= new BigNumber(2);
                if (_baseStats.elementType == ElementType.Wind) amount *= new BigNumber(0.7);
                break;
            case ElementType.Wind:
                if (_baseStats.elementType == ElementType.Water) amount *= new BigNumber(2);
                if (_baseStats.elementType == ElementType.Earth) amount *= new BigNumber(0.7);
                break;
            case ElementType.Earth:
                if (_baseStats.elementType == ElementType.Wind) amount *= new BigNumber(2);
                if (_baseStats.elementType == ElementType.Fire) amount *= new BigNumber(0.7);
                break;
            default:
                break;
        }
        return amount;
    }
}
