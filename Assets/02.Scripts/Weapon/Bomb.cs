using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject _explosionEffectPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        // 내 위치에 폭탄 생성
        GameObject effectObject = Instantiate(_explosionEffectPrefab);
        effectObject.transform.position = transform.position;

        // 충돌하면 나 자신을 삭제한다.
        Destroy(gameObject);
    }
}