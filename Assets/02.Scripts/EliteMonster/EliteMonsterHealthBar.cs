using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EliteMonster))]
public class EliteMonsterHealthBar : MonoBehaviour
{
    private EliteMonster _eliteMonster;
    [SerializeField] private Transform _healthBarTransform;
    [SerializeField] private Image _gaugeImageDelay;
    [SerializeField] private Image _gaugeImage;
    [SerializeField] private Image _gaugeImageDelayLate;
    [SerializeField] private Image _healthBarFillImage;

    [Header("효과")]
    [SerializeField] private float _flashDuration = 0.1f;
    [SerializeField] private float _shakeDuration = 0.2f;
    [SerializeField] private float _shakeAmount = 10f;
    [SerializeField] private float _shortDelay = 0.2f;
    [SerializeField] private float _longDelay = 0.5f;

    private Vector3 _originalPosition;
    private Camera _mainCamera;
    private float _lastHealth = -1;

    private void Awake()
    {
        _eliteMonster = gameObject.GetComponent<EliteMonster>();
        _mainCamera = Camera.main;

        if (_healthBarTransform != null)
        {
            _originalPosition = _healthBarTransform.localPosition;
        }
    }

    private void LateUpdate()
    {
        // 체력 변화 감지
        if (_lastHealth != _eliteMonster.Health.Value)
        {
            _lastHealth = _eliteMonster.Health.Value;
            _gaugeImage.fillAmount = GetHealthPercentage();

            StartCoroutine(HitDelayGauge_Coroutine(_gaugeImageDelay, _shortDelay));
            StartCoroutine(HitDelayGauge_Coroutine(_gaugeImageDelayLate, _longDelay));
            StartCoroutine(ShakeHealthBar());
            StartCoroutine(WhiteFlash(_gaugeImage, _gaugeImage.color));
            StartCoroutine(WhiteFlash(_gaugeImageDelay, _gaugeImageDelay.color));
            StartCoroutine(WhiteFlash(_gaugeImageDelayLate, _gaugeImageDelayLate.color));
        }

        // 빌보드
        if (_mainCamera != null && _healthBarTransform != null)
        {
            _healthBarTransform.forward = _mainCamera.transform.forward;
        }
    }

    private float GetHealthPercentage()
    {
        if (_eliteMonster == null || _eliteMonster.Health.MaxValue == 0)
        {
            return 0f;
        }
        return _eliteMonster.Health.Value / _eliteMonster.Health.MaxValue;
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

        float shakeTime = 0f;

        while (shakeTime < _shakeDuration)
        {
            float offsetX = Random.Range(-_shakeAmount, _shakeAmount);
            float offsetY = Random.Range(-_shakeAmount, _shakeAmount);

            _healthBarTransform.localPosition = _originalPosition + new Vector3(offsetX, offsetY, 0);

            shakeTime += Time.deltaTime;
            yield return null;
        }

        _healthBarTransform.localPosition = _originalPosition;
    }

    private IEnumerator WhiteFlash(Image gauge, Color originalColor)
    {
        if (gauge == null) yield break;

        gauge.color = Color.white;
        yield return new WaitForSeconds(_flashDuration);
        gauge.color = originalColor;
    }
}