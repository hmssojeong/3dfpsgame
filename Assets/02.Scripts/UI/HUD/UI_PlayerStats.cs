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

        UpdateHealthUI();
        UpdateStaminaUI();
    }

    private void Update()
    {
        UpdateStaminaUI();
    }

    private void UpdateHealthUI()
    {
        if (_stats != null && _healthSlider != null)
        {
            _healthSlider.value = _stats.GetHealthPercentage();
        }
    }

    private void UpdateStaminaUI()
    {
        if (_stats != null && _staminaSlider != null)
        {
            _staminaSlider.value = _stats.GetStaminaPercentage();
        }
    }
    private void Flash()
    {
        UpdateHealthUI();

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

        float fadePerSecond = _startAlpha / _flashSpeed;

        while (alpha > 0.0f)
        {
            alpha -= fadePerSecond * Time.deltaTime;
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
        if (_stats != null)
        {
            _stats.OnDamaged += Flash;
        }
    }

    private void OnDisable()
    {
        if (_stats != null)
        {
            _stats.OnDamaged -= Flash;
        }
    }
}
