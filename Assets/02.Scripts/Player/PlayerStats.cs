using UnityEngine;

// 플레이어의 '스탯'을 관리하는 컴포넌트
public class PlayerStats : MonoBehaviour
{
    // 고민해볼 거리
    // 1. 옵저버 패턴은 어떻게 해야지?
    // 2. ConsumableStat의 Regenerate는 PlayerStats에서만 호출 가능하게 하고 싶다. 다른 속성/기능은 다른 클래스에서 사용할 수 있다

    public ConsumableStat Health;
    public ConsumableStat Stamina;
    public ValueStat Damage;
    public ValueStat MoveSpeed;
    public ValueStat RunSpeed;
    public ValueStat JumpPower;

    public event System.Action OnDamaged;

    private void Start()
    {
        Health.Initialize();
        Stamina.Initialize();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        Health.Regenerate(deltaTime);
        Stamina.Regenerate(deltaTime);
    }

    public void PlayerTakeDamage(float amount)
    {
        Health.Consume(amount);
        Debug.Log($"플레이어가 {amount}만큼 데미지를 입었습니다.");
        OnDamaged?.Invoke();
        if(Health.Value <= 0)
        {
            Die();
        }

    }

    private void Die()
    {
        Debug.Log("플레이어가 사망했습니다.");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver();
        }        
    }

    public float GetHealthPercentage()
    {
        if (Health.MaxValue == 0) return 0;
        return Health.Value / Health.MaxValue;
    }

    public float GetStaminaPercentage()
    {
        if (Stamina.MaxValue == 0) return 0;
        return Stamina.Value / Stamina.MaxValue;
    }


}