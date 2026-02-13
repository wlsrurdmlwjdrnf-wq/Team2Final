using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Canvas))]
public class SkillVFX : MonoBehaviour, IPoolable
{
    public float LifeTime = 1f;
    private float _damageDuplicator = 0f;
    private IPool _pool;
    private GameObject _target;

    [SerializeField] private float _effectRange;
    [SerializeField] private float _projectileSpeed = 5f;
    [SerializeField] private LayerMask _monsterLayer;
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private ETargetingType _targetingType;

    private void OnEnable()
    {
        Invoke("ReturnPool", LifeTime);
    }

    public void Setup(GameObject enemy, float damageDuplicator) 
    {
        if (enemy == null) ReturnPool();
        _target = enemy;
        _damageDuplicator = damageDuplicator;
        transform.position = GameObject.FindGameObjectWithTag("Player").transform.position + _positionOffset;
    }
    public void Setup(Vector3 position, float damageDuplicator)
    {
        if (position == null) ReturnPool();
        _damageDuplicator = damageDuplicator;
        transform.position = position + _positionOffset;
    }

    private void Update()
    {
        switch (_targetingType) 
        {
            case ETargetingType.NonTarget:
                break;
            case ETargetingType.Target:
                transform.position = _target.transform.position + _positionOffset;
                break;
            case ETargetingType.Projectile:
                transform.Translate(Vector3.right * _projectileSpeed * Time.deltaTime);
                break;
        }
    }
    // 플레이어 공격방식 준수
    // Animation Event가 부를 함수
    public void OnAttackHit()
    {
        if (_targetingType == ETargetingType.Projectile) return; //투사체면 무시
        GiveDamage();
    }

    private void GiveDamage() 
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            _effectRange,
            _monsterLayer
        );
        if (hits.Length > 0)
        {
            IDamageable target = hits[0].GetComponent<IDamageable>();
            if (target != null)
            {
                //여기서 스킬 배율 곱하기
                BigNumber damage = PlayerStatManager.Instance.AttackPower * new BigNumber(_damageDuplicator);
                // 크리티컬
                if (Random.value < PlayerStatManager.Instance.CritRate)
                {
                    damage *= PlayerStatManager.Instance.CritDamage;
                    target.TakeDamage(damage, true);
                }
                else target.TakeDamage(damage);
                // 이펙트나 사운드 넣으면 될 듯
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_targetingType != ETargetingType.Projectile) return; //투사체 아니면 무시
        GiveDamage();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _effectRange);
    }

    public void SetPool(IPool pool) { _pool = pool; }
    public void ReturnPool() { _pool.Enqueue(this); }
}
