using System.Collections;
using System.IO;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance;

    private EGameState _state = EGameState.Ready;
    public EGameState State => _state;

    [SerializeField] private TextMeshProUGUI _stateTextUI;

    private void Awake()
    {
        instance = this;    
    }
    private void Start()
    {
        _state = EGameState.Ready;
        _stateTextUI.text = "준비중...";
    }

    private IEnumerator StartToPlay_Coroutine()
    {
        yield return new WaitForSeconds(2f);
        _stateTextUI.text = "시작!";

        yield return new WaitForSeconds(0.5f);
        _state = EGameState.Playing;

        _stateTextUI.gameObject.SetActive(false);
    }

}