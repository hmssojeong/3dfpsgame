using UnityEngine;

/// <summary>
/// 간단한 폭탄 개수 표시 UI (OnGUI 사용)
/// Canvas 없이도 작동하며, 왼쪽 하단에 폭탄 정보를 표시합니다
/// </summary>
public class SimpleBombUI : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private bool _showDebugInfo = true;

    private PlayerBombFire _playerFire;

    private void Start()
    {
        _playerFire = GetComponent<PlayerBombFire>();

        if (_playerFire == null)
        {
            _playerFire = FindObjectOfType<PlayerBombFire>();
        }

        if (_playerFire == null)
        {
            Debug.LogError("PlayerFire를 찾을 수 없습니다!");
            enabled = false;
        }
    }

    private void OnGUI()
    {
        if (_playerFire == null) return;

        // 스타일 설정
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.fontSize = 20;
        boxStyle.normal.textColor = Color.white;
        boxStyle.alignment = TextAnchor.MiddleCenter;
        boxStyle.fontStyle = FontStyle.Bold;

        GUIStyle textStyle = new GUIStyle(GUI.skin.label);
        textStyle.fontSize = 24;
        textStyle.fontStyle = FontStyle.Bold;
        textStyle.alignment = TextAnchor.MiddleLeft;

        // 배경 박스 (왼쪽 하단)
        GUI.Box(new Rect(10, Screen.height - 120, 200, 110), "", boxStyle);

        // 폭탄 아이콘 (이모지)
        GUIStyle iconStyle = new GUIStyle(GUI.skin.label);
        iconStyle.fontSize = 40;
        GUI.Label(new Rect(20, Screen.height - 110, 60, 60), "💣", iconStyle);

        // 폭탄 개수
        Color ammoColor = _playerFire.CurrentBombCount > 0 ? Color.white : Color.red;
        textStyle.normal.textColor = ammoColor;

        GUI.Label(new Rect(80, Screen.height - 100, 120, 40),
            $"{_playerFire.CurrentBombCount}/{_playerFire.MaxBombCount}", textStyle);

        // 재장전 중 표시
        if (_playerFire.IsReloading)
        {
            GUIStyle reloadStyle = new GUIStyle(GUI.skin.label);
            reloadStyle.fontSize = 16;
            reloadStyle.normal.textColor = Color.yellow;
            reloadStyle.fontStyle = FontStyle.Bold;

            GUI.Label(new Rect(80, Screen.height - 60, 120, 30),
                "재장전 중...", reloadStyle);

            // 재장전 바
            float progress = _playerFire.ReloadProgress;
            Rect barBg = new Rect(80, Screen.height - 35, 110, 10);
            Rect barFill = new Rect(80, Screen.height - 35, 110 * progress, 10);

            GUI.DrawTexture(barBg, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, Color.gray, 0, 0);
            GUI.DrawTexture(barFill, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0, Color.yellow, 0, 0);
        }
        else
        {
            // R키 힌트
            GUIStyle hintStyle = new GUIStyle(GUI.skin.label);
            hintStyle.fontSize = 14;
            hintStyle.normal.textColor = Color.gray;

            GUI.Label(new Rect(80, Screen.height - 50, 120, 30),
                "R: 재장전", hintStyle);
        }

        // 디버그 정보 (선택사항)
        if (_showDebugInfo)
        {
            GUIStyle debugStyle = new GUIStyle(GUI.skin.label);
            debugStyle.fontSize = 12;
            debugStyle.normal.textColor = Color.cyan;

            GUI.Label(new Rect(220, Screen.height - 100, 300, 20),
                "우클릭: 폭탄 던지기", debugStyle);
            GUI.Label(new Rect(220, Screen.height - 80, 300, 20),
                "R키: 수동 재장전", debugStyle);
        }
    }
}