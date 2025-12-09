using UnityEngine;

[Serializeable]
public class CosumeableStat
{
    [SerializeField] private float _maxValue;

    [SerializeField] private float _value;
    [SerializeField] private float _regenValue;

    public void Regenerate(float time)
    {
        _value += _regenValue * time;
    }

    public void IncreaseMax(float amount)
    {
        _maxValue += amount;
    }
    public void Increase(float amount)
    {
        _value += amount;
    }

    public void DecreaseMax(float amount)
    {
        _maxValue += amount;
    }
    public void Decrease(float amount)
    {
        _value -= amount;
    }

    public void SetValueMax(float amount)
    {
        _maxValue += amount;
    }
    public void SetValue(float value)
    {
        _value = value;
    }
}
