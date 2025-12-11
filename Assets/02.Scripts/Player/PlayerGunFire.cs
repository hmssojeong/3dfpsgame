using UnityEngine;

public class PlayerGunFire : MonoBehaviour
{
    // 목표: 마우스의 왼쪽 버튼을 누르면 바라보는 방향으로 총을 발사하고 싶다. (총알을 날리고 싶다.)
    [SerializeField] private Transform _fireTransform; // 총알이 발사될 위치
    [SerializeField] private ParticleSystem _hitEffect; // 피격 이펙트 프리팹

    [SerializeField] private float _fireRate = 0.2f;

    [Header("탄약 시스템")]
    [SerializeField] private AmmoSystem _ammo;

    [Header("반동")]
    [SerializeField] private FPSTPSCameraController _cameraController;

    private float _fireTimer = 0f; // 남은 쿨타임

    private void Start()
    {
        if(_cameraController == null)
        {
            _cameraController = FindAnyObjectByType<FPSTPSCameraController>();
        }
    }

    private void Update()
    {

        if (_fireTimer > 0f)
        {
            _fireTimer -= Time.deltaTime;

        }

        // 1. 마우스 왼쪽 버튼이 눌린다면..
        if (Input.GetMouseButton(0) && _fireTimer <= 0f)
        {
            if (_ammo != null && _ammo.TryConsume())
            {
                Fire();                    
                _fireTimer = _fireRate;
            }
        }
    }

        private void Fire()
        {
            // 반동
            if (_cameraController != null)
            {
                 _cameraController.ApplyGunRebound();
            }

            // 2. Ray를 생성하고 발사할 위치, 방향, 거리를 설정한다. (쏜다.)
            /*Ray ray = new Ray(_fireTransform.position, Camera.main.transform.forward);*/
             Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            // 3. RayCastHit(충돌한 대상의 정보)를 저장할 변수를 생성한다.
            RaycastHit hitInfo = new RaycastHit();

            // 4. 발사하고
            bool isHit = Physics.Raycast(ray, out hitInfo);
            if (isHit)
            {
                // 5. 충돌했다면... 피격 이펙트 표시
                Debug.Log(hitInfo.transform.name);

                // 파티클 생성과 플레이 방식
                // 1. Instantiate 방식 (+ 풀링) -> 한 화면에 여러가지 수정 후 여러개 그릴경우
                // 2. 하나를 캐싱해두고 Play    -> 인스펙터 설정 그대로 그릴 경우 // 한 화면에 한번만 그릴 경우
                // 3. 하나를 캐싱해두고 Emit    -> 한 화면에 위치만 수정 후 여러개 그릴 경우 ->인스펙터 속성을 바꾸고싶다면 Emit쓰기 ex) END색깔을 바꾼다던지 할 때

                //_hitEffect.transform.position = hitInfo.point;
                //_hitEffect.transform.forward = hitInfo.normal;

                _hitEffect.transform.position = hitInfo.point;
                _hitEffect.transform.forward = hitInfo.normal;

                _hitEffect.Play();

            }
        }

        // Ray: 레이저(시작위치, 방향, 거리)
        // RayCast: 레이저를 발사
        // RayCastHit: 레이저가 물체와 충돌했다면 그 정보를 저장하는 구조체

 }
