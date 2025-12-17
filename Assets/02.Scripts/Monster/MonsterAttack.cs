using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    [SerializeField] private Monster _monster;

    private void Awake()
    {
        if(_monster == null)
        {
            _monster = GetComponentInParent<Monster>();
        }
    }

    public void PlayerAttack()
    {
        PlayerStats player = GameObject.FindAnyObjectByType<PlayerStats>();
        player.PlayerTakeDamage(_monster.AttackDamage);
    }
}
