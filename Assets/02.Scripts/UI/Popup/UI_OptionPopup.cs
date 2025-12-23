using UnityEngine;
using UnityEngine.UI;

public class UI_OptionPopup : MonoBehaviour
{
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _gameExitButton;


    private void Start()
    {
        // 콜백함수: 어떤 이벤트가 일어나면 자동으로 호출 되는 함수
        _continueButton.onClick.AddListener(GameContinue);
        _restartButton.onClick.AddListener(GameRestart);
        _gameExitButton.onClick.AddListener(GameExit);

        Hide();
    }


    public void Show()
    {
        gameObject.SetActive(true);

        // todo
        // 1. 애니메이션 처리
        // 2. 사운드 처리
        // 3. 이펙트 처리
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        // todo
        // 1. 애니메이션 처리
        // 2. 사운드 처리
        // 3. 이펙트 처리
    }



    // 함수란 한가지 기능만 해야되고, 그 기능이 무엇을 하는지(의도, 결과)가 나타나는 이름을 가져야된다.
    // ~클릭햇을때 라는 이름은 기능의 이름이 아니라 "언제 호출되는지"가 드러나 있다.
    private void GameContinue()
    {
        GameManager.Instance.Continue();

        Hide();
    }

    private void GameRestart()
    {

    }

    private void GameExit()
    {

    }

}