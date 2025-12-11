using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 기능: 총알 UI 표시
// -"30/120" XPRTMXM
// - 재장전 게이지
// - 색상 변경 (탄약 부족 시)

public class GunAmmoUI : MonoBehaviour
{
    [Header("UI 요소들")]
    [SerializeField] private TextMeshProUGUI _ammoText;    
    [SerializeField] private Image _reloadBar;            
    [SerializeField] private GameObject _reloadPanel;

    [Header("스타일 설정")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _emptyColor = Color.red;
    [SerializeField] private Color _reloadingColor = Color.yellow;

    private AmmoSystem _ammoSystem;

    private void Start()
    {
   
        _ammoSystem = FindAnyObjectByType<AmmoSystem>();

        if (_ammoSystem == null)
        {
            enabled = false;
            return;
        }

        AmmoSystem.OnAmmoChanged += UpdateAmmoDisplay;

        UpdateAmmoDisplay(_ammoSystem.CurrentAmmo, _ammoSystem.ReserveAmmo);


        if (_reloadPanel != null)
        {
            _reloadPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        AmmoSystem.OnAmmoChanged -= UpdateAmmoDisplay;
    }

    private void Update()
    {
        if (_ammoSystem != null && _ammoSystem.IsReloading)
        {
            UpdateReloadProgress();
        }
    }

    private void UpdateAmmoDisplay(int current, int reserve)
    {
        if (_ammoText != null)
        {
            _ammoText.text = $"{current}/{reserve}";

            if (current <= 0)
            {
                _ammoText.color = _emptyColor; 
            }
            else if (_ammoSystem != null && _ammoSystem.IsReloading)
            {
                _ammoText.color = _reloadingColor;
            }
            else
            {
                _ammoText.color = _normalColor;
            }
        }
    }


    private void UpdateReloadProgress()
    {
        if (_reloadPanel != null)
        {
            _reloadPanel.SetActive(true);
        }

        if (_reloadBar != null)
        {
            _reloadBar.fillAmount = _ammoSystem.ReloadProgress;
        }

        if (!_ammoSystem.IsReloading && _reloadPanel != null)
        {
            _reloadPanel.SetActive(false);
        }
    }
}