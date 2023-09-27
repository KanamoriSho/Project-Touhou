using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TalkManager : MonoBehaviour
{
    [SerializeField, Label("キャラ")]
    private Image[] _characters = default;


    private enum Speaker            //話者格納用
    {
        [EnumLabel("話者", "プレイヤー")]
        player = 0,
        [EnumLabel("話者", "ボス")]
        boss = 1,
    }

    [SerializeField, Label("話者"), EnumElements(typeof(Speaker))]
    private Speaker[] _speakers = default;

    [SerializeField, Label("台詞")]
    private string[] _lines = default;

    [SerializeField, Label("立ち絵")]
    private Sprite[] _emotions = default;

    [SerializeField, Label("立ち絵登場タイミング")]
    private bool[] _entryTimings = default;

    [SerializeField, Label("ボス名前表示タイミング")]
    private int _nameAppearanceNumber = default;

    [SerializeField, Label("画面内ボス登場タイミング")]
    private int _bossAppearanceNumber = default;

    [SerializeField, Label("戦闘前会話終了タイミング")]
    private int _firstTalkEndNumber = default;

    private TextMeshProUGUI _talkText = default;                //台詞表示用テキスト

    private int _lineNumber = 0;                                //台詞番号格納用

    private bool _isTalkEnd = false;                            //会話終了フラグ

    private bool _isTalked = false;                             //一度会話を終えたかを格納するフラグ

    private bool _isCurrentTalkEnd = false;                     //現在の会話を終えたかを格納するフラグ

    private bool _isBossAppear = false;

    private Animator _animator = default;                       //自身のAnimator格納用

    private const string ANIM_START_TALK = "StartTalk";         //AnimatorTrigger会話開始

    private const string ANIM_END_TALK = "EndTalk";             //AnimatorTrigger会話終了

    private const string ANIM_BOSS_APPEAR = "BossAppearance";   //AnimatorTriggerボス登場

    private const string ANIM_PLAYER_TALK = "PlayerTalk";       //AnimatorBoolプレイヤー発言

    private const string ANIM_BOSS_TALK = "BossTalk";           //AnimatorBoolボス発言

    private const string ANIM_BOSS_NAME = "BossNameAppearance"; //AnimatorBoolボス名前表示

    public bool GetIsTalkEnd
    {
        //_isTalkEndを返す
        get { return _isTalkEnd; }
    }

    public bool SetIsTalkEnd
    {
        //値を_isTalkEndに格納
        set { _isTalkEnd = value; }
    }

    public bool GetIsBossAppear
    {
        get { return _isBossAppear; }
    }

    private void Awake()
    {

        //_talkTextの取得
        _talkText = this.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        //自身のAnimatorを取得
        _animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        //会話が終了している?
        if (_isTalkEnd)
        {
            //している

            //何もしない
            return;
        }

        //Zキーが押された?
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //押された

            //発言済みフラグをfalseに
            _isCurrentTalkEnd = false;

            //台詞番号を進める
            _lineNumber++;
        }



        //台詞番号が配列の要素数を超えた?
        if (_lineNumber >= _lines.Length)
        {
            //越えた

            //会話終了のAnimatorTriggerを起動
            _animator.SetTrigger(ANIM_END_TALK);

            _isTalkEnd = true;

            return;
        }
        //台詞番号がボス戦開始前会話の終了番号になったか
        else if (_lineNumber == _firstTalkEndNumber)
        {
            //台詞番号が会話終了番号に達した

            //会話済みフラグがfalse?
            if (!_isTalked)
            {
                //false

                //会話済みフラグをtrueに
                _isTalked = true;

                //現在の会話を終えたフラグをtrueに
                _isCurrentTalkEnd = true;

                //会話終了のAnimatorTriggerを起動
                _animator.SetTrigger(ANIM_END_TALK);

                _isTalkEnd = true;

            }
            else
            {
                //現在の会話を終えたフラグをfalse
                _isCurrentTalkEnd = false;
            }
        }
        else if (_lineNumber == _bossAppearanceNumber)
        {
            _isBossAppear = true;
        }

        //越えていない

        //キャラの登場フラグが立っている?
        if (_entryTimings[_lineNumber] && !_isCurrentTalkEnd)
        {
            //立っている

            //話者はプレイヤー?
            if ((int)_speakers[_lineNumber] == 0)
            {
                //プレイヤー

                //テキストの有効化
                _talkText.enabled = true;

                //会話開始のAnimatorTriggerを起動
                _animator.SetTrigger(ANIM_START_TALK);

                //ボス発言のAnimatorBoolをfalseに
                _animator.SetBool(ANIM_BOSS_TALK, false);
            }
            else
            {
                //ボス

                //ボス登場のAnimatorBoolをtrueに
                _animator.SetBool(ANIM_BOSS_APPEAR, true);

                //プレイヤー発言のAnimatorBoolをfalseに
                _animator.SetBool(ANIM_PLAYER_TALK, false);
            }

            //台詞番号がボスの名前表示タイミングと同じか
            if (_lineNumber == _nameAppearanceNumber)
            {
                //同じ

                //名前表示AnimatorTriggerを起動
                _animator.SetTrigger(ANIM_BOSS_NAME);
            }

            //現在の会話を終えたフラグをtrueに
            _isCurrentTalkEnd = true;

        }
        else
        {
            //立っていない

            //話者がプレイヤーか?
            if ((int)_speakers[_lineNumber] == 0)
            {
                //プレイヤー

                //プレイヤー発言AnimatorBoolをtrueに
                _animator.SetBool(ANIM_PLAYER_TALK, true);

                //ボス発言AnimatorBoolをfalseに
                _animator.SetBool(ANIM_BOSS_TALK, false);
            }
            else
            {
                //ボス

                //プレイヤー発言AnimatorBoolをfalseに
                _animator.SetBool(ANIM_PLAYER_TALK, false);

                //ボス発言AnimatorBoolをtrueに
                _animator.SetBool(ANIM_BOSS_TALK, true);
            }
        }

        //現在のセリフ番号に対応したセリフを表示
        _talkText.text = _lines[_lineNumber];
        
        //現在のセリフ番号に対応した立ち絵を表示
        _characters[(int)_speakers[_lineNumber]].sprite = _emotions[_lineNumber];
    }

    /// <summary>
    /// <para>EndTalk</para>
    /// <para>Animation Event用。 会話終了時にアニメーションに合わせてフラグを初期化</para>
    /// </summary>
    public void EndTalk()
    {
        _isCurrentTalkEnd = true;

        _isTalkEnd = true;
    }

}
