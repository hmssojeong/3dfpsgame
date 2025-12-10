using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 폭탄 개수를 화면에 표시하는 UI 시스템
/// 왼쪽 하단에 폭탄 아이콘과 개수를 표시합니다
/// </summary>
public class BombAmmoUI : MonoBehaviour
{
    [Header("UI 요소들")]
    [SerializeField] private Text _ammoText;           // "5/5" 같은 텍스트
    [SerializeField] private Image _reloadBar;         // 재장전 바 (선택사항)
    [SerializeField] private GameObject _reloadPanel;  // 재장전 중 표시 (선택사항)

    [Header("스타일 설정")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _emptyColor = Color.red;
    [SerializeField] private Color _reloadingColor = Color.yellow;

    private PlayerFire _playerFire;

    private void Start()
    {
        // PlayerFire 찾기
        _playerFire = FindObjectOfType<PlayerFire>();

        if (_playerFire == null)
        {
            Debug.LogError("PlayerFire를 찾을 수 없습니다!");
            enabled = false;
            return;
        }

        // 이벤트 구독
        PlayerFire.OnBombCountChanged += UpdateAmmoDisplay;

        // 초기 표시
        UpdateAmmoDisplay(_playerFire.CurrentBombCount, _playerFire.MaxBombCount);

        // 재장전 패널 초기 상태
        if (_reloadPanel != null)
        {
            _reloadPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        PlayerFire.OnBombCountChanged -= UpdateAmmoDisplay;
    }

    private void Update()
    {
        // 재장전 진행도 표시
        if (_playerFire != null && _playerFire.IsReloading)
        {
            UpdateReloadProgress();
        }
    }

    /// <summary>
    /// 탄약 개수 표시 업데이트
    /// </summary>
    private void UpdateAmmoDisplay(int current, int max)
    {
        if (_ammoText != null)
        {
            _ammoText.text = $"{current}/{max}";

            // 색상 변경
            if (current <= 0)
            {
                _ammoText.color = _emptyColor;
            }
            else if (_playerFire != null && _playerFire.IsReloading)
            {
                _ammoText.color = _reloadingColor;
            }
            else
            {
                _ammoText.color = _normalColor;
            }
        }
    }

    /// <summary>
    /// 재장전 진행도 업데이트
    /// </summary>
    private void UpdateReloadProgress()
    {
        if (_reloadPanel != null)
        {
            _reloadPanel.SetActive(true);
        }

        if (_reloadBar != null)
        {
            _reloadBar.fillAmount = _playerFire.ReloadProgress;
        }

        // 재장전 완료 시 패널 숨기기
        if (!_playerFire.IsReloading && _reloadPanel != null)
        {
            _reloadPanel.SetActive(false);
        }
    }
}