using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    private bool isLockCursor = true;

    void Awake()
    {
        LockCursor();
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




