using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.UI.Image;

public class Monster : MonoBehaviour
{
    #region Comment
    // 목표: 처음에는 가만히 있지만 플레이어가 다가가면 쫓아오는 좀비 몬스터를 만들고 싶다.
    //       ㄴ 쫓아 오다가 너무 멀어지면 제자리로 돌아간다.

    // Idle   : 가만히 있는다.
    //   I  (플레이어가 가까이 오면) (컨디션, 트랜지션)
    // Trace  : 플레이러를 쫒아간다.
    //   I  (플레이어와 너무 멀어지면)
    // Return : 제자리로 돌아가는 상태
    //   I  (제자리에 도착했다면)
    //  Idle
    // 공격
    // 피격
    // 죽음


    // 몬스터 인공지능(AI) : 사람처럼 행동하는 똑똑한 시스템/알고리즘
    // - 규칙 기반 인공지능 : 정해진 규칙에 따라 조건문/반복문등을 이용해서 코딩하는 것
    //                     -> ex) FSM(유한 상태머신), BT(행동 트리)
    // - 학습 기반 인공지능: 머신러닝(딥러닝, 강화학습 .. )

    // Finite State Machine(유한 상태 머신)
    // N개의 상태를 가지고 있고, 상태마다 행동이 다르다.
    #endregion

    public EMonsterState State = EMonsterState.Idle;
    private PlayerStats _playerStats;
    [SerializeField] private GameObject _player;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private MonsterKnockBack _knockback;

    public ConsumableStat Health;

    private Vector3 _originPos;

    public float DetectDistance = 4f;
    public float AttackDistance = 1.2f;

    public float MoveSpeed = 5f;
    public float AttackSpeed = 2f;
    public float AttackTimer = 0f;
    public float AttackDamage = 10f;

    private const float _patrolNearby = 1.5f;

    private float _originNearby = 0.5f;

    [Header("순찰 시스템")]
    [SerializeField] private bool _usePatrol = true;
    [SerializeField] private int _patrolPointCount = 3;
    [SerializeField] private float _patrolRadius = 5f;
    [SerializeField] private float _patrolWaitTime = 2f;
    [SerializeField] private float _patrolSpeed = 2f;

    private List<Vector3> _patrolPoints;
    private int _currentPatrolIndex = 0;
    private float _patrolTimer = 0f;
    private bool _isWaitingAtPatrolPoint = false;

    private void Start()
    {
        _originPos = transform.position;
        _playerStats = FindAnyObjectByType<PlayerStats>();
        _knockback = GetComponent<MonsterKnockBack>();

        // 순찰 시스템 초기화
        if (_usePatrol)
        {
            InitializePatrolPoints();
            State = EMonsterState.Patrol; // 시작 시 순찰 상태로
        }
    }

    private void Update()
    {
        // 몬스터의 상태에 따라 다른 행동을한다. (다른 메서드를 호출한다.)
        switch (State)
        {
            case EMonsterState.Idle:
                Idle();
                break;

            case EMonsterState.Patrol:
                Patrol();
                break;

            case EMonsterState.Trace:
                Trace();
                break;

            case EMonsterState.Comeback:
                Comeback();
                break;

            case EMonsterState.Attack:
                Attack();
                break;

            case EMonsterState.Hit:
                break;

            case EMonsterState.Death:
                break;
        }
    }

    private void InitializePatrolPoints()
    {
        // 가능한 순찰 오프셋 위치들 (원점 기준)
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

        // 랜덤하게 순찰 지점 선택
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
        // 순찰 지점이 없으면 Idle로 전환
        if (_patrolPoints == null || _patrolPoints.Count == 0)
        {
            State = EMonsterState.Idle;
            return;
        }

        // 플레이어가 탐지범위 안에 있다면 추적 시작
        if (Vector3.Distance(transform.position, _player.transform.position) <= DetectDistance)
        {
            State = EMonsterState.Trace;
            _isWaitingAtPatrolPoint = false;
            _patrolTimer = 0f;
            Debug.Log("상태 전환: Patrol -> Trace");
            return;
        }

        // 순찰 지점에서 대기 중이라면
        if (_isWaitingAtPatrolPoint)
        {
            _patrolTimer += Time.deltaTime;

            if (_patrolTimer >= _patrolWaitTime)
            {
                // 대기 완료, 다음 지점으로
                _isWaitingAtPatrolPoint = false;
                _patrolTimer = 0f;
                
                _currentPatrolIndex++;

                if (_currentPatrolIndex >= _patrolPoints.Count)
                {
                    _currentPatrolIndex = 0;
                }
            }
            return;
        }

        // 현재 목표 지점으로 이동
        Vector3 targetPoint = _patrolPoints[_currentPatrolIndex];
        float distanceToTarget = Vector3.Distance(transform.position, targetPoint);

        // 목표 지점에 도착했다면
        if (distanceToTarget <= _originNearby)
        {
            _isWaitingAtPatrolPoint = true;
            _patrolTimer = 0f;
            Debug.Log($"순찰 지점 {_currentPatrolIndex} 도착, 대기 시작");
            return;
        }

        // 목표 지점으로 이동
        Vector3 direction = (targetPoint - transform.position).normalized;
        _controller.Move(direction * _patrolSpeed * Time.deltaTime);
    }

    // 1. 함수는 한 가지 일만 잘해야 한다.
    // 2. 상태별 행동을 함수로 만든다.
    private void Idle()
    {
        // 대기하는 상태
        // Todo. Idle 애니메이션 실행

        // 플레이어가 탐지범위 안에 있다면...
        if (Vector3.Distance(transform.position, _player.transform.position) <= DetectDistance)
        {
            State = EMonsterState.Trace;
            Debug.Log("상태 전환: Idle -> Trace");
        }
    }

    private void Trace()
    {
        // 플레이어를 쫓아가는 상태
        // Todo. Run 애니메이션 실행

        // Comback 과제

        float distance = Vector3.Distance(transform.position, _player.transform.position);

        // 1. 플레이어를 향하는 방향을 구한다.
        Vector3 direction = (_player.transform.position - transform.position).normalized;
        // 2. 방향에 따라 이동한다.
        _controller.Move(direction * MoveSpeed * Time.deltaTime);

        // 플레이어와의 거리가 공격범위내라면
        if (distance <= AttackDistance)
        {
            State = EMonsterState.Attack;
        }

        if (distance > DetectDistance)
        {
            State = EMonsterState.Comeback;
            Debug.Log("집에 돌아왔습니다");
        }
    }

    private void Comeback()
    {
        // 과제 1. 제자리로 복귀하는 상태

        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        float distanceToOrigin = Vector3.Distance(transform.position, _originPos);

        // 원래 위치에 도착했다면
        if (distanceToOrigin <= 0.5f)
        {
            // 순찰을 사용한다면 순찰 상태로, 아니면 Idle로
            if (_usePatrol)
            {
                State = EMonsterState.Patrol;
                _currentPatrolIndex = 0; // 순찰 경로 처음부터 다시
                _isWaitingAtPatrolPoint = false;
                _patrolTimer = 0f;
                Debug.Log("상태 전환: Comeback -> Patrol");
            }
            else
            {
                State = EMonsterState.Idle;
                Debug.Log("상태 전환: Comeback -> Idle");
            }
            return;
        }

        // 원래 위치로 돌아가기
        Vector3 direction = (_originPos - transform.position).normalized;
        _controller.Move(direction * MoveSpeed * Time.deltaTime);

        // 복귀 중에 플레이어가 다시 가까워지면 추적 재개
        if (distanceToPlayer <= DetectDistance)
        {
            State = EMonsterState.Trace;
            Debug.Log("다시 쫒아갑니다.");
        }
    }

    private void Attack()
    {
        // 플레이어를 공격하는 상태

        // 플레이어와의 거리가 멀다면 다시 쫒아오는 상태로 전환
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        if (distance > AttackDistance)
        {
            State = EMonsterState.Trace;
            Debug.Log("상태 전환: Attack -> Trace");
            return;
        }

        AttackTimer += Time.deltaTime;
        if (AttackTimer >= AttackSpeed)
        {
            AttackTimer = 0f;
            Debug.Log("플레이어 공격!");

            if(_playerStats == null)
            {
                Debug.Log("playerStat이 없습니다.");
            }

            if(_playerStats != null)
            {
                // 과제 2번. 플레이어 공격하기
                _playerStats.PlayerTakeDamage(AttackDamage);
            }

        }

    }

    private Vector3 _lastAttackerPos;

    public bool TryTakeDamage(float damage, Vector3 attackerPos)
    {
        if (State == EMonsterState.Hit || State == EMonsterState.Death)
        {
            return false;
        }

        Health.Consume(damage);

        _lastAttackerPos = attackerPos; // 공격자 위치저장

        if (Health.Value > 0)
        {
            // 히트상태
            State = EMonsterState.Hit;
            StartCoroutine(Hit_Coroutine());
            Debug.Log("히트중입니다");
        }
        else
        {
            // 데스상태
            Debug.Log($"상태 전환: {State} -> Death");
            State = EMonsterState.Death;
            StartCoroutine(Death_Coroutine());
        }

        return true;

    }

    private IEnumerator Hit_Coroutine()
    {
        _knockback.ApplyKnockback(_player.transform.position);
        Debug.Log("넉백당했습니다");

        yield return new WaitForSeconds(0.2F);
        State = EMonsterState.Idle;
    }

    private IEnumerator Death_Coroutine()
    {
        yield return new WaitForSeconds(2F);
        State = EMonsterState.Idle;
        Debug.Log("몬스터 죽음");
        Destroy(gameObject);
    }
}
