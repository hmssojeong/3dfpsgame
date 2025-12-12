/*using UnityEngine;

public class PracticeDrum : MonoBehaviour
{
    // 체력이 있습니다.
    private ValueStat _health;

    [Header("폭발 설정")]
    public float ExplosionRadius = 2;
    public float Damage = 1000;
    public float ExplosionForce = 500f;


    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject ExplosionPrefab;

    private Rigidbody _rigidbody;
    

    private void Start()
    {
        _health = GetComponent<ValueStat>();
        Rigidbody rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Bomb bomb = GetComponent<Bomb>();

    }

    private void DrumHit()
    {
        _health -= damage;

        if (_health > 0)
        {
            Debug.Log($"남은체력{_health}");
        }
        else
        {
            DestroyDrum();
        }
    }

    private void DestroyDrum()
    {
        Instantiate(ExplosionPrefab,direction*transform.position);
        Vector3 direction = (transform.position - _player.transform.position).normalized;
        _rigidbody.AddForce(Vector3.up * ExplosionForce);
    }

    private void DrumNearbyDamage()
    {
        // 거리 5f
        // radious = 5f
        // 주변 5f 안에 attack
    }



}
*/