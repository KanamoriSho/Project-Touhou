using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField, Label("�L�������[�u�f�[�^")]
    private PlayerMoveData _playerMoveData = default;

    [SerializeField, Label("�V���b�g�p�v�[��")]
    private GameObject[] _shotPools = default;

    [SerializeField, Label("�{���G�t�F�N�g�̃I�u�W�F�N�g")]
    private GameObject _bombShockWave = default;

    private Transform _playerTransform = default;                   //���g��Transform�i�[�p

    private Animator _animator = default;                           //���g��Animtor�i�[�p

    private AudioSource _audioSource = default;                     //���g��AudioSource�i�[�p

    private CollisionManager _collisionManger = default;            //���g��CollisionManager�i�[�p

    private Animator _bombAnimator = default;                       //�{���G�t�F�N�g��Animator�i�[�p
    [SerializeField]
    private int _currentHP = default;                               //���݂�HP�i�[�p

    private int _currentShotNumber = 0;                             //���݂̔��˂���e�̔ԍ����i�[����ϐ�

    private int _currentShotCount = 0;                              //���񂻂̒e�������������i�[����ϐ�

    private int _maxShotCount = 0;                                  //���̒e�������������i�[����ϐ�

    private int _currentPelletCount = 0;                            //���˂���e�̌��݂̐��������i�[����ϐ�

    private int _maxPelletCount = 0;                                //���˂���e�̓������������i�[����ϐ�

    private float _multiShotOffsetAngle = default;                  //���������ɔ��˂���ꍇ�̔��ˊp���i�[����ϐ�

    private float _offsetAngle = default;                           //���������ɔ��˂���ꍇ�̔��ˊp���i�[����ϐ�

    private bool _isTalking = false;                                //��b���t���O
    [SerializeField]
    private bool _isShotInterval = false;                           //�ˌ��C���^�[�o��������t���O

    private bool _isInvincible = false;                             //���G�t���O

    private bool _isBombCoolTime = false;                           //�{���g�p��N�[���^�C������t���O

    private bool _isDead = false;                                   //���S����p�t���O

    PlayerMoveData.ShotPatern _currentShotPatern = default;         //�e�̌��������i�[����Enum

    private Vector2 _startPosition = new Vector2(0, -3);            //�������W

    private Vector2 _targetingPosition = default;                   //�_���Ă�����W�i�[�p(���ˊp�v�Z�p)

    private WaitForSeconds _shotInterval = default;                 //�e�̘A�ˑ��x���Ǘ�����R���[�`���̃C���^�[�o��

    private WaitForSeconds _invincibleTime = default;               //���G���Ԃ��Ǘ�����R���[�`���̃C���^�[�o��

    private WaitForSeconds _disableEnemyShotsInterval = default;    //�{���g�p����e��������܂ł̃C���^�[�o��

    private WaitForSeconds _bombInterval = default;                 //�{���g�p����e��������܂ł̃C���^�[�o��

    #region Getter Setter

    public int GetCurrentShotCount
    {
        //_currentShotCount��Ԃ�
        get { return _currentShotCount; }
    }

    public int GetMaxShotCount
    {
        //_maxShotCount��Ԃ�
        get { return _maxShotCount; }
    }

    public int GetCurrentPelletCount
    {
        //_currentPelletCount��Ԃ�
        get { return _currentPelletCount; }
    }

    public int GetMaxPelletCount
    {
        //_maxPelletCount��Ԃ�
        get { return _maxPelletCount; }
    }

    public bool GetIsPlayerDead
    {
        //_isDead(���S����)��Ԃ�
        get { return _isDead; }
    }

    public bool GetIsChengeSpeedPerShot
    {
        //���݂̒e�ɕt������_isChangeSpeedPerShot��Ԃ�
        get { return _playerMoveData._isChangeSpeedPerShot[_currentShotNumber]; }
    }


    public bool SetIsTalking
    {
        set { _isTalking = value; }
    }

    public bool GetIsInvincible
    {
        get { return _isInvincible; }
    }

    #endregion

    private const float SECOND = 1.0f;                              //��b�̒萔

    private const float BOMB_SHOT_DISABLE_TIME = 0.1f;              //�{�����G�e�𖳌�������܂ł̎���

    private const float SLOW_MOVING_RATE = 0.5f;                    //�X���[�ړ����̌�����

    private const string ANIMATION_BOOL_LEFT_MOVE = "LeftMoving";   //���ړ���Animator��Bool��

    private const string ANIMATION_BOOL_RIGHT_MOVE = "RightMoving"; //�E�ړ���Animator��Bool��

    private const string ANIMATION_BOOL_SIDE_MOVING = "SideMoving"; //���ړ���Animator��Bool��

    private void Awake()
    {
        //60FPS�ɐݒ�
        Application.targetFrameRate = 60;

        //���g��Animator�̎擾
        _animator = this.GetComponent<Animator>();

        //���g��Transform���擾
        _playerTransform = this.transform;

        //���g��AudioSource�̎擾
        _audioSource = this.GetComponent<AudioSource>();

        //���g��CollisionManager�R���|�[�l���g�̎擾
        _collisionManger = this.GetComponent<CollisionManager>();

        //�{���p�G�t�F�N�g�I�u�W�F�N�g�̎擾
        _bombShockWave = GameObject.FindGameObjectWithTag("BombEffect");

        //�{���p�G�t�F�N�g��Animator���擾
        _bombAnimator = _bombShockWave.GetComponent<Animator>();

        //�{���p�G�t�F�N�g�𖳌���
        _bombShockWave.SetActive(false);

        //HP�̐ݒ�
        _currentHP = _playerMoveData._maxHP;

        //�e���ˁ`���ˊԂ̑҂����Ԃ��L���b�V��
        _shotInterval = new WaitForSeconds(SECOND / (float)_playerMoveData._shotPerSeconds[_currentShotNumber]);

        //���G���Ԃ��L���b�V��
        _invincibleTime = new WaitForSeconds(_playerMoveData._afterHitInvincibleTime);

        //�G�e�������ҋ@���Ԃ̃L���b�V��
        _disableEnemyShotsInterval = new WaitForSeconds(BOMB_SHOT_DISABLE_TIME);

        //�{���̎g�p�C���^�[�o���̃L���b�V��
        _bombInterval = new WaitForSeconds(_playerMoveData._bombCoolTime);

        /*�e���v�[���ɐ�������
         * _characterMoveData._waves                   : �E�F�[�u��(�{�X�L�����ȊO��1)
         * _characterMoveData._initiallyGeneratedShots : ���������e��(�X�N���v�^�u���I�u�W�F�N�g����󂯎��)
         */

        //�g�p�e�̎�ޕ����[�v
        for (int shotNumber = 0; shotNumber < _playerMoveData._shots.Count; shotNumber++)
        {
            //�g�p�����e�𐶐����郋�[�v
            for (int shotCounter = 0; shotCounter < _playerMoveData._initiallyGeneratedShots; shotCounter++)
            {
                //�e�̐���
                GameObject newShot = Instantiate(_playerMoveData._shots[shotNumber], _shotPools[shotNumber].transform);

                //���������e��false�ɂ���
                newShot.SetActive(false);
            }
        }
    }

    void Update()
    {
        if(_isDead)
        {
            return;
        }

        //X���̓��͒l�i�[�p
        float movingXInput = default;

        //Y���̓��͒l�i�[�p
        float movingYInput = default;

        //_playerMoveData._speed : �v���C���[�̈ړ����x(�X�N���v�^�u���I�u�W�F�N�g����󂯎��)

        //�E����
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //X���̓��͒l�Ƀv���C���[�̑��x������      
            movingXInput = _playerMoveData._speed;

            //Animator�́u���ړ����vbool��false��
            _animator.SetBool(ANIMATION_BOOL_LEFT_MOVE, false);

            //Animator�́u�E�ړ����vbool��true��
            _animator.SetBool(ANIMATION_BOOL_RIGHT_MOVE, true);

            //Animator�́u���ړ����vbool��true��
            _animator.SetBool(ANIMATION_BOOL_SIDE_MOVING, true);
        }
        //������
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            //X���̓��͒l��(�v���C���[�̑��x * -1)������
            movingXInput = -_playerMoveData._speed;

            //Animator�́u���ړ����vbool��true��
            _animator.SetBool(ANIMATION_BOOL_LEFT_MOVE, true);

            //Animator�́u���ړ����vbool��false��
            _animator.SetBool(ANIMATION_BOOL_RIGHT_MOVE, false);

            //Animator�́u���ړ����vbool��true��
            _animator.SetBool(ANIMATION_BOOL_SIDE_MOVING, true);
        }
        //���E���͖���
        else
        {
            //X���̓��͒l��0��
            movingXInput = 0;

            //Animator�́u���ړ����vbool��false��
            _animator.SetBool(ANIMATION_BOOL_LEFT_MOVE, false);

            //Animator�́u���ړ����vbool��false��
            _animator.SetBool(ANIMATION_BOOL_RIGHT_MOVE, false);

            //Animator�́u���ړ����vbool��false��
            _animator.SetBool(ANIMATION_BOOL_SIDE_MOVING, false);
        }

        //�����
        if (Input.GetKey(KeyCode.UpArrow))
        {
            //Y���̓��͒l�Ƀv���C���[�̑��x������   
            movingYInput = _playerMoveData._speed;
        }
        //������
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            //Y���̓��͒l��(�v���C���[�̑��x * -1)������ 
            movingYInput = -_playerMoveData._speed;
        }

        //���V�t�g����
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //�ᑬ�ړ�

            //X���̈ړ����͒l��0.5�{����
            movingXInput = movingXInput * SLOW_MOVING_RATE;

            //Y���̈ړ����͒l��0.5�{����
            movingYInput = movingYInput * SLOW_MOVING_RATE;
        }

        //�ŏI�I�Ȉړ����͒l���ړ������ɑ���
        Move(movingXInput, movingYInput);

        //Z����
        if (Input.GetKey(KeyCode.Z) && !_isTalking)
        {
            //�e���ˏ���
            Shot();
        }

        //���G��Ԃ�?
        if (_isInvincible)
        {
            //���G

            //�����I��
            return;
        }

        //��e������?
        if (_collisionManger.GetSetHitFlag)
        {
            //����

            //��e�R���[�`�����J�n
            StartCoroutine(OnHit());

            //CollisionManager�̔�e�t���O��false��
            _collisionManger.GetSetHitFlag = false;
        }
    }

    /// <summary>
    /// <para>Shot</para>
    /// <para>�e�̔��ˏ����B ��ѕ��A�p�x����ݒ肷��</para>
    /// </summary>
    private void Shot()
    {
        //�C���^�[�o������
        if (_isShotInterval)
        {
            //�C���^�[�o����

            //�������Ȃ�
            return;
        }

        //���ˊp�̏�����
        _multiShotOffsetAngle = 0;

        //���݂̒e�̌��������i�[(enum)
        _currentShotPatern = _playerMoveData._shotPaterns[_currentShotNumber];

        //�i�[���������������Ƃɏ�������
        switch (_currentShotPatern)           //�e�̌�����
        {
            //�P������
            case PlayerMoveData.ShotPatern.OneShot:

                #region �P������
                //�e�̗L���� or ����
                EnableShot();

                #endregion

                break;

            //�P������������
            case PlayerMoveData.ShotPatern.AllAtOnce:

                #region ��������
                //���������e�����擾
                _maxPelletCount = _playerMoveData._pelletCountInShots[_currentShotNumber];

                //��x�ɐ�������e������郋�[�v
                for (int pelletCount = 0; pelletCount <= _maxPelletCount; pelletCount++)
                {
                    //���[�v�������݂̐����e���Ƃ��ēn��
                    _currentPelletCount = pelletCount;

                    //�e�̗L���� or ����
                    EnableShot();
                }

                #endregion

                break;

            //��`��������
            case PlayerMoveData.ShotPatern.MultipleShots:

                #region ��`��������

                //���������e�����擾
                _maxPelletCount = _playerMoveData._pelletCountInShots[_currentShotNumber];

                //�ő唭�ˊp
                float maxOffset = 0;

                //���݂̔��ˊp
                float currentAngle = 0;

                //�e�̎U�z�p���擾
                float formedAngle = _playerMoveData._multiShotFormedAngles[_currentShotNumber];

                Debug.Log("formedAngle :" + formedAngle);

                //��x�ɐ�������e������郋�[�v
                for (int pelletCount = 0; pelletCount < _maxPelletCount; pelletCount++)
                {
                    //���[�v�������݂̐����e���Ƃ��ēn��
                    _currentPelletCount = pelletCount;

                    //���e��?
                    if (pelletCount == 0)
                    {
                        //���e

                        //�U�z�p���琳�ʂ���ɂ����ő唭�ˊp���Z�o
                        maxOffset = formedAngle / 2;

                        //�ő唭�ˊp����
                        _multiShotOffsetAngle = -maxOffset;

                        //�e�ƒe�̊Ԃ̊p�x���Z�o
                        currentAngle = formedAngle / (_maxPelletCount - 1);
                    }
                    else
                    {
                        //2���ڈȍ~

                        //���e�Őݒ肵�����ˊp�ɉ��Z
                        _multiShotOffsetAngle = _multiShotOffsetAngle + currentAngle;
                    }

                    //�e�̗L���� or ����
                    EnableShot();
                }

                #endregion

                break;

            //���ˏ󔭎�
            case PlayerMoveData.ShotPatern.RadialShots:

                #region ���ˏ󔭎�

                //�V���b�g�`�V���b�g�Ԃ̊p�x�i�[�p
                float currentRadialAngle = 0;

                //���������e�����擾
                _maxPelletCount = _playerMoveData._pelletCountInShots[_currentShotNumber];

                //���������e�������[�v
                for (int pelletCount = 0; pelletCount < _maxPelletCount; pelletCount++)
                {
                    //���[�v�������݂̐����e���Ƃ��ēn��
                    _currentPelletCount = pelletCount;

                    if (pelletCount == 0)       //���e�̏ꍇ
                    {
                        //���炵�p�̏�����
                        _multiShotOffsetAngle = 0;

                        //�e�ƒe�̊Ԃ̊p�x���Z�o
                        currentRadialAngle = 360 / _maxPelletCount;
                    }
                    else
                    {
                        //�ŏ��ɐݒ肵�����ˊp�ɉ��Z
                        _multiShotOffsetAngle = _multiShotOffsetAngle + currentRadialAngle;
                    }

                    //�e�̗L���� or ����
                    EnableShot();
                }

                #endregion

                break;
        }

        //���݂̐����e���̏�����
        _currentPelletCount = 0;

        //�C���^�[�o������
        StartCoroutine(RateOfShot());

    }

    /// <summary>
    /// <para>EnableShot</para>
    /// <para>���˂���e�ɑΉ������v�[����T�����A���g�p�̒e������΂��̒e��L�����B������ΐV���Ƀv�[�����ɐ�������</para>
    /// </summary>
    private void EnableShot()
    {
        //�I�u�W�F�N�g�v�[�����ɖ��g�p�I�u�W�F�N�g���������{��
        foreach (Transform shot in _shotPools[_currentShotNumber].transform)
        {
            //���g�p�I�u�W�F�N�g����������
            if (!shot.gameObject.activeSelf)
            {
                //���g�p�I�u�W�F�N�g��������

                //�������e��L����
                shot.gameObject.SetActive(true);

                //�e��̔���
                CheckShotType(shot);

                //true�ɂ����e���v���C���[�̈ʒu�Ɉړ�
                shot.position = this.transform.position;

                //�������I��
                return;
            }
        }

        //�ȉ����g�p�I�u�W�F�N�g�����������ꍇ�V�����e�𐶐�

        //�擾�����e�I�u�W�F�N�g��Ή�����v�[���̎q�I�u�W�F�N�g�Ƃ��Đ���
        GameObject newShot = Instantiate(_playerMoveData._shots[_currentShotNumber], _shotPools[_currentShotNumber].transform);

        //�e��̔���
        CheckShotType(newShot.transform);

        //���������e���L�����N�^�[�̈ʒu�Ɉړ�
        newShot.transform.position = this.transform.position;
    }

    /// <summary>
    /// <para>RateOfShot</para>
    /// <para>�e���˂̃C���^�[�o���������s��</para>
    /// </summary>
    /// <returns>_shotInterval : �C���^�[�o������</returns>
    IEnumerator RateOfShot()
    {
        //�eSE�����邩?
        if (_playerMoveData._shotSoundEffect != null)
        {
            //����

            _audioSource.PlayOneShot(_playerMoveData._shotSoundEffect);     //�e����SE���Đ�
        }

        _isShotInterval = true;         //�e���ˑҋ@�t���O��true��

        yield return _shotInterval;     //�ҋ@���ԕ��ҋ@

        _isShotInterval = false;        //�e���ˑҋ@�t���O��true��

    }

    /// <summary>
    /// <para>CheckShotType</para>
    /// <para>�e�̎�ނ𔻒肷��B���@�_���t���O�������Ă���ꍇ�ɔ��ˊp�Ƀv���C���[�Ƃ̃x�N�g���p�����Z����</para>
    /// </summary>
    /// <param name="shot">Shot���\�b�h�ŗL����/�������ꂽ�e�B�I�u�W�F�N�g�v�[���T���̍ۂ�Transform�^�Ŏ擾���邽��Transform�^</param>
    private void CheckShotType(Transform shot)
    {
        if (!_playerMoveData._isTargetingEnemy[_currentShotNumber])
        {
            _targetingPosition = _playerTransform.position + Vector3.up;
        }
        else
        {
            //��ԋ߂��̓G���擾���ă^�[�Q�b�g�Ƃ��Ď擾���鏈��
        }

        Vector2 degree = _targetingPosition - (Vector2)this.transform.position;


        float radian = Mathf.Atan2(degree.y, degree.x);

        shot.GetComponent<ShotMove>().GetSetShotAngle = radian * Mathf.Rad2Deg + _offsetAngle + _multiShotOffsetAngle;
    }

    /// <summary>
    /// <para>Move</para>
    /// <para>�ړ�����</para>
    /// </summary>
    /// <param name="horizontalInput">X������</param>
    /// <param name="verticalInput">Y������</param>
    private void Move(float horizontalInput, float verticalInput)
    {
        if (horizontalInput == 0 && verticalInput == 0)     //�ړ����͂������ꍇ
        {
            return;     //�������Ȃ�
        }

        Vector2 playerPosition = _playerTransform.position;      //�v���C���[�̌��݈ʒu���擾

        //���݈ʒu�Ɉړ����͂ƈړ��X�s�[�h�����Z����
        playerPosition = new Vector2(playerPosition.x + horizontalInput * Time.deltaTime,
                                        playerPosition.y + verticalInput * Time.deltaTime);

        /*
         *  if(�E or ��̈ړ������𒴂�����)
         *      �E or ��̈ړ��������ɖ߂�
         *  else if(�� or ���̈ړ������𒴂�����)
         *      �� or ���̈ړ��������ɖ߂�
         */

        //�E�̈ړ��͈͐����𒴂�����
        if (playerPosition.x > _playerMoveData._xLimitOfMoving)
        {
            //������

            playerPosition.x = _playerMoveData._xLimitOfMoving;     //���݂̍��W��X���̈ړ�����l�ɖ߂�
        }
        //���̈ړ��͈͐����𒴂�����
        else if (playerPosition.x < -_playerMoveData._xLimitOfMoving)
        {
            //������

            playerPosition.x = -_playerMoveData._xLimitOfMoving;     //���݂̍��W��X����(�ړ�����l * -1)�ɖ߂�
        }

        //��̈ړ��͈͐����𒴂�����
        if (playerPosition.y > _playerMoveData._yLimitOfMoving)
        {
            //������

            playerPosition.y = _playerMoveData._yLimitOfMoving;     //���݂̍��W��Y���̈ړ�����l�ɖ߂�
        }
        //���̈ړ��͈͐����𒴂�����
        else if (playerPosition.y < -_playerMoveData._yLimitOfMoving)
        {
            //������

            playerPosition.y = -_playerMoveData._yLimitOfMoving;     //���݂̍��W��Y����(�ړ�����l * -1)�ɖ߂�
        }

        _playerTransform.position = playerPosition;         //�ړ��������W���v���C���[�ɔ��f����
    }


    /// <summary>
    /// <para>Bomb</para>
    /// <para>�{���g�p������(��e���ɂ��Ă�)</para>
    /// </summary>
    /// <returns></returns>
    IEnumerator Bomb()
    {
        if(_isBombCoolTime)
        {
            yield break;
        }

        _bombShockWave.SetActive(true);                                     //�{���G�t�F�N�g��L����

        _bombShockWave.transform.position = _playerTransform.position;      //�{���G�t�F�N�g���v���C���[�̍��W�Ɉړ�

        _bombAnimator.SetTrigger("Enable");                                 //�{���G�t�F�N�gAnimator�̋N���g���K�[���I����

        yield return _disableEnemyShotsInterval;                            //�{����������G�̒e��������܂ł̃C���^�[�o���ҋ@

        GameObject[] enemyShotsInPicture = GameObject.FindGameObjectsWithTag("EnemyShot");      //���ݗL��������Ă���G�̒e��S�Ď擾

        StartCoroutine(BombCoolTime());

        //��قǎ擾�����G�̒e�̐��������[�v����
        for(int shotCount = 0; shotCount < enemyShotsInPicture.Length; shotCount++)
        {
            enemyShotsInPicture[shotCount].GetComponent<Animator>().SetTrigger("Disable");      //Animator�́u�������v�g���K�[���I����
        }
    }

    IEnumerator BombCoolTime()
    {
        _isBombCoolTime = true;

        yield return _bombInterval;

        _isBombCoolTime = false;
    }

    /// <summary>
    /// <para>OnHit</para>
    /// <para>�����蔻�� CollisionManager����ł�_isHit�t���O��true�ŌĂяo��</para>
    /// </summary>
    IEnumerator OnHit()
    {
        _isInvincible = true;

        _currentHP--;

        if(_currentHP <= 0)
        {
            _isDead = true;
        }

        _audioSource.PlayOneShot(_playerMoveData._hitSoundEffect);

        _animator.SetTrigger("Hit");

        StartCoroutine(Bomb());

        yield return _invincibleTime;

        _isInvincible = false;


    }

    /// <summary>
    /// <para>Interval</para>
    /// <para>�C���^�[�o�������p�̃R���[�`��</para>
    /// </summary>
    /// <param name="intervalTime">�ҋ@���Ԃ��L���b�V������WaitForSeconds</param>
    /// <returns>intervalTime �w��b���ҋ@���ĕԂ�</returns>
    IEnumerator Interval(WaitForSeconds intervalTime)
    {
        yield return intervalTime;
    }

    public void PositionReset()
    {
        _playerTransform.position = _startPosition;
    }
}
