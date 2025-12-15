using UnityEngine;
using DG.Tweening;

public class FPSTPSCameraController : MonoBehaviour
{
    [Header("필수 설정")]
    [Tooltip("플레이어 Transform")]
    public Transform player;

    [Header("FPS 설정 (1인칭)")]
    [Tooltip("플레이어 눈 위치")]
    public Vector3 FpsOffset = new Vector3(0f, 1.6f, 0f);
    public float FpsFOV = 60f;

    [Header("TPS 설정 (3인칭)")]
    [Tooltip("플레이어 뒤쪽 위치")]
    public Vector3 TpsOffset = new Vector3(0f, 1.5f, -3f);
    public float TpsFOV = 70f;

    [Header("마우스 회전")]
    public float MouseSensitivity = 200f;
    [Range(-89f, 0f)]
    public float MinVerticalAngle = -80f;
    [Range(0f, 89f)]
    public float MaxVerticalAngle = 80f;

    [Header("전환 애니메이션")]
    [Tooltip("카메라 전환 시간")]
    public float TransitionDuration = 0.5f;
    [Tooltip("전환")]
    public Ease TransitionEase = Ease.OutCubic;

    [Header("총기 반동")]
    [Tooltip("위로 올라가는 반동")]
    [Range(0.5f, 5f)]
    public float GunVertical = 2.0f;

    [Tooltip("총 가로 반동")]
    [Range(0f, 2f)]
    public float GunHorizontal = 2f;

    [Tooltip("반동복구속도")]
    [Range(5f, 30f)]
    public float GunReturnSpeed = 12f;


    [Tooltip("반동복구강도")]
    [Range(0f, 1f)]
    public float GunSnapiness = 0.3f;

    [Tooltip("연사시 반동누적")]
    [Range(1f, 3f)]
    public float GunAccumulationRate = 1.5f;

    [Tooltip("최대 누적")]
    [Range(1f, 10f)]
    public float MaxGunAccumulation = 8f;


    // 내부 변수
    private Camera _cam;
    private bool _isFPS = true;
    private float _verticalRotation = 0f;
    private float _horizontalRotation = 0f;
    private Tween _currentTween;

    // 반동 변수
    private float _currentGunVertical = 0f;
    private float _currentGunHorizontal = 0f;
    private float _gunAccumulation = 0f;


    private void Awake()
    {
        // 카메라 찾기
        _cam = GetComponent<Camera>();
        if (_cam == null)
        {
            _cam = Camera.main;
        }

        // Player 자동 찾기 (부모 오브젝트)
        if (player == null && transform.parent != null)
        {
            player = transform.parent;
        }

        // 요소가 없으면 스크립트 비활성화
        if (player == null)
        {
            enabled = false;
            return;
        }

        if (_cam == null)
        {
            enabled = false;
            return;
        }
    }

    private void Start()
    {

        if (player == null || _cam == null) return;

        if (GameManager.Instance == null || GameManager.Instance.State != EGameState.Playing)
        {
            return;
        }

        // Transform 초기화
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // 초기 설정
        _cam.fieldOfView = FpsFOV;
        _horizontalRotation = player.eulerAngles.y; // 플레이어 방향 동기화

        // 커서 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (player == null || _cam == null) return;

        // T키로 카메라 전환
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleCameraMode();
        }

        // ESC로 커서 해제
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 좌클릭으로 커서 다시 잠금
        if (Input.GetMouseButtonDown(0) && Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // 마우스 회전 처리
        HandleMouseRotation();

        // 반동 처리
        HandleGunRebound();
    }

    void LateUpdate()
    {
        if (player == null || _cam == null) return;

        // 카메라 위치 업데이트 (트윈 중이 아닐 때만)
        if (_currentTween == null || !_currentTween.IsActive())
        {
            UpdateCameraPosition();
        }
    }

    private void HandleMouseRotation()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // 마우스 입력
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 수평 회전 (플레이어 Y축) -좌우
        _horizontalRotation += mouseX * MouseSensitivity * Time.deltaTime;

        // 수직 회전 (카메라 X축) -상하
        _verticalRotation -= mouseY * MouseSensitivity * Time.deltaTime;
        _verticalRotation = Mathf.Clamp(_verticalRotation, MinVerticalAngle, MaxVerticalAngle); // clamp: -80 ~ 80도 사이로 제한 (고개를 뒤로 꺽을 수 없게)

        // 플레이어 회전 적용 (좌우만)
        player.rotation = Quaternion.Euler(0f, _horizontalRotation, 0f);

        // 카메라 회전 적용 (상하 + 플레이어 방향 + 반동)
        float finalVertical = _verticalRotation - _currentGunVertical;
        float finalHorizontal = _currentGunHorizontal;

        _cam.transform.rotation = player.rotation * Quaternion.Euler(finalVertical, finalHorizontal, 0f);
    }

    private void HandleGunRebound()
    {
        float returnSpeed = GunReturnSpeed * (1f + GunSnapiness * 2f);

        _currentGunVertical = Mathf.Lerp(_currentGunVertical, 0f, Time.deltaTime * returnSpeed);
        _currentGunHorizontal = Mathf.Lerp(_currentGunHorizontal, 0f, Time.deltaTime * returnSpeed);

        // 거의 0에 가까우면 완전히 0으로 스냅
        if (Mathf.Abs(_currentGunVertical) < 0.01f)
        {
            _currentGunVertical = 0f;
        }
        if (Mathf.Abs(_currentGunHorizontal) < 0.01f)
            _currentGunHorizontal = 0f;

        // 누적 반동 감소 (시간이 지나면 자연스럽게 줄어듦)
        _gunAccumulation = Mathf.Lerp(_gunAccumulation, 0f, Time.deltaTime * 3f);
    }

    public void ApplyGunRebound()
    {
        // 누적 반동 증가 (연사 시 점점 더 심해짐)
        _gunAccumulation = Mathf.Min(_gunAccumulation + 1f, MaxGunAccumulation);
        float accumulationFactor = 1f + (_gunAccumulation * 0.15f * GunAccumulationRate);

        // 세로 반동 (위로 올라감) - 즉시 추가!
        float verticalUp = GunVertical * accumulationFactor;
        _currentGunVertical += verticalUp;

        // 가로 반동 (좌우 랜덤) - 즉시 추가!
        float horizontalUp = Random.Range(-GunHorizontal, GunHorizontal) * accumulationFactor;
        _currentGunHorizontal += horizontalUp;

        // 반동 제한 (너무 많이 올라가지 않게)
        _currentGunVertical = Mathf.Clamp(_currentGunVertical, 0f, 25f);
        _currentGunHorizontal = Mathf.Clamp(_currentGunHorizontal, -15f, 15f);
    }


    public void ResetGunRebound()
    {
        _currentGunVertical = 0f;
 /*       _currentGunHorizontal = 0f;*/
        _gunAccumulation = 0f;
    }

    private void UpdateCameraPosition()
    {
        Vector3 targetPosition;

        if (_isFPS)
        {
            // FPS: 플레이어 위치 + 눈 높이 오프셋
            targetPosition = player.position + player.TransformDirection(FpsOffset);
        }
        else
        {
            // TPS: 플레이어 위치 + 뒤쪽 오프셋
            targetPosition = player.position + player.TransformDirection(TpsOffset);
        }

        _cam.transform.position = targetPosition;
    }

    private void ToggleCameraMode()
    {
        // 이전 트윈이 실행 중이면 중단
        if (_currentTween != null && _currentTween.IsActive())
        {
            _currentTween.Kill();
        }

        // 모드 전환
        _isFPS = !_isFPS;

        // 목표 위치와 FOV 계산
        Vector3 targetPosition;
        float targetFOV;

        if (_isFPS)
        {
            targetPosition = player.position + player.TransformDirection(FpsOffset);
            targetFOV = FpsFOV;
            Debug.Log("FPS 1인칭");
        }
        else
        {
            targetPosition = player.position + player.TransformDirection(TpsOffset);
            targetFOV = TpsFOV;
            Debug.Log("TPS 3인칭");
        }

        // DOTween 시퀀스로 부드러운 전환
        Sequence sequence = DOTween.Sequence();

        // 카메라 위치 전환
        sequence.Join(_cam.transform.DOMove(targetPosition, TransitionDuration).SetEase(TransitionEase));

        // FOV 전환
        sequence.Join(_cam.DOFieldOfView(targetFOV, TransitionDuration).SetEase(TransitionEase));

        _currentTween = sequence; // 새 애니메이션으로 교체, 저장해서 다음 전환 시 중단 가능하게
    }
}