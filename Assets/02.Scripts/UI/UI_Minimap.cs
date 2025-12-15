using UnityEngine;
using UnityEngine.UI;

public class UI_Minimap : MonoBehaviour
{
    [SerializeField] private Camera _minimapCamera;

    [SerializeField] private float _defaultZoom = 10f;

    [SerializeField] private float _minZoom = 5f;
    [SerializeField] private float _maxZoom = 30f;

    [SerializeField] private float _zoomStep = 2f;
    [SerializeField] private float _zoomSpeed = 5f;

    [Header("UI 버튼")]
    [SerializeField] private Button _zoomInButton;
    [SerializeField] private Button _zoomOutButton;

    // 내부 변수
    private float _currentZoom;
    private float _targetZoom;

    private void Start()
    {
        if (_minimapCamera == null)
        {
            _minimapCamera = GetComponent<Camera>();
        }

        if (_minimapCamera == null)
        {
            enabled = false;
            return;
        }

        _currentZoom = _defaultZoom;
        _targetZoom = _defaultZoom;
        _minimapCamera.orthographicSize = _currentZoom;

        if (_zoomInButton != null)
        {
            _zoomInButton.onClick.AddListener(ZoomIn);
        }

        if (_zoomOutButton != null)
        {
            _zoomOutButton.onClick.AddListener(ZoomOut);
        }
    }

    private void Update()
    {
        // 부드러운 줌 전환
        if (_zoomSpeed > 0)
        {
            _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, Time.deltaTime * _zoomSpeed);
        }
        else
        {
            _currentZoom = _targetZoom;
        }

        _minimapCamera.orthographicSize = _currentZoom;
    }

    public void ZoomIn()
    {
        _targetZoom -= _zoomStep;
        _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
    }

    public void ZoomOut()
    {
        _targetZoom += _zoomStep;
        _targetZoom = Mathf.Clamp(_targetZoom, _minZoom, _maxZoom);
    }

    public void SetZoom(float zoom)
    {
        _targetZoom = Mathf.Clamp(zoom, _minZoom, _maxZoom);
    }

    public void ResetZoom()
    {
        _targetZoom = _defaultZoom;
    }

    private void OnDestroy()
    {
        if (_zoomInButton != null)
        {
            _zoomInButton.onClick.RemoveListener(ZoomIn);
        }

        if (_zoomOutButton != null)
        {
            _zoomOutButton.onClick.RemoveListener(ZoomOut);
        }
    }
}