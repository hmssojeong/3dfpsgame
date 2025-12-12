/*using UnityEngine;
using System.Collections;

public class CoroutineExample1 : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Test1();
            // 코루틴: 협력(Co) + 루틴(Routine)의 합성어: 협력 동작
            StartCoroutine(Test2_Coroutine());
            // 1초 시간이 걸린다. (렉, 프리징, 블로킹)
            // -> 코드를 '동기'적으로 실행하는게 아니라 '비동기'적으로 실행하고 싶다.
            //   동기: 이전 코드가 실행 완료된 다음에 그 다음 코드를 실행하는 것
            // 비동기: 이전 코드가 실행 완료 여부와 상관없이 다음(다른) 코드를 실행하는 것
            // 유니티에서는 '비동기' 방식을 지원하기 위해 "코루틴"이라는 기능을 제공한다.
            Test3();
        }
    }
    private void Test1()
    {
        Debug.Log("Test1");
    }

    private IEnumerator Test2_Coroutine()
    {
        Debug.Log("텀블러좀 가져와주세요.");
        // 1초정도의 시간이 걸리는 로직 (1~10초 사이)
        int sum = 0;
        for (int i=0; i<100; i++)
        {
            // yield 키워드를 이용하면 코루틴 함수의 실행을 중단하고 이어할 수 있다.
            yield return null;                      // 다음 프레임까지 쉰다.
            yield return new WaitForSeconds(3f);    //3초쉰다.
            yield break;                            //코루틴을 끝낸다.

            for (int j = 0; j < 10000000; j++)
            {
                sum += (i*j);
            }
        }
        
        Debug.Log("네 가져왔습니다.");
    }

    private void Test3()
    {
        Debug.Log("Test3");
    }
}
*/