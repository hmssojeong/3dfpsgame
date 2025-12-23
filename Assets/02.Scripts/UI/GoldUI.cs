using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private PlayerGold _playerGold;

    [Header("애니메이션 설정")]
    [SerializeField] private bool _useAnimation = true;
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 1.2f);

    private int _displayedGold = 0;
    private int _targetGold = 0;
    private float _animationTimer = 0f;
    private Vector3 _originalScale;

    private void Start()
    {
        // PlayerGold 찾기
        if (_playerGold == null)
        {
            _playerGold = FindAnyObjectByType<PlayerGold>();
        }

        if (_playerGold == null)
        {
            Debug.LogError("[골드 UI] PlayerGold를 찾을 수 없습니다!");
            return;
        }

        // 이벤트 구독
        _playerGold.OnGoldChanged.AddListener(OnGoldChanged);

        // 초기 골드 표시
        _displayedGold = _playerGold.CurrentGold;
        _targetGold = _displayedGold;
        UpdateGoldText();

        if (_goldText != null)
        {
            _originalScale = _goldText.transform.localScale;
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (_playerGold != null)
        {
            _playerGold.OnGoldChanged.RemoveListener(OnGoldChanged);
        }
    }

    private void OnGoldChanged(int newGold)
    {
        _targetGold = newGold;

        if (_useAnimation)
        {
            _animationTimer = 0f;
        }
        else
        {
            _displayedGold = newGold;
            UpdateGoldText();
        }
    }

    private void Update()
    {
        if (!_useAnimation || _displayedGold == _targetGold)
            return;

        _animationTimer += Time.deltaTime;
        float t = Mathf.Clamp01(_animationTimer / _animationDuration);

        // 숫자 증가 애니메이션
        _displayedGold = Mathf.RoundToInt(Mathf.Lerp(_displayedGold, _targetGold, t));
        UpdateGoldText();

        // 스케일 애니메이션
        if (_goldText != null && _originalScale != Vector3.zero)
        {
            float scale = _scaleCurve.Evaluate(t);
            _goldText.transform.localScale = _originalScale * scale;
        }

        // 애니메이션 완료
        if (t >= 1f)
        {
            _displayedGold = _targetGold;
            UpdateGoldText();

            if (_goldText != null)
            {
                _goldText.transform.localScale = _originalScale;
            }
        }
    }

    private void UpdateGoldText()
    {
        if (_goldText != null)
        {
            _goldText.text = $"{_displayedGold:N0}"; // 천 단위 콤마 포함
        }
    }
}