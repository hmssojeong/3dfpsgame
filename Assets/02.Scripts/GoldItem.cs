using System.Collections;
using UnityEngine;

public class GoldItem : MonoBehaviour
{
    [SerializeField] private int _goldAmount = 10;

    [SerializeField] private float _attractDistance = 5f;
    [SerializeField] private float _collectDistance = 1f;
    [SerializeField] private float _attractSpeed = 8f;
    [SerializeField] private float _bezierHeight = 2f;

    //물리
    [SerializeField] private float _dropForce = 5f;
    [SerializeField] private float _dropUpForce = 3f;
    [SerializeField] private float _groundCheckDistance = 0.1f;

    private Transform _player;
    private Rigidbody _rigidbody;
    private bool _isAttracted = false;
    private bool _isGrounded = false;
    private Vector3 _startPosition;
    private float _attractTimer = 0f;

    [SerializeField] private float _rotationSpeed = 360f;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            _rigidbody = gameObject.AddComponent<Rigidbody>();
        }
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void Drop(Vector3 dropPosition)
    {
        transform.position = dropPosition;

        Vector2 randomDirection = Random.insideUnitCircle;
        Vector3 force = new Vector3(randomDirection.x, 0f, randomDirection.y) * _dropForce;
        force.y = _dropUpForce;

        _rigidbody.AddForce(force, ForceMode.Impulse);

        //회전
        _rigidbody.AddTorque(Random.insideUnitSphere * 1f, ForceMode.Impulse);

        StartCoroutine(GroundCheck_Coroutine());
    }

    private IEnumerator GroundCheck_Coroutine()
    {
        yield return new WaitForSeconds(0.3f);

        while (!_isGrounded)
        {
            // Raycast로 땅 체크
            if (Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance))
            {
                _isGrounded = true;
                _rigidbody.isKinematic = true;
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update()
    {
        if (_player == null)
        {
            return;
        }

        if (!_isGrounded)
        {
            return;
        }

        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (distanceToPlayer <= _collectDistance)
        {
            CollectGold();
            return;
        }

        if (!_isAttracted && distanceToPlayer <= _attractDistance)
        {
            _isAttracted = true;
            _startPosition = transform.position;
            _attractTimer = 0f;
        }

        if (_isAttracted)
        {
            MoveToPlayerWithBezier();
        }
    }

    private void MoveToPlayerWithBezier()
    {
        _attractTimer += Time.deltaTime * _attractSpeed;
        float t = Mathf.Clamp01(_attractTimer);


        Vector3 p0 = _startPosition;
        Vector3 p2 = _player.position + Vector3.up * 0.5f;
        Vector3 p1 = (p0 + p2) / 2f + Vector3.up * _bezierHeight;

        // Quadratic Bezier 공식: B(t) = (1-t)^2 * P0 + 2(1-t)t * P1 + t^2 * P2
        Vector3 newPosition = Mathf.Pow(1 - t, 2) * p0 +
                             2 * (1 - t) * t * p1 +
                             Mathf.Pow(t, 2) * p2;

        transform.position = newPosition;

        if (t >= 5f)
        {
            CollectGold();
        }
    }

    private void CollectGold()
    {
        PlayerGold playerGold = _player.GetComponent<PlayerGold>();
        if (playerGold != null)
        {
            playerGold.AddGold(_goldAmount);
        }

        Destroy(gameObject);
    }

    public void SetGoldAmount(int amount)
    {
        _goldAmount = amount;
    }
}
