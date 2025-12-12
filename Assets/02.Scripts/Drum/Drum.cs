using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Drum : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [SerializeField] private ConsumableStat _health;
    [SerializeField] private ValueStat _damage;
    [SerializeField] private ParticleSystem _explosionParticePrefab;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _health.Initialize();
    }

    public bool TryTakeDamage(float damage)
    {
        if (_health.Value <= 0) return false;

        _health.Decrease(damage);

        if (_health.Value <= 0)
        {
            Explode();
        }

        return true;
    }

    private void Explode()
    {
        ParticleSystem explosionParticle = Instantiate(_explosionParticePrefab);
        explosionParticle.transform.position = this.transform.position;
        explosionParticle.Play();

        Destroy(this.gameObject);
    }
}