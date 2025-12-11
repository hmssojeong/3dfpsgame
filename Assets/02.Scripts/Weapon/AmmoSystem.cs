using UnityEngine;
// 기능: 탄약관리
// - 현재 탄창 탄약 (current)
// - 총 예비 탄약 (reserve)
// - 재장전 로직
// - 이벤트 (탄약 변경, 재장전 진행)
public class AmmoSystem : MonoBehaviour
{
    [Header("탄약 설정")]
    [SerializeField] private int _maxAmmo = 30;
    [SerializeField] private int _maxReserve = 120;
    [SerializeField] private float _reloadTime = 1.6f;

    private int _currentAmmo;
    private int _reserveAmmo;
    private float _reloadTimer = 0f;
    private bool _isReloading = false;

    public static System.Action<int, int> OnAmmoChanged;

    private void Start()
    {
        _currentAmmo = _maxAmmo;
        _reserveAmmo = _maxReserve;
        UpdateUI();
    }

    private void Update()
    {
        HandleReload();

        if(Input.GetKeyDown(KeyCode.R))
        {
            StartReload();
        }
    }

    public bool TryConsume()
    {
        if (_isReloading)
        {
            return false;
        }

        if (_currentAmmo <=0)
        {
            return false;
        }

        _currentAmmo--;
        UpdateUI();

         if (_currentAmmo <= 0 && _reserveAmmo > 0)
         {
             StartReload();
          }

        return true;
    }


    private void StartReload()
    {

        if (_currentAmmo >= _maxAmmo)
        {
            return;
        }


        if (_reserveAmmo <= 0)
        {
            return;
        }

        // 이미 재장전 중이면 무시
        if (_isReloading)
        {
            return;
        }

        _isReloading = true;
        _reloadTimer = _reloadTime;

    }


    private void HandleReload()
    {
        if (!_isReloading) return;

        _reloadTimer -= Time.deltaTime;

        if (_reloadTimer <= 0f)
        {
            // 재장전 완료
            int needed = _maxAmmo - _currentAmmo;     
            int toReload = Mathf.Min(needed, _reserveAmmo); 

            _currentAmmo += toReload;  
            _reserveAmmo -= toReload;   

            _isReloading = false;
            _reloadTimer = 0f;

            UpdateUI();

        }
    }


    private void UpdateUI()
    {
        OnAmmoChanged?.Invoke(_currentAmmo, _reserveAmmo);
    }

    public int CurrentAmmo => _currentAmmo;

 
    public int ReserveAmmo => _reserveAmmo;


    public float ReloadProgress
    {
        get
        {
            if (!_isReloading) return 1f;
            return 1f - (_reloadTimer / _reloadTime);
        }
    }

    public bool IsReloading => _isReloading;
}

