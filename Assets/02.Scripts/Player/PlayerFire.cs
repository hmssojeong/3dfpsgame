using UnityEngine;

/// <summary>
/// 플레이어 폭탄 발사 시스템 (오브젝트 풀링 버전)
/// 마우스 우클릭으로 폭탄을 던지며, 풀에서 가져와 재사용합니다
/// </summary>
public class PlayerFire : MonoBehaviour
{
    [Header("폭탄 설정")]
    [SerializeField] private Transform _fireTransform;
    [SerializeField] private float _throwPower = 15f;

    [Header("탄약 설정")]
    [SerializeField] private int _maxBombCount = 5;  // 최대 폭탄 개수
    [SerializeField] private float _reloadTime = 2f;  // 재장전 시간 (초)

    // 현재 상태
    private int _currentBombCount;
    private float _reloadTimer = 0f;
    private bool _isReloading = false;

    // 이벤트: UI 업데이트용
    public static System.Action<int, int> OnBombCountChanged;  // (현재, 최대)

    private void Start()
    {
        // 시작 시 최대 개수로 초기화
        _currentBombCount = _maxBombCount;
        UpdateUI();
    }

    private void Update()
    {
        // 재장전 처리
        HandleReload();

        // 마우스 우클릭으로 폭탄 발사
        if (Input.GetMouseButtonDown(1))
        {
            TryThrowBomb();
        }

        // R키로 수동 재장전
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartReload();
        }
    }

    /// <summary>
    /// 폭탄 던지기 시도
    /// </summary>
    private void TryThrowBomb()
    {
        // 폭탄이 없거나 재장전 중이면 불가
        if (_currentBombCount <= 0)
        {
            Debug.Log("폭탄이 없습니다! (R키로 재장전)");
            return;
        }

        if (_isReloading)
        {
            Debug.Log("재장전 중...");
            return;
        }

        // BombPool 확인
        if (BombPool.Instance == null)
        {
            Debug.LogError("BombPool이 없습니다! BombPool을 씬에 추가하세요!");
            return;
        }

        // 풀에서 폭탄 가져오기 (Instantiate 대신!)
        Bomb bomb = BombPool.Instance.GetBomb(_fireTransform.position, Quaternion.identity);

        if (bomb == null)
        {
            Debug.LogError("풀에서 폭탄을 가져올 수 없습니다!");
            return;
        }

        // Rigidbody로 던지기
        Rigidbody rigidbody = bomb.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.AddForce(Camera.main.transform.forward * _throwPower, ForceMode.Impulse);
        }

        // 폭탄 개수 감소
        _currentBombCount--;
        UpdateUI();

        Debug.Log($"폭탄 발사! 남은 개수: {_currentBombCount}/{_maxBombCount}");

        // 폭탄이 0개가 되면 자동 재장전
        if (_currentBombCount <= 0)
        {
            StartReload();
        }
    }

    /// <summary>
    /// 재장전 시작
    /// </summary>
    private void StartReload()
    {
        // 이미 최대치면 재장전 불필요
        if (_currentBombCount >= _maxBombCount)
        {
            Debug.Log("이미 최대 개수입니다!");
            return;
        }

        // 이미 재장전 중이면 무시
        if (_isReloading)
        {
            return;
        }

        _isReloading = true;
        _reloadTimer = _reloadTime;
        Debug.Log($"재장전 시작... ({_reloadTime}초)");
    }

    /// <summary>
    /// 재장전 처리
    /// </summary>
    private void HandleReload()
    {
        if (!_isReloading) return;

        _reloadTimer -= Time.deltaTime;

        if (_reloadTimer <= 0f)
        {
            // 재장전 완료
            _currentBombCount = _maxBombCount;
            _isReloading = false;
            _reloadTimer = 0f;

            UpdateUI();
            Debug.Log($"재장전 완료! {_currentBombCount}/{_maxBombCount}");
        }
    }

    /// <summary>
    /// UI 업데이트
    /// </summary>
    private void UpdateUI()
    {
        OnBombCountChanged?.Invoke(_currentBombCount, _maxBombCount);
    }

    /// <summary>
    /// 외부에서 현재 폭탄 개수 확인
    /// </summary>
    public int CurrentBombCount => _currentBombCount;

    /// <summary>
    /// 외부에서 최대 폭탄 개수 확인
    /// </summary>
    public int MaxBombCount => _maxBombCount;

    /// <summary>
    /// 재장전 진행도 (0~1)
    /// </summary>
    public float ReloadProgress
    {
        get
        {
            if (!_isReloading) return 1f;
            return 1f - (_reloadTimer / _reloadTime);
        }
    }

    /// <summary>
    /// 재장전 중인지 확인
    /// </summary>
    public bool IsReloading => _isReloading;
}