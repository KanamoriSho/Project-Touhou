using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TalkManager : MonoBehaviour
{
    [SerializeField, Label("�L����")]
    private Image[] _characters = default;


    private enum Speaker            //�b�Ҋi�[�p
    {
        [EnumLabel("�b��", "�v���C���[")]
        player = 0,
        [EnumLabel("�b��", "�{�X")]
        boss = 1,
    }

    [SerializeField, Label("�b��"), EnumElements(typeof(Speaker))]
    private Speaker[] _speakers = default;

    [SerializeField, Label("�䎌")]
    private string[] _lines = default;

    [SerializeField, Label("�����G")]
    private Sprite[] _emotions = default;

    [SerializeField, Label("�����G�o��^�C�~���O")]
    private bool[] _entryTimings = default;

    [SerializeField, Label("�{�X���O�\���^�C�~���O")]
    private int _nameAppearanceNumber = default;

    [SerializeField, Label("��ʓ��{�X�o��^�C�~���O")]
    private int _bossAppearanceNumber = default;

    [SerializeField, Label("�퓬�O��b�I���^�C�~���O")]
    private int _firstTalkEndNumber = default;

    private TextMeshProUGUI _talkText = default;                //�䎌�\���p�e�L�X�g

    private int _lineNumber = 0;                                //�䎌�ԍ��i�[�p

    private bool _isTalkEnd = false;                            //��b�I���t���O

    private bool _isTalked = false;                             //��x��b���I���������i�[����t���O

    private bool _isCurrentTalkEnd = false;                     //���݂̉�b���I���������i�[����t���O

    private bool _isBossAppear = false;

    private Animator _animator = default;                       //���g��Animator�i�[�p

    private const string ANIM_START_TALK = "StartTalk";         //AnimatorTrigger��b�J�n

    private const string ANIM_END_TALK = "EndTalk";             //AnimatorTrigger��b�I��

    private const string ANIM_BOSS_APPEAR = "BossAppearance";   //AnimatorTrigger�{�X�o��

    private const string ANIM_PLAYER_TALK = "PlayerTalk";       //AnimatorBool�v���C���[����

    private const string ANIM_BOSS_TALK = "BossTalk";           //AnimatorBool�{�X����

    private const string ANIM_BOSS_NAME = "BossNameAppearance"; //AnimatorBool�{�X���O�\��

    public bool GetIsTalkEnd
    {
        //_isTalkEnd��Ԃ�
        get { return _isTalkEnd; }
    }

    public bool SetIsTalkEnd
    {
        //�l��_isTalkEnd�Ɋi�[
        set { _isTalkEnd = value; }
    }

    public bool GetIsBossAppear
    {
        get { return _isBossAppear; }
    }

    private void Awake()
    {

        //_talkText�̎擾
        _talkText = this.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        //���g��Animator���擾
        _animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        //��b���I�����Ă���?
        if (_isTalkEnd)
        {
            //���Ă���

            //�������Ȃ�
            return;
        }

        //Z�L�[�������ꂽ?
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //�����ꂽ

            //�����ς݃t���O��false��
            _isCurrentTalkEnd = false;

            //�䎌�ԍ���i�߂�
            _lineNumber++;
        }



        //�䎌�ԍ����z��̗v�f���𒴂���?
        if (_lineNumber >= _lines.Length)
        {
            //�z����

            //��b�I����AnimatorTrigger���N��
            _animator.SetTrigger(ANIM_END_TALK);

            _isTalkEnd = true;

            return;
        }
        //�䎌�ԍ����{�X��J�n�O��b�̏I���ԍ��ɂȂ�����
        else if (_lineNumber == _firstTalkEndNumber)
        {
            //�䎌�ԍ�����b�I���ԍ��ɒB����

            //��b�ς݃t���O��false?
            if (!_isTalked)
            {
                //false

                //��b�ς݃t���O��true��
                _isTalked = true;

                //���݂̉�b���I�����t���O��true��
                _isCurrentTalkEnd = true;

                //��b�I����AnimatorTrigger���N��
                _animator.SetTrigger(ANIM_END_TALK);

                _isTalkEnd = true;

            }
            else
            {
                //���݂̉�b���I�����t���O��false
                _isCurrentTalkEnd = false;
            }
        }
        else if (_lineNumber == _bossAppearanceNumber)
        {
            _isBossAppear = true;
        }

        //�z���Ă��Ȃ�

        //�L�����̓o��t���O�������Ă���?
        if (_entryTimings[_lineNumber] && !_isCurrentTalkEnd)
        {
            //�����Ă���

            //�b�҂̓v���C���[?
            if ((int)_speakers[_lineNumber] == 0)
            {
                //�v���C���[

                //�e�L�X�g�̗L����
                _talkText.enabled = true;

                //��b�J�n��AnimatorTrigger���N��
                _animator.SetTrigger(ANIM_START_TALK);

                //�{�X������AnimatorBool��false��
                _animator.SetBool(ANIM_BOSS_TALK, false);
            }
            else
            {
                //�{�X

                //�{�X�o���AnimatorBool��true��
                _animator.SetBool(ANIM_BOSS_APPEAR, true);

                //�v���C���[������AnimatorBool��false��
                _animator.SetBool(ANIM_PLAYER_TALK, false);
            }

            //�䎌�ԍ����{�X�̖��O�\���^�C�~���O�Ɠ�����
            if (_lineNumber == _nameAppearanceNumber)
            {
                //����

                //���O�\��AnimatorTrigger���N��
                _animator.SetTrigger(ANIM_BOSS_NAME);
            }

            //���݂̉�b���I�����t���O��true��
            _isCurrentTalkEnd = true;

        }
        else
        {
            //�����Ă��Ȃ�

            //�b�҂��v���C���[��?
            if ((int)_speakers[_lineNumber] == 0)
            {
                //�v���C���[

                //�v���C���[����AnimatorBool��true��
                _animator.SetBool(ANIM_PLAYER_TALK, true);

                //�{�X����AnimatorBool��false��
                _animator.SetBool(ANIM_BOSS_TALK, false);
            }
            else
            {
                //�{�X

                //�v���C���[����AnimatorBool��false��
                _animator.SetBool(ANIM_PLAYER_TALK, false);

                //�{�X����AnimatorBool��true��
                _animator.SetBool(ANIM_BOSS_TALK, true);
            }
        }

        //���݂̃Z���t�ԍ��ɑΉ������Z���t��\��
        _talkText.text = _lines[_lineNumber];
        
        //���݂̃Z���t�ԍ��ɑΉ����������G��\��
        _characters[(int)_speakers[_lineNumber]].sprite = _emotions[_lineNumber];
    }

    /// <summary>
    /// <para>EndTalk</para>
    /// <para>Animation Event�p�B ��b�I�����ɃA�j���[�V�����ɍ��킹�ăt���O��������</para>
    /// </summary>
    public void EndTalk()
    {
        _isCurrentTalkEnd = true;

        _isTalkEnd = true;
    }

}
