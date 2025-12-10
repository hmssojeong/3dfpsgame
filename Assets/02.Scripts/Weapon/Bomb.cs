using UnityEngine;

/// <summary>
/// 폭탄 오브젝트 (오브젝트 풀링 지원)
/// 충돌 시 폭발하고 풀로 돌아갑니다
/// </summary>
public class Bomb : MonoBehaviour
{
    [Header("폭발 설정")]
    public GameObject _explosionEffectPrefab;
    [SerializeField] private float _explosionDelay = 0f;  // 폭발 지연 시간 (선택사항)

    private BombPool _pool;  // 소속된 풀

    /// <summary>
    /// 풀 설정 (BombPool에서 호출)
    /// </summary>
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
            // 풀이 없으면 기존 방식대로 파괴
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 풀로 돌아가기
    /// </summary>
    private void ReturnToPool()
    {
        if (_pool != null)
        {
            _pool.ReturnBomb(this);
        }
    }

    /// <summary>
    /// 수동으로 폭탄 반환 (시간 경과 후 등)
    /// </summary>
    public void Explode()
    {
        if (_explosionEffectPrefab != null)
        {
            GameObject effectObject = Instantiate(_explosionEffectPrefab);
            effectObject.transform.position = transform.position;
        }

        ReturnToPool();
    }
}