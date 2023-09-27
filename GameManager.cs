using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //private static GameManager _instance = null;

    private GameObject _player = default;

    private GameObject _boss = default;

    private PlayerMove _playerMove = default;

    private EnemyCharacterMove _bossMove = default;

    [SerializeField, Label("ゲームオーバーUI")]
    private SpriteRenderer _gameOverUI = default;

    [SerializeField, Label("ステージクリアUI")]
    private GameObject _stageClearUI = default;

    [SerializeField, Label("ボスHPバー")]
    private Slider _bossHPBar = default;

    [SerializeField]
    private TalkManager _talkManager = default;

    private bool _isTalking = false;

    private bool _isTalkOnce = false;

    private bool _isBossFirstAppear = false;

    //public GameManager GetInstance
    //{
    //    get { return _instance; }
    //}

    private void Awake()
    {

        _player = GameObject.FindGameObjectWithTag("Player");

        _player.SetActive(true);

        _boss = GameObject.FindGameObjectWithTag("Boss");

        _boss.SetActive(true);

        _playerMove = _player.GetComponent<PlayerMove>();

        _bossMove = _boss.GetComponent<EnemyCharacterMove>();

        _boss.SetActive(false);

        ChangeTimeScale(false);

        _gameOverUI.enabled = false;

        _stageClearUI.SetActive(false);

        _isTalking = true;

        _isBossFirstAppear = false;

        _isTalkOnce = false;


}

    private void Update()
    {
        if(SceneManager.GetActiveScene().name == "Title")
        {
            return;
        }

        if(_talkManager.GetIsBossAppear && !_isBossFirstAppear)
        {
            _boss.SetActive(true);

            _isBossFirstAppear = true;
        }

        if(_talkManager.GetIsTalkEnd)
        {
            _isTalking = false;
        }
        else
        {
            _isTalking = true;
        }

        _bossMove.SetIsTalking = _isTalking;

        _playerMove.SetIsTalking = _isTalking;


        _bossHPBar.value = _bossMove.GetCurrentHP;

        bool isPlayerDead = _playerMove.GetIsPlayerDead;

        bool isBossDead = _bossMove.GetIsDead;

        if(isPlayerDead)
        {
            ChangeTimeScale(true);

            _gameOverUI.enabled = true;

            if (Input.GetKeyDown(KeyCode.Z))
            {
                SceneManager.LoadScene("Title");
            }
        }

        if(isBossDead)
        {
            _isTalking = true;

            if (!_isTalkOnce)
            {
                _isTalkOnce = true;

                _talkManager.SetIsTalkEnd = false;
            }

            _bossHPBar.enabled = false;

            if (_talkManager.GetIsTalkEnd)
            {
                _stageClearUI.SetActive(true);

                if(Input.GetKeyDown(KeyCode.Z))
                {
                    _boss.SetActive(true);

                    SceneManager.LoadScene("Title");
                }
            }

        }


    }



    private void ChangeTimeScale(bool togle)
    {
        if(togle)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
