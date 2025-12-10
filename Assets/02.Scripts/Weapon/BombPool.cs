using UnityEngine;
using UnityEngine.Pool;

public class BombPool : MonoBehaviour
{
    [Header("풀 설정")]
    [SerializeField] private Bomb _bombPrefab;              // 풀링할 폭탄 프리팹
    [SerializeField] private int _defaultCapacity = 10;     // 기본 용량
    [SerializeField] private int _maxSize = 20;             // 최대 크기

    // Unity ObjectPool
    private ObjectPool<Bomb> _bombPool;
    private Transform _poolParent;  // 풀 오브젝트들의 부모

    public static BombPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 풀 초기화
        InitializePool();
    }

    private void InitializePool()
    {
        // 풀 부모 오브젝트 생성
        _poolParent = new GameObject("BombPool").transform;
        _poolParent.SetParent(transform);

        // ObjectPool 생성
        _bombPool = new ObjectPool<Bomb>(
            createFunc: OnCreateBomb,           // 생성 시
            actionOnGet: OnGetBomb,             // 가져올 때
            actionOnRelease: OnReleaseBomb,     // 반환할 때
            actionOnDestroy: OnDestroyBomb,     // 파괴할 때
            collectionCheck: true,              // 중복 반환 체크
            defaultCapacity: _defaultCapacity,  // 기본 용량
            maxSize: _maxSize                   // 최대 크기
        );
    }

    private Bomb OnCreateBomb()
    {
        Bomb bomb = Instantiate(_bombPrefab, _poolParent);
        bomb.name = $"Bomb_{System.Guid.NewGuid().ToString().Substring(0, 8)}";

        // 폭탄에게 풀 참조 전달
        bomb.SetPool(this);

        return bomb;
    }

    private void OnGetBomb(Bomb bomb)
    {
        // 활성화
        bomb.gameObject.SetActive(true);

        // Rigidbody 초기화
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

    }

    private void OnReleaseBomb(Bomb bomb)
    {
        // 비활성화
        bomb.gameObject.SetActive(false);

        // 부모 설정
        bomb.transform.SetParent(_poolParent);

        // Rigidbody 정리
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }


    private void OnDestroyBomb(Bomb bomb)
    {
        Destroy(bomb.gameObject);
    }

    public Bomb GetBomb(Vector3 position, Quaternion rotation)
    {
        // ObjectPool에서 가져오기
        Bomb bomb = _bombPool.Get();

        if (bomb != null)
        {
            // 위치와 회전 설정
            bomb.transform.position = position;
            bomb.transform.rotation = rotation;
        }

        return bomb;
    }

    public void ReturnBomb(Bomb bomb)
    {
        if (bomb == null) return;

        // ObjectPool에 반환
        _bombPool.Release(bomb);
    }

    public void ClearAllBombs()
    {
        // 모든 자식 폭탄을 반환
        foreach (Transform child in _poolParent)
        {
            Bomb bomb = child.GetComponent<Bomb>();
            if (bomb != null && bomb.gameObject.activeInHierarchy)
            {
                ReturnBomb(bomb);
            }
        }
    }

    public void DisposePool()
    {
        if (_bombPool != null)
        {
            _bombPool.Clear();
        }
    }

    private void OnDestroy()
    {
        // 풀 정리
        DisposePool();
    }

    public void PrintPoolStatus()
    {
        int activeCount = 0;
        int totalCount = 0;

        foreach (Transform child in _poolParent)
        {
            totalCount++;
            if (child.gameObject.activeInHierarchy)
            {
                activeCount++;
            }
        }
    }
}