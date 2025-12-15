using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private bool isLockCursor = true;

    void Awake()
    {
        LockCursor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isLockCursor)
            {
                UnlockCursor();
            }
            else
            {
                LockCursor();
            }
        }
    }
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isLockCursor = true;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isLockCursor = false;
    }
}