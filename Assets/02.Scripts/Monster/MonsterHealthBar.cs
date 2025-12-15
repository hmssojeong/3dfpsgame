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

    private float _lastHealth = -1;

    private void Awake()
    {
        _monster = gameObject.GetComponent<Monster>();
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
}
