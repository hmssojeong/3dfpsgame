using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Monster))]
public class MonsterHealthBar : MonoBehaviour
{
    private Monster _monster;
    [SerializeField] private Transform _healthBarTransform;
    [SerializeField] private Image _guageImageDelay;
    [SerializeField] private Image _guageImage;
    [SerializeField] private Image _guageImageDelayLate;

    [SerializeField] private float _shakeDuration = 0.2f;
    [SerializeField] private float _shakeAmount = 10f;

    private Vector3 _originalPosition;

    private float _lastHealth = -1;

    private void Awake()
    {
        _monster = gameObject.GetComponent<Monster>();

        if (_healthBarTransform != null)
        {
            _originalPosition = _healthBarTransform.localPosition;
        }
    }

    private void LateUpdate()
    {
        // 0 ~ 1
        // UI가 알고있는 몬스터 체력값과 다를 경우에만 fillAmount를 수정한다.
        if(_lastHealth != _monster.Health.Value)
        {
            _lastHealth = _monster.Health.Value;
            _guageImage.fillAmount = _monster.Health.Value / _monster.Health.MaxValue;
            StartCoroutine(HitDelayGuage_Coroutine());
            StartCoroutine(HitDelayLateGuage_Coroutine());
            StartCoroutine(ShakeHealthBar());
        }

        // 빌보드 기법: 카메라의 위치와 회전에 상관없이 항상 정면을 바라보게하는 기법
        _healthBarTransform.forward = Camera.main.transform.forward;

    }

    private IEnumerator HitDelayGuage_Coroutine()
    {
        yield return new WaitForSeconds(0.2f);
        _guageImageDelay.fillAmount = _monster.Health.Value / _monster.Health.MaxValue;
    }

    private IEnumerator HitDelayLateGuage_Coroutine()
    {
        yield return new WaitForSeconds(0.5f);
        _guageImageDelayLate.fillAmount = _monster.Health.Value / _monster.Health.MaxValue;
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
}
