using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _staminaSlider;

    [SerializeField] private Image _image;
    [SerializeField] private float _flashSpeed = 2f;
    [SerializeField] private float _startAlpha = 0.8f;


    private Coroutine _coroutine;

    private void Start()
    {
        if (_image != null)
        {
            _image.enabled = false;
        }
    }

    private void Update()
    {
        _healthSlider.value = _stats.Health.Value / _stats.Health.MaxValue;
        _staminaSlider.value = _stats.Stamina.Value / _stats.Stamina.MaxValue;
    }

    private void Flash()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _image.enabled = true;
        _coroutine = StartCoroutine(FadeAway());
    }

    private IEnumerator FadeAway()
    {
        float alpha = _startAlpha;
        Color color = _image.color;

        while (alpha > 0.0f)
        {
            alpha -= (_startAlpha / _flashSpeed) * Time.deltaTime;
            color.a = alpha;
            _image.color = color;
            yield return null;
        }

        color.a = 0f;
        _image.color = color;
        _image.enabled = false;

    }

    private void OnEnable()
    {
        _stats.OnDamaged += Flash;
    }

    private void OnDisable()
    {
        _stats.OnDamaged -= Flash;
    }
}
