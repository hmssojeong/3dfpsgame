using UnityEngine;

// 카메라 회전 기능
// 마우스를 조작하면 카메라를 그 방향으로 회전하고 싶다.
public class CameraRotate : MonoBehaviour
{
    private void Update()
    {
        // 1. 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Debug.Log($"{mouseX}, {mouseY}");

        // 2. 입력에 따른 회전 방향 만들기
        Vector3 Direction = new Vector3(mouseX, mouseY, 0);

        // 3. 회전방향으로 카메라 회전하기
    }

}
