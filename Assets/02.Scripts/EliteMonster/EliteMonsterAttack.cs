using UnityEngine;

public class EliteMonsterAttack : MonoBehaviour
{
    [SerializeField] private EliteMonster _eliteMonster;
    private Animator _animator;

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
        
    }

    public void PerformChargeAttack()
    {
       
    }
}