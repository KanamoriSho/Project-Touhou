using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField, Label("メニューボタン")]
    private GameObject[] _buttonObject = default;

    [SerializeField, Label("選択時のスプライト")]
    private Sprite[] _spritesAtSelection = default;

    [SerializeField, Label("カーソル移動間隔"), Range(0.0f, 1.0f)]
    private float _cursoreIntervalTime = 0.1f;

    [SerializeField, Label("エラー音")]
    private AudioClip _errorSound = default;

    private AudioSource _audioSource = default;
    
    private List<Image> _buttonImage = new List<Image>();

    private List<Sprite> _defaultSprites = new List<Sprite>();

    private const float _magnificationScalse = 1.1f;

    private WaitForSeconds _cursoreInterval = default;

    private Animator _animator = default;
    
    private int _currentSelect = 0;

    private bool _isStarted = false;

    private bool _isInterval = false;

    private void Awake()
    {

        //Animator取得
        _animator = this.GetComponent<Animator>();

        //AudioSource取得
        _audioSource = this.GetComponent<AudioSource>();

        //カーソルの移動間隔をキャッシュ
        _cursoreInterval = new WaitForSeconds(_cursoreIntervalTime);

        //
        for (int i = 0; i < _buttonObject.Length; i++)
        {
            _buttonImage.Add(_buttonObject[i].GetComponent<Image>());

            _defaultSprites.Add(_buttonImage[i].sprite);
        }
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            if(!_isStarted)
            {
                _isStarted = true;

                _buttonObject[_currentSelect].transform.localScale = new Vector3(_magnificationScalse, _magnificationScalse, 1);

                _buttonImage[_currentSelect].sprite = _spritesAtSelection[_currentSelect];

                _animator.SetTrigger("ButtonPressed");
            }
            else
            {
                SceneMove();
            }
        }

        if(!_isStarted || _isInterval)
        {
            return;
        }

        if(Input.GetAxisRaw("Vertical") == 0)
        {
            return;
        }
        else if(Input.GetAxisRaw("Vertical") > 0)
        {
            ScaleReset();

            _currentSelect--;

            if(_currentSelect < 0)
            {
                _currentSelect = _buttonObject.Length - 1;
            }
        }
        else
        {
            ScaleReset();

            _currentSelect++;

            if (_currentSelect >= _buttonObject.Length)
            {
                _currentSelect = 0;
            }
        }

        _buttonObject[_currentSelect].transform.localScale = new Vector3(_magnificationScalse, _magnificationScalse, 1);

        _buttonImage[_currentSelect].sprite = _spritesAtSelection[_currentSelect];

        StartCoroutine(CursoreInterval());
    }

    private void ScaleReset()
    {
        _buttonObject[_currentSelect].transform.localScale = Vector3.one;

        _buttonImage[_currentSelect].sprite = _defaultSprites[_currentSelect];
    }

    private void SceneMove()
    {
        switch(_currentSelect)
        {
            case 0:

                SceneManager.LoadScene("Stage1");

                break;

            case 1:

                MakeASound();

                break;

            case 2:

                MakeASound();

                break;

            case 3:

                MakeASound();

                break;

            case 4:

                MakeASound();

                break;

            case 5:

                MakeASound();

                break;

            case 6:

                MakeASound();

                break;

            case 7:

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif

                break;
        }
    }

    private void MakeASound()
    {
        _audioSource.PlayOneShot(_errorSound);
    }

    private IEnumerator CursoreInterval()
    {
        _isInterval = true;

        yield return _cursoreInterval;

        _isInterval = false;
    }
}
