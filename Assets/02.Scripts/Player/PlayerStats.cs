using UnityEngine;

// 플레이어의 '스탯'을 관리하는 컴포넌트
public class PlayerStats : MonoBehaviour
{
    // 도메인: 특정 분야의 지식

    // 스태미나 스탯 (소모가능한 스탯)
    public float MaxStamina;
    public float Stamina;
    public float StaminaRegen;
    
    // 체력 (소모가능한 스탯)
    public float MaxHealth;
    public float Health;
    public float HealthRegen;

    // 스탯 (값스탯)
    public ValueStat Damage;
    public ValueStat MoveSpeed;
    public ValueStat RunSpeed;

    
    // 스태미나, 체력 스탯 관련 코드(회복, 소모, 업그레이드,)
}
