using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float RotationSpeed = 200f; // 0 ~ 360

    private float _accumulationX = 0;

    private void Update()
    {
        // 게임 시작하면 y축이 0도에서 -> -1도

/*        if (!Input.GetMouseButton(1))
        {
            return;
        }*/   //우클릭할때만 회전 가능하게 했던 코드

        float mouseX = Input.GetAxis("Mouse X");
        _accumulationX += mouseX * RotationSpeed * Time.deltaTime; //  범위가 없다.

 
        transform.eulerAngles = new Vector3(0, _accumulationX);
    }
}
