using UnityEngine;

// 구조체: 서로 다른 종류의 변수들을 하나의 이름으로 묶어 새로운 사용자 정의 자료형을 만드는 것
public struct Damage
{
    public float Value;
    public Vector3 HitPoint;
    public Vector3 AttackerPos;
    public GameObject Who;
    public bool Critical;
    public Vector3 Normal;
}
