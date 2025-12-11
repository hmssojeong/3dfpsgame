using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("폭발 설정")]
    public GameObject _explosionEffectPrefab;
    [SerializeField] private float _explosionDelay = 0f;
    [SerializeField] private float _explosionRadius = 5f;  
    [SerializeField] private float _explosionDamage = 50f; 


    private BombPool _pool;

    public void SetPool(BombPool pool)
    {
        _pool = pool;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 폭발 효과 생성
        if (_explosionEffectPrefab != null)
        {
            GameObject effectObject = Instantiate(_explosionEffectPrefab);
            effectObject.transform.position = transform.position;
        }

        ExplosionDamage();

        // 풀로 반환 (Destroy 대신)
        if (_pool != null)
        {
            // 약간의 지연 후 반환 (선택사항)
            if (_explosionDelay > 0f)
            {
                Invoke(nameof(ReturnToPool), _explosionDelay);
            }
            else
            {
                ReturnToPool();
            }
        }
        else
        {
            // 풀이 없으면 파괴
            Destroy(gameObject);
        }
    }

    private void ExplosionDamage()
    {
        // 폭발 범위 내의 모든 콜라이더 검색
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _explosionRadius);

        foreach (Collider hitCollider in hitColliders)
        {
            // Monster 컴포넌트 확인
            Monster monster = hitCollider.GetComponent<Monster>();

            if (monster != null)
            {
                // 폭탄 위치를 공격자 위치로 전달
                bool damaged = monster.TryTakeDamage(_explosionDamage, transform.position);

            }
        }
    }
    private void ReturnToPool()
    {
        if (_pool != null)
        {
            _pool.ReturnBomb(this);
        }
    }

    public void Explode()
    {
        if (_explosionEffectPrefab != null)
        {
            GameObject effectObject = Instantiate(_explosionEffectPrefab);
            effectObject.transform.position = transform.position;
        }

        ExplosionDamage();

        ReturnToPool();
    }
}