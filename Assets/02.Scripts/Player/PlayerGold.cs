using UnityEngine;
using UnityEngine.Events;

public class PlayerGold : MonoBehaviour
{
    [Header("골드")]
    [SerializeField] private int _currentGold = 0;

    [Header("이벤트")]
    public UnityEvent<int> OnGoldChanged;

    public int CurrentGold => _currentGold;

    private void Start()
    {
        // 초기 골드 UI 업데이트
        OnGoldChanged?.Invoke(_currentGold);
    }

    /// <summary>
    /// 골드 추가
    /// </summary>
    public void AddGold(int amount)
    {
        if (amount <= 0) return;

        _currentGold += amount;
        OnGoldChanged?.Invoke(_currentGold);

        Debug.Log($"[플레이어] 골드 +{amount} (총: {_currentGold})");
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0) return false;
        if (_currentGold < amount)
        {
            Debug.Log("[플레이어] 골드가 부족합니다!");
            return false;
        }

        _currentGold -= amount;
        OnGoldChanged?.Invoke(_currentGold);

        Debug.Log($"[플레이어] 골드 -{amount} (총: {_currentGold})");
        return true;
    }

    public bool HasGold(int amount)
    {
        return _currentGold >= amount;
    }

    public void ResetGold()
    {
        _currentGold = 0;
        OnGoldChanged?.Invoke(_currentGold);
    }
}