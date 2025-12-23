using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EliteMonster : MonoBehaviour, IDamageable
{
    public EEliteMonsterState State = EEliteMonsterState.Idle;

    private PlayerStats _playerStats;
    [SerializeField] private GameObject _player;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private EliteMonsterKnockBack _knockback;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;

    public ConsumableStat Health;

    private Vector3 _originPos;
    private EEliteMonsterState _stateBeforeHit;

    [Header("기본 스탯")]
    public float DetectDistance = 7.5f;
    public float AttackDistance = 3.5f;
    public float MoveSpeed = 4f;
    public float AttackSpeed = 3f;
    public float AttackTimer = 0f;
    public float AttackDamage = 30f;

    [Header("엘리트 특수 능력")]
    [SerializeField] private float _detectWarningTime = 2f;
    private float _detectTimer = 0f;

    [SerializeField] private float _chargeSpeed = 8f;
    [SerializeField] private float _chargeDistance = 15f;
    [SerializeField] private float _chargeDamage = 20f;
    private float _chargeDistanceTraveled = 0f;
    private Vector3 _chargeDirection;

    [SerializeField] private float _heavyAttackRange = 5f;
    [SerializeField] private float _heavyAttackDamage = 20f;
    [SerializeField] private float _heavyAttackCooldown = 6f;
    private float _heavyAttackTimer = 0f;

    [Header("순찰 시스템")]
    [SerializeField] private bool _usePatrol = true;
    [SerializeField] private int _patrolPointCount = 5;
    [SerializeField] private float _patrolRadius = 8f;
    [SerializeField] private float _patrolWaitTime = 3f;
    [SerializeField] private float _patrolSpeed = 2f;

    private List<Vector3> _patrolPoints;
    private int _currentPatrolIndex = 0;
    private float _patrolTimer = 0f;
    private bool _isWaitingAtPatrolPoint = false;

    private const float _patrolNearby = 1.5f;
    private const float _originNearby = 0.5f;

    [Header("골드 드랍")]
    [SerializeField] private GameObject _goldPrefab;
    [SerializeField] private int _minGoldDrop = 10;
    [SerializeField] private int _maxGoldDrop = 20;
    [SerializeField] private int _goldValue = 10;

    private void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        _originPos = transform.position;
        _playerStats = FindAnyObjectByType<PlayerStats>();
        _knockback = GetComponent<EliteMonsterKnockBack>();

        _agent.speed = MoveSpeed;
        _agent.stoppingDistance = AttackDistance;

        if (_usePatrol)
        {
            InitializePatrolPoints();
            State = EEliteMonsterState.Patrol;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.State != EGameState.Playing)
        {
            return;
        }

        switch (State)
        {
            case EEliteMonsterState.Idle:
                Idle();
                break;

            case EEliteMonsterState.Patrol:
                Patrol();
                break;

            case EEliteMonsterState.Detect:
                Detect();
                break;

            case EEliteMonsterState.Charge:
                Charge();
                break;

            case EEliteMonsterState.Trace:
                Trace();
                break;

            case EEliteMonsterState.Attack:
                Attack();
                break;

            case EEliteMonsterState.HeavyAttack:
                HeavyAttack();
                break;

            case EEliteMonsterState.Hit:
                break;

            case EEliteMonsterState.Comeback:
                Comeback();
                break;

            case EEliteMonsterState.Death:
                break;
        }

        if (_heavyAttackTimer > 0)
        {
            _heavyAttackTimer -= Time.deltaTime;
        }
    }

    private void InitializePatrolPoints()
    {
        List<Vector3> possibleOffsets = new List<Vector3>
        {
            new Vector3(0, 0, 0),
            new Vector3(_patrolRadius, 0, _patrolRadius),
            new Vector3(-_patrolRadius, 0, -_patrolRadius),
            new Vector3(_patrolRadius, 0, -_patrolRadius),
            new Vector3(-_patrolRadius, 0, _patrolRadius),
            new Vector3(_patrolRadius * _patrolNearby, 0, 0),
            new Vector3(-_patrolRadius * _patrolNearby, 0, 0),
            new Vector3(0, 0, _patrolRadius * _patrolNearby),
            new Vector3(0, 0, -_patrolRadius * _patrolNearby),
        };

        _patrolPoints = new List<Vector3>();
        List<Vector3> offsetsCopy = new List<Vector3>(possibleOffsets);

        for (int i = 0; i < _patrolPointCount && offsetsCopy.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, offsetsCopy.Count);
            Vector3 worldPosition = _originPos + offsetsCopy[randomIndex];
            _patrolPoints.Add(worldPosition);
            offsetsCopy.RemoveAt(randomIndex);
        }

        _currentPatrolIndex = 0;
    }

    private void Patrol()
    {
        if (_patrolPoints == null || _patrolPoints.Count == 0)
        {
            State = EEliteMonsterState.Idle;
            return;
        }

        if (Vector3.Distance(transform.position, _player.transform.position) <= DetectDistance)
        {
            State = EEliteMonsterState.Detect;
            _detectTimer = 0f;
            _isWaitingAtPatrolPoint = false;
            _patrolTimer = 0f;
            Debug.Log("[엘리트] 상태 전환: Patrol -> Detect");
            _animator.SetTrigger("PatrolToDetect");
            return;
        }

        if (_isWaitingAtPatrolPoint)
        {
            _patrolTimer += Time.deltaTime;
            if (_patrolTimer >= _patrolWaitTime)
            {
                _isWaitingAtPatrolPoint = false;
                _patrolTimer = 0f;
                _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Count;
            }
            return;
        }

        Vector3 targetPoint = _patrolPoints[_currentPatrolIndex];
        float distanceToTarget = Vector3.Distance(transform.position, targetPoint);

        if (distanceToTarget <= _originNearby)
        {
            _isWaitingAtPatrolPoint = true;
            _patrolTimer = 0f;
            return;
        }

        Vector3 direction = (targetPoint - transform.position).normalized;
        _controller.Move(direction * _patrolSpeed * Time.deltaTime);
    }

    private void Idle()
    {
        _animator.SetTrigger("Idle");

        if (Vector3.Distance(transform.position, _player.transform.position) <= DetectDistance)
        {
            State = EEliteMonsterState.Detect;
            _detectTimer = 0f;
            Debug.Log("[엘리트] 상태 전환: Idle -> Detect");
            _animator.SetTrigger("IdleToDetect");
        }
    }

    private void Detect()
    {
        Vector3 directionToPlayer = (_player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        _detectTimer += Time.deltaTime;

        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance > DetectDistance)
        {
            State = EEliteMonsterState.Idle;
            Debug.Log("[엘리트] 상태 전환: Detect -> Idle (플레이어 멀어짐)");
            _animator.SetTrigger("DetectToIdle");
            return;
        }

        if (_detectTimer >= _detectWarningTime)
        {
            State = EEliteMonsterState.Charge;
            _chargeDistanceTraveled = 0f;
            _chargeDirection = directionToPlayer;
            Debug.Log("[엘리트] 상태 전환: Detect -> Charge");
            _animator.SetTrigger("DetectToCharge");
        }
    }

    private void Charge()
    {
        float chargeStep = _chargeSpeed * Time.deltaTime;
        _controller.Move(_chargeDirection * chargeStep);
        _chargeDistanceTraveled += chargeStep;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        if (distanceToPlayer <= AttackDistance)
        {
            _playerStats.PlayerTakeDamage(_chargeDamage);
            Debug.Log($"[엘리트] 돌진 데미지: {_chargeDamage}");

            State = EEliteMonsterState.Attack;
            _animator.SetTrigger("ChargeToAttack");
            return;
        }

        if (_chargeDistanceTraveled >= _chargeDistance)
        {
            State = EEliteMonsterState.Trace;
            Debug.Log("[엘리트] 상태 전환: Charge -> Trace");
            _animator.SetTrigger("ChargeToTrace");
        }
    }

    private void Trace()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        _agent.SetDestination(_player.transform.position);

        if (distance <= AttackDistance)
        {
            if (_heavyAttackTimer <= 0f)
            {
                State = EEliteMonsterState.HeavyAttack;
                Debug.Log("[엘리트] 상태 전환: Trace -> HeavyAttack");
                _animator.SetTrigger("TraceToHeavyAttack");
            }
            else
            {
                State = EEliteMonsterState.Attack;
                Debug.Log("[엘리트] 상태 전환: Trace -> Attack");
                _animator.SetTrigger("TraceToAttack");
            }
            return;
        }

        if (distance > DetectDistance)
        {
            State = EEliteMonsterState.Comeback;
            Debug.Log("[엘리트] 상태 전환: Trace -> Comeback");
            _animator.SetTrigger("TraceToComeback");
        }
    }

    private void Attack()
    {
        float distance = Vector3.Distance(transform.position, _player.transform.position);

        if (distance > AttackDistance)
        {
            State = EEliteMonsterState.Trace;
            Debug.Log("[엘리트] 상태 전환: Attack -> Trace");
            _animator.SetTrigger("AttackToTrace");
            return;
        }

        AttackTimer += Time.deltaTime;
        if (AttackTimer >= AttackSpeed)
        {
            _animator.SetTrigger("Attack");
            AttackTimer = 0f;
            Debug.Log($"[엘리트] 공격 데미지: {AttackDamage}");
        }
    }

    private void HeavyAttack()
    {
        _animator.SetTrigger("HeavyAttack");

        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        if (distanceToPlayer <= _heavyAttackRange)
        {
            _playerStats.PlayerTakeDamage(_heavyAttackDamage);
            Debug.Log($"[엘리트] 강공격 데미지: {_heavyAttackDamage}");
        }

        _heavyAttackTimer = _heavyAttackCooldown;

        StartCoroutine(HeavyAttack_Coroutine());
    }

    private IEnumerator HeavyAttack_Coroutine()
    {
        yield return new WaitForSeconds(1.5f);
        State = EEliteMonsterState.Trace;
        Debug.Log("[엘리트] 상태 전환: HeavyAttack -> Trace");
    }

    private void Comeback()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        float distanceToOrigin = Vector3.Distance(transform.position, _originPos);

        if (distanceToOrigin <= _originNearby)
        {
            if (_usePatrol)
            {
                State = EEliteMonsterState.Patrol;
                _currentPatrolIndex = 0;
                _isWaitingAtPatrolPoint = false;
                _patrolTimer = 0f;
                Debug.Log("[엘리트] 상태 전환: Comeback -> Patrol");
            }
            else
            {
                State = EEliteMonsterState.Idle;
                Debug.Log("[엘리트] 상태 전환: Comeback -> Idle");
            }
            return;
        }

        _agent.SetDestination(_originPos);

        if (distanceToPlayer <= DetectDistance)
        {
            State = EEliteMonsterState.Detect;
            _detectTimer = 0f;
            Debug.Log("[엘리트] 상태 전환: Comeback -> Detect");
            _animator.SetTrigger("ComebackToDetect");
        }
    }

    private Vector3 _lastAttackerPos;

    public bool TryTakeDamage(Damage damage)
    {
        if (State == EEliteMonsterState.Death)
        {
            return false;
        }

        Health.Consume(damage.Value);

        _agent.enabled = false;

        _lastAttackerPos = damage.AttackerPos;

        if (Health.Value > 0)
        {
            _stateBeforeHit = State;
            State = EEliteMonsterState.Hit;
            StartCoroutine(Hit_Coroutine());
            _animator.SetTrigger("Hit");
            Debug.Log("[엘리트] 피격");
        }
        else
        {
            State = EEliteMonsterState.Death;
            StartCoroutine(Death_Coroutine());
            _animator.SetTrigger("Death");
            Debug.Log("[엘리트] 사망");
        }

        return true;
    }

    private IEnumerator Hit_Coroutine()
    {
        // 넉백 적용
        _knockback.ApplyKnockback(_player.transform.position);

        yield return new WaitForSeconds(0.3f);

        _agent.enabled = true;

        State = _stateBeforeHit;
        Debug.Log($"[엘리트] 상태 복구: Hit -> {_stateBeforeHit}");
    }

    private IEnumerator Death_Coroutine()
    {
        if (_controller != null)
        {
            _controller.enabled = false;
        }

        // 죽는 애니메이션 재생 시간
        yield return new WaitForSeconds(3f);

        // 골드 드랍
        DropGold();

        Destroy(gameObject);
    }

    private void DropGold()
    {
        if (_goldPrefab == null)
        {
            Debug.LogWarning("[엘리트] 골드 프리팹이 설정되지 않았습니다!");
            return;
        }

        int dropCount = Random.Range(_minGoldDrop, _maxGoldDrop + 1);
        Vector3 dropPosition = transform.position + Vector3.up * 1f;

        for (int i = 0; i < dropCount; i++)
        {
            GameObject goldObject = Instantiate(_goldPrefab, dropPosition, Quaternion.identity);
            GoldItem goldItem = goldObject.GetComponent<GoldItem>();

            if (goldItem != null)
            {
                goldItem.SetGoldAmount(_goldValue);
                goldItem.Drop(dropPosition);
            }
        }

        Debug.Log($"[엘리트] 골드 {dropCount}개 드랍!");
    }
}