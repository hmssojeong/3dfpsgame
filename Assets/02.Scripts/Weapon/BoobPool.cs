using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 폭탄 오브젝트 풀링 시스템
/// 배열/리스트를 이용하여 폭탄을 미리 생성하고 재사용합니다
/// </summary>
public class BombPool : MonoBehaviour
{
    [Header("풀 설정")]
    [SerializeField] private Bomb _bombPrefab;           // 풀링할 폭탄 프리팹
    [SerializeField] private int _poolSize = 10;         // 풀 크기 (미리 생성할 개수)
    [SerializeField] private bool _expandable = true;    // 부족하면 자동 확장

    // 풀 저장소
    private List<Bomb> _bombPool;
    private Transform _poolParent;  // 풀 오브젝트들의 부모

    // 싱글톤 (간단한 접근용)
    public static BombPool Instance { get; private set; }

    private void Awake()
    {
        // 싱글톤 설정
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

    /// <summary>
    /// 풀 초기화 - 미리 폭탄들을 생성
    /// </summary>
    private void InitializePool()
    {
        // 풀 부모 오브젝트 생성 (Hierarchy 정리용)
        _poolParent = new GameObject("BombPool").transform;
        _poolParent.SetParent(transform);

        // 리스트 초기화
        _bombPool = new List<Bomb>(_poolSize);

        // 폭탄들을 미리 생성
        for (int i = 0; i < _poolSize; i++)
        {
            Bomb bomb = CreateNewBomb();
            _bombPool.Add(bomb);
        }

        Debug.Log($"[BombPool] 폭탄 풀 초기화 완료! ({_poolSize}개)");
    }

    /// <summary>
    /// 새로운 폭탄 생성 (비활성 상태)
    /// </summary>
    private Bomb CreateNewBomb()
    {
        Bomb bomb = Instantiate(_bombPrefab, _poolParent);
        bomb.gameObject.SetActive(false);  // 비활성화
        bomb.name = $"Bomb_{_bombPool.Count}";

        // 폭탄에게 풀로 돌아가는 방법 알려주기
        bomb.SetPool(this);

        return bomb;
    }

    /// <summary>
    /// 풀에서 폭탄 가져오기
    /// </summary>
    public Bomb GetBomb(Vector3 position, Quaternion rotation)
    {
        // 1. 사용 가능한 폭탄 찾기
        foreach (Bomb bomb in _bombPool)
        {
            if (!bomb.gameObject.activeInHierarchy)
            {
                // 찾았다! 활성화하고 위치 설정
                bomb.transform.position = position;
                bomb.transform.rotation = rotation;
                bomb.gameObject.SetActive(true);

                // Rigidbody 초기화
                Rigidbody rb = bomb.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                return bomb;
            }
        }

        // 2. 사용 가능한 폭탄이 없음
        if (_expandable)
        {
            // 확장 가능하면 새로 만들기
            Debug.LogWarning($"[BombPool] 풀이 부족합니다. 확장 중... (현재: {_bombPool.Count})");
            Bomb newBomb = CreateNewBomb();
            _bombPool.Add(newBomb);

            // 재귀 호출로 다시 가져오기
            return GetBomb(position, rotation);
        }
        else
        {
            // 확장 불가능하면 null 반환
            Debug.LogError("[BombPool] 사용 가능한 폭탄이 없습니다!");
            return null;
        }
    }

    /// <summary>
    /// 풀로 폭탄 반환
    /// </summary>
    public void ReturnBomb(Bomb bomb)
    {
        if (bomb == null) return;

        // 비활성화하고 풀 부모 아래로 이동
        bomb.gameObject.SetActive(false);
        bomb.transform.SetParent(_poolParent);

        // Rigidbody 정리
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// 모든 활성 폭탄 반환
    /// </summary>
    public void ReturnAllBombs()
    {
        foreach (Bomb bomb in _bombPool)
        {
            if (bomb.gameObject.activeInHierarchy)
            {
                ReturnBomb(bomb);
            }
        }
    }

    /// <summary>
    /// 현재 풀 상태 확인 (디버그용)
    /// </summary>
    public void PrintPoolStatus()
    {
        int activeCount = 0;
        foreach (Bomb bomb in _bombPool)
        {
            if (bomb.gameObject.activeInHierarchy)
            {
                activeCount++;
            }
        }

        Debug.Log($"[BombPool] 전체: {_bombPool.Count}, 사용 중: {activeCount}, 대기 중: {_bombPool.Count - activeCount}");
    }
}






















































































