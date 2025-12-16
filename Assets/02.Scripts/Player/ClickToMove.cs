using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerStats))]
public class ClickToMove : MonoBehaviour
{
    [Header("필수 컴포넌트")]
    private NavMeshAgent _agent;
    private PlayerStats _stats;
    private PlayerMove _playerMove; // 키보드 이동 스크립트 참조
    private CharacterController _controller;

    [Header("클릭 설정")]
    [SerializeField] private LayerMask _groundLayer; // 클릭 가능한 바닥 레이어
    [SerializeField] private float _stoppingDistance = 0.5f;

    [Header("시각적 피드백 (선택)")]
    [SerializeField] private GameObject _clickMarkerPrefab; // 클릭 위치 표시 마커
    [SerializeField] private float _markerLifetime = 1f; // 마커 지속 시간
    private GameObject _currentMarker;

    [Header("디버그")]
    [SerializeField] private bool _showDebugInfo = false;

    private bool _isMoving = false;
    private Vector3 _targetPosition;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _stats = GetComponent<PlayerStats>();
        _playerMove = GetComponent<PlayerMove>();
        _controller = GetComponent<CharacterController>();

        // NavMeshAgent 초기 설정
        _agent.enabled = false;
        _agent.stoppingDistance = _stoppingDistance;
        _agent.autoBraking = true;
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != EGameState.Playing)
        {
            return;
        }

        // 마우스 우클릭 감지
        if (Input.GetMouseButtonDown(1))
        {
            HandleRightClick();
        }

        // 키보드 입력이 있으면 NavMesh 이동 중단
        if (_isMoving && HasKeyboardInput())
        {
            StopNavMovement();
        }

        // 목적지 도착 체크
        if (_isMoving && _agent.enabled)
        {
            CheckArrival();
        }

        // 디버그 정보 표시
        if (_showDebugInfo && _isMoving)
        {
            Debug.DrawLine(transform.position, _targetPosition, Color.green);
        }
    }

    // 우클릭 처리
    private void HandleRightClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundLayer))
        {
            StartNavMovement(hit.point);

            // 클릭 위치에 마커 표시 (옵션)
            if (_clickMarkerPrefab != null)
            {
                ShowClickMarker(hit.point);
            }

            if (_showDebugInfo)
            {
                Debug.Log($"클릭 위치로 이동: {hit.point}");
            }
        }
    }

    // NavMesh 이동 시작
    private void StartNavMovement(Vector3 destination)
    {
        _targetPosition = destination;

        // PlayerMove 비활성화
        if (_playerMove != null)
        {
            _playerMove.enabled = false;
        }

        // CharacterController 비활성화
        if (_controller != null)
        {
            _controller.enabled = false;
        }

        // NavMeshAgent 활성화
        _agent.enabled = true;
        _agent.speed = _stats.MoveSpeed.Value;

        // NavMesh 위에 있는지 확인
        if (_agent.isOnNavMesh)
        {
            _agent.SetDestination(destination);
            _isMoving = true;
        }
        else
        {
            Debug.LogWarning("플레이어가 NavMesh 위에 없습니다!");
            StopNavMovement();
        }
    }

    // NavMesh 이동 중단
    private void StopNavMovement()
    {
        if (!_isMoving) return;

        _isMoving = false;

        // NavMeshAgent 중단 및 비활성화
        if (_agent.enabled)
        {
            _agent.ResetPath();
            _agent.velocity = Vector3.zero;
            _agent.enabled = false;
        }

        // CharacterController 활성화
        if (_controller != null)
        {
            _controller.enabled = true;
        }

        // PlayerMove 활성화
        if (_playerMove != null)
        {
            _playerMove.enabled = true;
        }

        if (_showDebugInfo)
        {
            Debug.Log("NavMesh 이동 중단");
        }
    }

    // 목적지 도착 확인
    private void CheckArrival()
    {
        if (!_agent.pathPending)
        {
            // 남은 거리가 정지 거리 이하면 도착
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                // 목적지에 거의 도착했거나 경로가 끝났을 때
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    StopNavMovement();
                }
            }
        }
    }

    // 키보드 입력 확인
    private bool HasKeyboardInput()
    {
        return Input.GetAxis("Horizontal") != 0 ||
               Input.GetAxis("Vertical") != 0 ||
               Input.GetButtonDown("Jump");
    }

    // 클릭 마커 표시
    private void ShowClickMarker(Vector3 position)
    {
        // 기존 마커 제거
        if (_currentMarker != null)
        {
            Destroy(_currentMarker);
        }

        // 새 마커 생성
        _currentMarker = Instantiate(_clickMarkerPrefab, position, Quaternion.identity);

        // 일정 시간 후 자동 제거
        Destroy(_currentMarker, _markerLifetime);
    }

    // 외부에서 강제로 중단
    public void ForceStop()
    {
        StopNavMovement();
    }

    // NavMesh 이동 중인지 확인
    public bool IsNavMoving()
    {
        return _isMoving;
    }

    // 현재 목표 위치 가져오기
    public Vector3 GetTargetPosition()
    {
        return _targetPosition;
    }

    // 남은 거리 가져오기
    public float GetRemainingDistance()
    {
        if (_isMoving && _agent.enabled && !_agent.pathPending)
        {
            return _agent.remainingDistance;
        }
        return 0f;
    }

    // NavMeshAgent 속도 실시간 업데이트 (달리기 등)
    public void UpdateAgentSpeed(float newSpeed)
    {
        if (_agent.enabled)
        {
            _agent.speed = newSpeed;
        }
    }

    private void OnDrawGizmos()
    {
        if (_showDebugInfo && _isMoving && _agent != null && _agent.enabled)
        {
            // 경로 그리기
            if (_agent.path != null && _agent.path.corners.Length > 0)
            {
                Gizmos.color = Color.yellow;
                Vector3[] corners = _agent.path.corners;

                for (int i = 0; i < corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(corners[i], corners[i + 1]);
                }
            }

            // 목표 지점 그리기
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_targetPosition, 0.5f);
        }
    }
}