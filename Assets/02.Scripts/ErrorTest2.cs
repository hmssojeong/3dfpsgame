using UnityEngine;

public class ErrorTest2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        // MissingReferenceException : 보통 삭제한 게임 오브젝트를 참조/접근(필드,메서드) 하려고 할 때 뜹니다.
        Destroy(gameobject);
        Debug.Log(gameobject.name);

        // IndexOutOfRangeException : 배열(리스트)에서 유효하지 않은 인덱스에 접근할 때
        int[] numbers = new int[10];
        Debug.Log(numbers[13]);

        // DevideByZeroException : 0으로 나누기를 시도할 때
        int number1 = 13;
        int number2 = 0;
        Debug.Log(message: number1 / number2);
    }
}
