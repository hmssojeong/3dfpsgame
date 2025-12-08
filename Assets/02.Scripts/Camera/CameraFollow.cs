using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target; // The target for the camera to follow

    private void LateUpdate()
    {
       transform.position = Target.position;
    }
}
