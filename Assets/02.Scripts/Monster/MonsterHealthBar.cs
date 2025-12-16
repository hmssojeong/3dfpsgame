using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Monster))]
public class MonsterHealthBar : MonoBehaviour
{
    private Monster _monster;
    [SerializeField] private Transform _healthBarTransform;
    [SerializeField] private Image _gaugeImageDelay;
    [SerializeField] private Image _gaugeImage;
    [SerializeField] private Image _gaugeImageDelayLate;
    [SerializeField] private Image _healthBarFillImage;

    [Header("하얀색 효과")]
    [SerializeField] private float _flashDuration = 0.1f;

    [SerializeField] private float _shakeDuration = 0.2f;
    [SerializeField] private float _shakeAmount = 10f;

    [SerializeField] private float _shortDelay = 0.2f;
    [SerializeField] private float _longDelay = 0.5f;

    private Color _originalColor;
    private Vector3 _originalPosition;
    private Camera _mainCamera;

    private float _lastHealth = -1;

    private void Awake()
    {
        _monster = gameObject.GetComponent<Monster>();
        _mainCamera = Camera.main; // 카메라 캐싱.

        // 체력바의 원래 색상 저장
        if (_healthBarFillImage != null)
        {
            _originalColor = _healthBarFillImage.color;
        }

        if (_healthBarTransform != null)
        {
            _originalPosition = _healthBarTransform.localPosition;
        }
    }

    private void LateUpdate()
    {
        if(_lastHealth != _monster.Health.Value)
        {
            _lastHealth = _monster.Health.Value;
            _gaugeImage.fillAmount = GetHealthPercentage();
            StartCoroutine(HitDelayGauge_Coroutine(_gaugeImageDelay, _shortDelay));
            StartCoroutine(HitDelayGauge_Coroutine(_gaugeImageDelayLate, _longDelay));
            StartCoroutine(ShakeHealthBar());
            StartCoroutine(WhiteFlash());
        }

        // 빌보드 기법: 카메라의 위치와 회전에 상관없이 항상 정면을 바라보게하는 기법
        if (_mainCamera != null && _healthBarTransform != null)
        {
            _healthBarTransform.forward = _mainCamera.transform.forward;
        }

    }

    private float GetHealthPercentage()
    {
        if (_monster == null || _monster.Health.MaxValue == 0)
        {
            return 0f;
        }
        return _monster.Health.Value / _monster.Health.MaxValue;
    }

    private IEnumerator HitDelayGauge_Coroutine(Image gaugeImage, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gaugeImage != null)
        {
            gaugeImage.fillAmount = GetHealthPercentage();
        }
    }

    private IEnumerator ShakeHealthBar()
    {
        if (_healthBarTransform == null) yield break;

        float _shakeTime = 0f;

        while (_shakeTime < _shakeDuration)
        {
            float offsetX = Random.Range(-_shakeAmount, _shakeAmount);
            float offsetY = Random.Range(-_shakeAmount, _shakeAmount);

            _healthBarTransform.localPosition = _originalPosition + new Vector3(offsetX, offsetY, 0);

            _shakeTime += Time.deltaTime;
            yield return null;
        }
        _healthBarTransform.localPosition = _originalPosition;
    }

    private IEnumerator WhiteFlash()
    {
        if (_healthBarFillImage == null) yield break;

        _healthBarFillImage.color = Color.white;

        yield return new WaitForSeconds(_flashDuration);
        _healthBarFillImage.fillAmount = GetHealthPercentage();

        _healthBarFillImage.color = _originalColor;
    }
}
