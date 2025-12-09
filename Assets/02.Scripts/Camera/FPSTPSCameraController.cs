using UnityEngine;
using DG.Tweening;

/// <summary>
/// FPS/TPS 카메라 전환 시스템
/// T키로 1인칭/3인칭 카메라를 DOTween으로 부드럽게 전환합니다
/// </summary>
public class FPSTPSCameraController : MonoBehaviour
{
    [Header("=== 필수 설정 ===")]
    [Tooltip("플레이어 Transform (자동으로 부모를 찾습니다)")]
    public Transform player;

    [Header("=== FPS 설정 (1인칭) ===")]
    [Tooltip("플레이어 눈 위치")]
    public Vector3 fpsOffset = new Vector3(0f, 1.6f, 0f);
    public float fpsFOV = 60f;

    [Header("=== TPS 설정 (3인칭) ===")]
    [Tooltip("플레이어 뒤쪽 위치")]
    public Vector3 tpsOffset = new Vector3(0f, 1.5f, -3f);
    public float tpsFOV = 70f;

    [Header("=== 마우스 회전 ===")]
    public float mouseSensitivity = 200f;
    [Range(-89f, 0f)]
    public float minVerticalAngle = -80f;
    [Range(0f, 89f)]
    public float maxVerticalAngle = 80f;

    [Header("=== 전환 애니메이션 ===")]
    [Tooltip("카메라 전환 시간")]
    public float transitionDuration = 0.5f;
    [Tooltip("전환 이징 (부드러운 정도)")]
    public Ease transitionEase = Ease.OutCubic;

    // 내부 변수
    private Camera cam;
    private bool isFPS = true;
    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;
    private Tween currentTween;

    void Awake()
    {
        // 카메라 찾기
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        // Player 자동 찾기 (부모 오브젝트)
        if (player == null && transform.parent != null)
        {
            player = transform.parent;
            Debug.Log($"[FPS/TPS Camera] Player 자동 할당: {player.name}");
        }

        // 검증
        if (player == null)
        {
            Debug.LogError("[FPS/TPS Camera] ❌ Player가 없습니다! Inspector에서 할당하세요!");
            enabled = false;
            return;
        }

        if (cam == null)
        {
            Debug.LogError("[FPS/TPS Camera] ❌ Camera를 찾을 수 없습니다!");
            enabled = false;
            return;
        }
    }

    void Start()
    {
        if (player == null || cam == null) return;

        // Transform 초기화
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // 초기 설정
        cam.fieldOfView = fpsFOV;
        horizontalRotation = player.eulerAngles.y;

        // 커서 잠금
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("✅ [FPS/TPS Camera] 초기화 완료! T키로 카메라 전환하세요.");
    }

    void Update()
    {
        if (player == null || cam == null) return;

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
    }

    void LateUpdate()
    {
        if (player == null || cam == null) return;

        // 카메라 위치 업데이트 (트윈 중이 아닐 때만)
        if (currentTween == null || !currentTween.IsActive())
        {
            UpdateCameraPosition();
        }
    }

    /// <summary>
    /// 마우스 입력으로 카메라와 플레이어 회전
    /// </summary>
    private void HandleMouseRotation()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // 마우스 입력
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // 수평 회전 (플레이어 Y축)
        horizontalRotation += mouseX * mouseSensitivity * Time.deltaTime;

        // 수직 회전 (카메라 X축)
        verticalRotation -= mouseY * mouseSensitivity * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        // 플레이어 회전 적용
        player.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        // 카메라 회전 적용
        cam.transform.rotation = player.rotation * Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    /// <summary>
    /// 현재 모드에 맞게 카메라 위치 업데이트
    /// </summary>
    private void UpdateCameraPosition()
    {
        Vector3 targetPosition;

        if (isFPS)
        {
            // FPS: 플레이어 위치 + 눈 높이 오프셋
            targetPosition = player.position + player.TransformDirection(fpsOffset);
        }
        else
        {
            // TPS: 플레이어 위치 + 뒤쪽 오프셋
            targetPosition = player.position + player.TransformDirection(tpsOffset);
        }

        cam.transform.position = targetPosition;
    }

    /// <summary>
    /// FPS ↔ TPS 카메라 모드 전환 (DOTween 사용)
    /// </summary>
    private void ToggleCameraMode()
    {
        // 이전 트윈이 실행 중이면 중단
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        // 모드 전환
        isFPS = !isFPS;

        // 목표 위치와 FOV 계산
        Vector3 targetPosition;
        float targetFOV;

        if (isFPS)
        {
            targetPosition = player.position + player.TransformDirection(fpsOffset);
            targetFOV = fpsFOV;
            Debug.Log("📹 FPS 모드 (1인칭)");
        }
        else
        {
            targetPosition = player.position + player.TransformDirection(tpsOffset);
            targetFOV = tpsFOV;
            Debug.Log("🎯 TPS 모드 (3인칭)");
        }

        // DOTween 시퀀스로 부드러운 전환
        Sequence sequence = DOTween.Sequence();

        // 카메라 위치 전환
        sequence.Join(
            cam.transform.DOMove(targetPosition, transitionDuration)
                .SetEase(transitionEase)
        );

        // FOV 전환
        sequence.Join(
            cam.DOFieldOfView(targetFOV, transitionDuration)
                .SetEase(transitionEase)
        );

        currentTween = sequence;
    }

    /// <summary>
    /// 현재 카메라 모드 확인 (외부 접근용)
    /// </summary>
    public bool IsFPSMode => isFPS;

    // 디버그: 화면에 현재 상태 표시
    void OnGUI()
    {
        if (player == null) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 16;
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;

        GUI.Label(new Rect(10, 10, 400, 25),
            $"카메라 모드: {(isFPS ? "FPS (1인칭)" : "TPS (3인칭)")}", style);
        GUI.Label(new Rect(10, 35, 400, 25),
            $"FOV: {cam.fieldOfView:F0}", style);
        GUI.Label(new Rect(10, 60, 400, 25),
            "T키: 카메라 전환", style);
    }
}