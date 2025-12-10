using UnityEngine;
// 기능: 탄약관리
// - 현재 탄창 탄약 (current)
// - 총 예비 탄약 (reserve)
// - 재장전 로직
// - 이벤트 (탄약 변경, 재장전 진행)
public class AmmoSystem : MonoBehaviour
{
    [Header("탄약 설정")]
    [SerializeField] private float _maxAmmo = 30f;
    [SerializeField] private float _maxReserve = 120f;
    [SerializeField] private float _reloadTime = 1.6f;

    private float _currentAmmo;
    private float _currentReserve;
    private bool _isReloading = false;

}
