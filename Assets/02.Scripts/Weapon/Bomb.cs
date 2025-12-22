using UnityEditor.PackageManager;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("폭발 설정")]
    public GameObject _explosionEffectPrefab;
    [SerializeField] private float _explosionDelay = 0f;
    [SerializeField] private float _explosionRadius = 5f;  
    [SerializeField] private float _explosionDamage = 50f;
    [SerializeField] private GameObject _player;

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
        // 가상의 구를 만들어서 그 구 영역 안에 있는 Monster 레이어의 모든 콜라이더를 찾아서 배열로 반환
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius, 1 << LayerMask.NameToLayer("Monster"));

        for (int i = 0; i < colliders.Length; i++)
        {
            Monster monster = colliders[i].gameObject.GetComponent<Monster>();

            if (monster != null)
            {
                // 폭발 원점과의 거리 계산
                float distance = Vector3.Distance(transform.position, monster.transform.position);
                // 거리가 너무 가까우면 최소 1로 설정 (0으로 나누기 방지)
                distance = Mathf.Max(1f, distance);

                // 거리에 따라 데미지 감쇠 (가까울수록 높은 데미지)
                float finalDamage = _explosionDamage / distance;
                 Damage damage = new Damage()
                {
                    Value = finalDamage,
                 };
                // 몬스터에게 데미지 적용
                monster.TryTakeDamage(damage);
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