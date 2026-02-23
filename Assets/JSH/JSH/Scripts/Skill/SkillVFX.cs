using System.Collections.Generic;
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
    private ElementType _elementType;

    [SerializeField] private float _effectRange;
    [SerializeField] private float _projectileSpeed = 5f;
    [SerializeField] private LayerMask _monsterLayer;
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private ETargetingType _targetingType;

    private void OnEnable()
    {
        Invoke("ReturnPool", LifeTime);
    }

    public void Setup(ElementType element, GameObject enemy, float damageDuplicator) 
    {
        if (enemy == null) ReturnPool();
        _elementType = element;
        _target = enemy;
        _damageDuplicator = damageDuplicator;
        transform.position = GameObject.FindGameObjectWithTag("Player").transform.position + _positionOffset;
    }
    public void Setup(ElementType element, Vector3 position, float damageDuplicator)
    {
        if (position == null) ReturnPool();
        _elementType = element;
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
        //if (_targetingType == ETargetingType.Projectile) return; //투사체면 무시
        EnemyFiltering();
    }

    private void EnemyFiltering(bool hitAll = true, int maxTargets = 1) 
    {
        //적당히 화면 내에 있는 적 가져오기
        List<IDamageable> enemies = SkillManager.Instance.CheckEnemy(5f);
        if (enemies.Count <= 0) return;
        //실제 이펙트 원형 범위 체크
        List<IDamageable> validEnemies = new List<IDamageable>();
        Vector2 centerA = transform.position;
        float radiusA = _effectRange;

        foreach (var enemy in enemies) 
        {
            if (enemy is MonoBehaviour mono && mono.gameObject.activeSelf)
            {
                Vector2 centerB = mono.transform.position;
                float radiusB = 0.5f;

                if (IsCollisionCircle(centerA, radiusA, centerB, radiusB)) { validEnemies.Add(enemy); }
            }
        }

        if (validEnemies.Count <= 0) return;
        //범위 공격 or 가까운 적 N명
        if (hitAll)
        {
            foreach (var enemy in validEnemies) { GiveDamage(enemy); }
        }
        else 
        {
            Vector2 playerPos = SkillManager.Instance.GetPlayer().transform.position;
            validEnemies.Sort((a, b) =>
            {
                Vector2 posA = (a as MonoBehaviour).transform.position;
                Vector2 posB = (b as MonoBehaviour).transform.position;
                float distA = (playerPos - posA).sqrMagnitude;
                float distB = (playerPos - posB).sqrMagnitude;
                return distA.CompareTo(distB); //distA 가 distB 보다 크면 1, 작으면 -1, 같으면 0
            });
                                //둘 중 더 작은거 반환
            for (int i = 0; i < Mathf.Min(maxTargets, validEnemies.Count); i++)  
            {
                GiveDamage(validEnemies[i]);
            }
        }
    }

    private void GiveDamage(IDamageable target) 
    {
        if (target != null)
        {
            //여기서 스킬 배율 곱하기
            BigNumber damage = PlayerStatManager.Instance.AttackPower * new BigNumber(_damageDuplicator);
            // 크리티컬
            if (Random.value < PlayerStatManager.Instance.CritRate)
            {
                damage *= PlayerStatManager.Instance.CritDamage;
                target.TakeDamage(damage, true, _elementType);
            }
            else target.TakeDamage(damage, false, _elementType);
            // 이펙트나 사운드 넣으면 될 듯
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (_targetingType != ETargetingType.Projectile) return; //투사체 아니면 무시
    //    EnemyFiltering();
    //}

    private bool IsCollisionCircle(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB) 
    {
        //원 충돌 판정 = 센터A & 센터B 사이의 거리 <= 반지름A + 반지름B
        float sqrDistance = (centerA - centerB).sqrMagnitude; //이게 Vector2.Distance보다 좋다고 함(제곱근연산 없어서)
        float sqrRadius = (radiusA + radiusB) * (radiusA + radiusB); //위에가 두 센터 사이 거리 제곱이라 얘도 제곱
        return sqrDistance <= sqrRadius;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _effectRange);
    }

    public void SetPool(IPool pool) { _pool = pool; }
    public void ReturnPool() { _pool.Enqueue(this); }
}
