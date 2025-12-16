using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerStats))]
public class ClickToMove : MonoBehaviour
{
    private NavMeshAgent _agent;
    private PlayerStats _stats;
    private PlayerMove _playerMove; // 키보드 이동 스크립트 참조

    [SerializeField] private LayerMask _groundLayer; // 클릭 가능한 바닥 레이어
    [SerializeField] private float _stoppingDistance = 0.5f;

    private bool _isMoving = false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _stats = GetComponent<PlayerStats>();
        _playerMove = GetComponent<PlayerMove>();

        // 처음엔 NavMeshAgent 비활성화
        _agent.enabled = false;
    }

    private void Update()
    {
        // 마우스 우클릭 감지
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundLayer))
            {
                StartNavMovement(hit.point);
            }
        }

        // 키보드 입력이 있으면 NavMesh 이동 중단
        if (_isMoving && HasKeyboardInput())
        {
            StopNavMovement();
        }

        // 목적지 도착 체크
        if (_isMoving && !_agent.pathPending && _agent.remainingDistance <= _stoppingDistance)
        {
            StopNavMovement();
        }
    }

    // NavMesh 이동 시작
    private void StartNavMovement(Vector3 destination)
    {
        // PlayerMove 비활성화
        if (_playerMove != null)
        {
            _playerMove.enabled = false;
        }

        // CharacterController 비활성화
        CharacterController controller = GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        // NavMeshAgent 활성화
        _agent.enabled = true;
        _agent.speed = _stats.MoveSpeed.Value;

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
        if (_isMoving)
        {
            _isMoving = false;

            // NavMeshAgent 중단 및 비활성화
            if (_agent.enabled)
            {
                _agent.ResetPath();
                _agent.enabled = false;
            }

            // CharacterController 활성화
            CharacterController controller = GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = true;
            }

            // PlayerMove 활성화
            if (_playerMove != null)
            {
                _playerMove.enabled = true;
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

    // 외부에서 강제로 중단할 수 있도록
    public void ForceStop()
    {
        StopNavMovement();
    }

    // NavMesh 이동 중인지 확인
    public bool IsNavMoving()
    {
        return _isMoving;
    }
}