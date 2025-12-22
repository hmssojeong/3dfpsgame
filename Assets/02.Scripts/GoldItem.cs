using System.Collections;
using UnityEngine;

public class GoldItem : MonoBehaviour
{
    [Header("골드 정보")]
    [SerializeField] private int _goldAmount = 10;

    [Header("습득 설정")]
    [SerializeField] private float _attractDistance = 5f;
    [SerializeField] private float _collectDistance = 1f;
    [SerializeField] private float _attractSpeed = 8f;
    [SerializeField] private float _bezierHeight = 2f;

    [Header("물리 설정")]
    [SerializeField] private float _dropForce = 3f; // 퍼지는 힘 (줄임)
    [SerializeField] private float _dropUpForce = 4f; // 위로 튀는 힘
    [SerializeField] private float _settleTime = 1f; // 정착 시간

    private Transform _player;
    private Rigidbody _rigidbody;
    private Collider _collider;
    private bool _isAttracted = false;
    private bool _isGrounded = false;
    private Vector3 _startPosition;
    private float _attractTimer = 0f;

    [Header("회전 효과")]
    [SerializeField] private float _rotationSpeed = 360f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            _rigidbody = gameObject.AddComponent<Rigidbody>();
        }

        _collider = GetComponent<Collider>();
        if (_collider == null)
        {
            _collider = gameObject.AddComponent<SphereCollider>();
        }

        // Rigidbody 설정
        _rigidbody.mass = 0.1f;
        _rigidbody.linearDamping = 0.5f; // 공기 저항
        _rigidbody.angularDamping = 2f; // 회전 저항
        _rigidbody.useGravity = true;
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (_player == null)
        {
            Debug.LogWarning("[골드] 플레이어를 찾을 수 없습니다!");
        }
    }

    public void Drop(Vector3 dropPosition)
    {
        transform.position = dropPosition;

        // 랜덤 방향으로 힘 가하기 (normalized 사용)
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 force = new Vector3(randomDirection.x, 0f, randomDirection.y) * _dropForce;
        force.y = _dropUpForce;

        _rigidbody.AddForce(force, ForceMode.Impulse);

        // 약간의 회전력 추가 (너무 세지 않게)
        _rigidbody.AddTorque(Random.insideUnitSphere * 0.5f, ForceMode.Impulse);

        // 일정 시간 후 정착
        StartCoroutine(Settle_Coroutine());
    }

    /// <summary>
    /// 일정 시간 후 물리를 끄고 바닥에 정착
    /// </summary>
    private IEnumerator Settle_Coroutine()
    {
        // 물리 시뮬레이션 시간 (튕기고 굴러다니는 시간)
        yield return new WaitForSeconds(_settleTime);

        // 물리 끄기
        _rigidbody.isKinematic = true;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        // 바닥에 정착
        _isGrounded = true;

        Debug.Log("[골드] 바닥에 정착");
    }

    /// <summary>
    /// 충돌 시 처리 (바닥에 닿았을 때)
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        // 바닥에 닿으면 약간의 마찰 추가
        if (_rigidbody != null && !_isGrounded)
        {
            // 속도를 줄여서 빨리 정착하도록
            _rigidbody.linearVelocity *= 0.7f;
        }
    }

    private void Update()
    {
        if (_player == null) return;

        // 땅에 닿지 않았으면 플레이어 끌어당기기 안함
        if (!_isGrounded) return;

        // Y축 회전 (빙글빙글)
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        // 플레이어가 습득 거리 안에 있으면 즉시 습득
        if (distanceToPlayer <= _collectDistance)
        {
            CollectGold();
            return;
        }

        // 플레이어가 감지 거리 안에 있으면 끌어당기기 시작
        if (!_isAttracted && distanceToPlayer <= _attractDistance)
        {
            _isAttracted = true;
            _startPosition = transform.position;
            _attractTimer = 0f;
        }

        // 베지어 곡선으로 플레이어에게 이동
        if (_isAttracted)
        {
            MoveToPlayerWithBezier();
        }
    }

    private void MoveToPlayerWithBezier()
    {
        _attractTimer += Time.deltaTime * _attractSpeed;
        float t = Mathf.Clamp01(_attractTimer);

        // 베지어 곡선 제어점 계산
        Vector3 p0 = _startPosition; // 시작점
        Vector3 p2 = _player.position + Vector3.up * 0.5f; // 끝점 (플레이어 중심)
        Vector3 p1 = (p0 + p2) / 2f + Vector3.up * _bezierHeight; // 제어점 (중간 위쪽)

        // Quadratic Bezier 공식: B(t) = (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2
        Vector3 newPosition = Mathf.Pow(1 - t, 2) * p0 +
                             2 * (1 - t) * t * p1 +
                             Mathf.Pow(t, 2) * p2;

        transform.position = newPosition;

        // 끝에 도달하면 습득
        if (t >= 1f)
        {
            CollectGold();
        }
    }


    private void CollectGold()
    {
        PlayerGold playerGold = _player.GetComponent<PlayerGold>();
        if (playerGold != null)
        {
            playerGold.AddGold(_goldAmount);
            Debug.Log($"[골드] {_goldAmount} 골드 획득!");
        }

        // TODO: 골드 습득 사운드 재생
        // AudioManager.Instance.PlaySFX("GoldCollect");

        Destroy(gameObject);
    }

    // 외부에서 골드 양 설정
    public void SetGoldAmount(int amount)
    {
        _goldAmount = amount;
    }
}