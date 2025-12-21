using UnityEngine;

public class EliteMonsterKnockBack : MonoBehaviour
{
    [Header("넉백 설정")]
    [SerializeField] private float _knockbackForce = 2.5f;
    [SerializeField] private float _knockbackTime = 0.3f;

    private CharacterController _controller;
    private Vector3 _knockbackDirection;
    private float _knockbackTimer = 0f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public void ApplyKnockback(Vector3 attackerPos)
    {
        _knockbackDirection = (transform.position - attackerPos).normalized;
        _knockbackTimer = _knockbackTime;
    }

    private void Update()
    {
        if (_knockbackTimer > 0)
        {
            _knockbackTimer -= Time.deltaTime;
            _controller.Move(_knockbackDirection * _knockbackForce * Time.deltaTime);
        }
    }

    public bool IsKnockbacking()
    {
        return _knockbackTimer > 0;
    }
}