using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private EGameState _state = EGameState.Ready;
    public EGameState State => _state;

    [SerializeField] private TextMeshProUGUI _stateTextUI;
    [SerializeField] private UI_OptionPopup _optionPopupUI;

    private void Awake()
    {
        _instance = this;

        LockCursor();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }



    private void Start()
    {
        _stateTextUI.gameObject.SetActive(true);

        _state = EGameState.Ready;
        _stateTextUI.text = "준비중...";

        StartCoroutine(StartToPlay_Coroutine());
    }

    public void TriggerGameOver()
    {
        if (_state != EGameState.GameOver)
        {
            StartCoroutine(GameOver_Coroutine());
        }
    }

    private IEnumerator StartToPlay_Coroutine()
    {
        yield return new WaitForSeconds(2f);

        _stateTextUI.text = "시작!";

        yield return new WaitForSeconds(0.5f);

        _state = EGameState.Playing;

        _stateTextUI.gameObject.SetActive(false);
    }

    private IEnumerator GameOver_Coroutine()
    {
        _state = EGameState.GameOver;
        _stateTextUI.gameObject.SetActive(true);
        _stateTextUI.text = "GameOver";

        yield return null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();

            _optionPopupUI.Show();
        }
    }

    private void Pause()
    {
        Time.timeScale = 0;

        UnlockCursor();
    }

    public void Continue()
    {
        Time.timeScale = 1;

        LockCursor();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Quit()
    {
        // 게임 종료 전 필요한 로직을 실행한다.

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}

