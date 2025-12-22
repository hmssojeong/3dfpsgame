using UnityEngine;

public class EliteMonsterAttack : MonoBehaviour
{
    [SerializeField] private EliteMonster _eliteMonster;

    private void Awake()
    {
        if (_eliteMonster == null)
        {
            _eliteMonster = GetComponentInParent<EliteMonster>();
        }
    }

    public void PerformNormalAttack()
    {
        PlayerStats player = GameObject.FindAnyObjectByType<PlayerStats>();
        if (player != null)
        {
            float damage = _eliteMonster.AttackDamage;
            player.PlayerTakeDamage(damage);
            Debug.Log($"일반 공격 데미지: {damage}");
        }
    }

    public void PerformHeavyAttack()
    {
        //공격 애니메이션
    }

    public void PerformChargeAttack()
    {
        //돌진 애니메이션
    }
}