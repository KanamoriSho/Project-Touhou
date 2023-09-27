using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacterMove : MonoBehaviour
{
    [SerializeField, Label("�L�������[�u�f�[�^")]
    private CharacterMoveData _characterMoveData = default;

    [SerializeField, Label("�V���b�g�p�v�[��")]
    private GameObject[] _shotPools = default;

    [SerializeField, Label("�`�F�b�N�|�C���g")]
    private List<Vector2> _moveCheckpoints = new List<Vector2>();       //�ړ��p�`�F�b�N�|�C���g�̍��W���i�[����

    [SerializeField, Label("�ҋ@����")]
    private List<float> _intervalBetweenMoves = new List<float>();      //�ړ��`�ړ��ԑҋ@���Ԃ��i�[���郊�X�g

    private CharactorShootingData _currentMovementAndShootingPaterns = default; 

    private CollisionManager _collisionManger = default;                //���g��CollisionManager�i�[�p

    private float _timer = 0;                                           //���Ԍv���p�ϐ�
    [SerializeField]
    private int _currentHP = default;                                   //���݂�HP�i�[�p

    private int _currentLife = default;                                 //���݂̎c�@�i�[�p

    private int _waveCount = 0;                                         //�E�F�[�u�����i�[����ϐ�
    
    private int _currentShotNumber = 0;                                 //���݂̔��˂���e�̔ԍ����i�[����ϐ�

    private int _currentShotCount = 0;                                  //�������̒e�������������i�[����ϐ�

    private int _maxShotCount = 0;                                      //���̒e�������������i�[����ϐ�

    private int _currentPelletCount = 0;                                //���˂���e�̌��݂̐��������i�[����ϐ�

    private int _maxPelletCount = 0;                                    //���˂���e�̓������������i�[����ϐ�

    private int _checkpointCounter = 0;                                 //���݂̈ړ��`�F�b�N�|�C���g�̔ԍ����i�[����

    private int _nextCheckpointNumber = 0;                              //���Ɍ������`�F�b�N�|�C���g�̔ԍ����i�[����

    private float _multiShotOffsetAngle = default;                      //���������ɔ��˂���ꍇ�̔��ˊp���i�[����ϐ�

    private float _swingShotOffsetAngle = default;                      //��]����������ۂ̉��Z�p���i�[����ϐ�

    private Vector2 _targetingPosition = default;                       //�_���Ă�����W�i�[�p(���ˊp�v�Z�p)

    private Vector2 _movingOffset = new Vector2(0, 0);                  //�`�F�b�N�|�C���g����ǂꂾ�����炵�Ĉړ������邩(����ړ����p)

    CharactorShootingData.ShotPatern _currentShotPatern = default;      //�e�̌��������i�[����Enum

    private WaitForSeconds _movingInterval = default;                   //�ړ����̃R���[�`���̃L���b�V��

    private WaitForSeconds _shotInterval = default;                     //�e�̘A�ˑ��x���Ǘ�����R���[�`���̃L���b�V��

    private const float SECOND = 1.0f;                                  //��b�̒萔

    private bool _isTalking = false;                                    //��b���t���O
    
    private bool _isMovingInterval = false;                             //�ړ��ҋ@������t���O
    
    private bool _isShotInterval = false;                               //���˃C���^�[�o��������t���O
    
    private bool _isNotShotInThisCheckpoint = false;                    //���̃`�F�b�N�|�C���g�Ŕ��ˏ����𖳎����邩

    private bool _isArrived = false;                                    //�`�F�b�N�|�C���g�ɓ���������
    
    private bool _isShotInSameTime = false;                             //���̒e�Ɠ����Ɍ����̃t���O�i�[�p

    private bool _isDead = false;                                       //���S����p�t���O

    private const string PLAYER_TAG = "Player";                         //�v���C���[�̃^�O���i�[����萔

    private GameObject _player = default;                               //�v���C���[�i�[�p

    private Animator _animator = default;                               //���g��Animtor�i�[�p

    private AudioSource audioSource = default;                          //���g��AudioSource�i�[�p

    BezierCurve _bezierCurve = default;                                 //�x�W�F�Ȑ������N���X�̃C���X�^���X�擾�p

    BezierCurveParameters _bezierCurveParameters = default;             //�x�W�F�Ȑ������̈����i�[�p�\���̂̃C���X�^���X�擾�p

    #region Getter, Setter

    public int GetCurrentShotCount
    {
        //_currentPelletCount��Ԃ�
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

    public int GetMaxHP
    {
        //_characterMoveData��_maxHP��Ԃ�
        get { return _characterMoveData._maxHP; }
    }

    public int GetCurrentHP
    {
        //_currentHP��Ԃ�
        get { return _currentHP; }
    }

    public bool GetIsDead
    {
        //_isDead(���S����)��Ԃ�
        get { return _isDead; }
    }

    public bool GetIsChangeSpeedPerShot
    {
        //CharactorMoveData�̔��˂��Ƃɏ����������������邩�̃t���O��Ԃ�(ShotMove�Ɏ󂯓n��)
        get { return _currentMovementAndShootingPaterns._isChangeSpeedPerShoot[_currentShotNumber]; }
    }


    public bool SetIsTalking
    {
        set { _isTalking = value; }
    }

    #endregion

    //�ȉ��̕ϐ���OnDrowGizmos�Ɏ󂯓n�����߂Ƀt�B�[���h�ϐ��ɂ��Ă܂��B

    private Vector2 _fixedRelayPoint = default;         //�x�W�F�Ȑ��̒��ԓ_�i�[�p

    private Vector2 _relayPointY = default;             //_relayPointVector��̏c(Y)�����W�i�[�p

    private void Awake()
    {

        //Animator�̎擾
        _animator = this.gameObject.GetComponent<Animator>();

        //�v���C���[�L�����̎擾
        _player = GameObject.FindGameObjectWithTag(PLAYER_TAG);

        //���g��CollisionManager�R���|�[�l���g�̎擾
        _collisionManger = this.GetComponent<CollisionManager>();

        //HP�̐ݒ�
        _currentHP = _characterMoveData._maxHP;

        _currentLife = _characterMoveData._maxLife;

        //���݂̍s���p�^�[�����擾
        _currentMovementAndShootingPaterns = _characterMoveData._movementAndShootingPaterns[_waveCount];

        //��b���t���O
        _isTalking = false;

        //�ړ��ҋ@������t���O
        _isMovingInterval = false;

        //���˃C���^�[�o��������t���O
        _isShotInterval = false;
        
        //���̃`�F�b�N�|�C���g�Ŕ��ˏ����𖳎����邩
        _isNotShotInThisCheckpoint = false;

        //�`�F�b�N�|�C���g�ɓ���������
        _isArrived = false;

        //���̒e�Ɠ����Ɍ����̃t���O�i�[�p
        _isShotInSameTime = false;
        
        //���S����p�t���O
        _isDead = false;

        /*�e���v�[���ɐ�������
         * _characterMoveData._waves                   : �E�F�[�u��(�{�X�L�����ȊO��1)
         * _characterMoveData._initiallyGeneratedShots : ���������e��(�X�N���v�^�u���I�u�W�F�N�g����󂯎��)
         */

        //�E�F�[�u�������[�v
        for (int waveCount = 0; waveCount < _characterMoveData._waveCount; waveCount++)
        {
            //���̃E�F�[�u�Ŏg�p����e��̐����i�[
            int _currentShotNumber = _characterMoveData._movementAndShootingPaterns[waveCount]._shots.Length;

            //�E�F�[�u���g�p�e�̎�ޕ����[�v
            for (int shotNumber = 0; shotNumber < _currentShotNumber; shotNumber++)
            {

                //�E�F�[�u���Ŏg�p�����e�𐶐����郋�[�v
                for (int shotLength = 0; shotLength < _characterMoveData._initiallyGeneratedShots; shotLength++)
                {
                    //�g�p����e��z�񂩂���o���i�[
                    GameObject currentShotObject =
                                _characterMoveData._shots[_characterMoveData._movementAndShootingPaterns[waveCount]._shots[shotNumber] - 1];

                    //�e�̐���
                    GameObject newShot = Instantiate(currentShotObject, _shotPools[shotNumber].transform);

                    //���������e��false�ɂ���
                    newShot.SetActive(false);
                }
            }
        }
    }

    private void OnEnable()
    {
        _bezierCurve = new BezierCurve();

        _bezierCurveParameters = new BezierCurveParameters();
    }

    void Update()
    {
        if(_isDead || _isTalking)
        {
            return;
        }

        //���Ԃ����Z
        _timer += Time.deltaTime;

        //���̈ړ���`�F�b�N�|�C���g���w��ł��Ă��邩
        if (_nextCheckpointNumber != _checkpointCounter + 1 || _nextCheckpointNumber != 0)
        {
            //�ł��Ă��Ȃ�

            //���݂̃`�F�b�N�|�C���g + 1�����̃`�F�b�N�|�C���g�ԍ��Ƃ��Ċi�[
            _nextCheckpointNumber = GetNextArrayNumber(_checkpointCounter, _moveCheckpoints.Count);

            //�s���p�^�[���̍X�V
            _currentMovementAndShootingPaterns = _characterMoveData._movementAndShootingPaterns[_waveCount];
        }

        //��e������?
        if (_collisionManger.GetSetHitFlag)
        {
            //����

            //��e�R���[�`�����J�n
            OnHit();

            //CollisionManager�̔�e�t���O��false��
            _collisionManger.GetSetHitFlag = false;
        }

        //�ړ����ɒe�������ۂ��̃t���O���i�[
        bool isShotOnTheMove = _currentMovementAndShootingPaterns._isMovingShooting[_currentShotNumber];

        /*���݂̒e�̌��^�C�~���O��
         * 
         * ���� : �ړ����Ȃ��猂��    �� : �~�܂��Č���
         * 
         * ���̔�����Ƃ�B
         */
        bool isCurrentShotMach = CheckCurrentAndNextShotType(isShotOnTheMove);

        //���݂̍��W�����`�F�b�N�|�C���g�Ɠ�����
        if (this.transform.position == (Vector3)_moveCheckpoints[_nextCheckpointNumber] && !_isArrived)
        {
            //�ړ��A��~�̔��˃p�^�[����
            if (isCurrentShotMach)
            {
                //true

                //���̃`�F�b�N�|�C���g�ł͔��ˏ��������Ȃ�
                _isNotShotInThisCheckpoint = true;
            }
            else
            {
                //false

                //���̃`�F�b�N�|�C���g�ł͔��ˏ������s��
                _isNotShotInThisCheckpoint = false;
            }

            //�ړ��Ԃ̃C���^�[�o�����L���b�V��
            _movingInterval = new WaitForSeconds(_intervalBetweenMoves[_checkpointCounter]);

            //���݂̃`�F�b�N�|�C���g����������
            _checkpointCounter = _nextCheckpointNumber;

            //���̒e�Ɠ����Ɍ����̃t���O���擾
            _isShotInSameTime = _currentMovementAndShootingPaterns._isShotInSameTime[_currentShotNumber];

            if (!_isShotInSameTime)
            {
                //���˂���e�̒e�ԍ��̕ύX�A���ː��̏�����
                SetShotNumber();
            }

            //�ړ��`�ړ��Ԃ̑ҋ@�R���[�`��

            //�ҋ@���ԕ��ҋ@
            StartCoroutine(MovementInterval());

            _isArrived = true;
        }

        //�ړ��Ԃ̑ҋ@���Ȃ�
        if (_isMovingInterval)
        {
            //���̃`�F�b�N�|�C���g�ł͔��ˏ��������Ȃ���
            if (_isNotShotInThisCheckpoint)
            {
                //���Ȃ�

                return;
            }

            //�ړ����Ɍ��t���O��false?
            if (!isShotOnTheMove)
            {
                //false

                //�e�Ɏ󂯓n���p�����[�^�̐ݒ�E����
                SettingShotPrameters();
            }

            return;
        }

        //�ړ����Ɍ��t���O��true?
        if (isShotOnTheMove)
        {
            //true

            //�e�Ɏ󂯓n���p�����[�^�̐ݒ�E����
            SettingShotPrameters();
        }

        //�ړ����ɋȐ��I�ɔ��?
        if (!_characterMoveData._isCurveMoving)
        {
            //false

            //���݈ʒu
            Vector2 currentPosition = _moveCheckpoints[_checkpointCounter];

            //�ړ���̖ڕW���W
            Vector2 nextPosition = _moveCheckpoints[_nextCheckpointNumber];

            /* �ړ����x�̌v�Z
             * �ړ����x * �ړ����x�p�A�j���[�V�����J�[�u�̒l
             */
            float movingSpeed = _characterMoveData._speed * _characterMoveData._speedCurve.Evaluate(_timer);

            /* Lerp�Ń`�F�b�N�|�C���g�Ԃ��ړ�
             * �ґ��ړ��p�I�t�Z�b�g�l�����Z����(�P�̔�s�̏ꍇ��+-0)
             */
            this.transform.position = Vector2.Lerp(currentPosition, nextPosition, movingSpeed) + _movingOffset;
        }
        else
        {
            //true


            //�x�W�F�Ȑ��̎Z�o�ɕK�v�ȃp�����[�^�ނ��\���̂Ɏ󂯓n��

            //�J�n�n�_
            _bezierCurveParameters.StartingPosition = _moveCheckpoints[_checkpointCounter];
            //�I�_
            _bezierCurveParameters.TargetPosition = _moveCheckpoints[_nextCheckpointNumber];
            //�o�ߎ��Ԍv���^�C�}�[
            _bezierCurveParameters.Timer = _timer;
            //���ԓ_X���W
            _bezierCurveParameters.CurveMoveVerticalOffset = _characterMoveData._curveMoveVerticalOffset;
            //���ԓ_Y���W
            _bezierCurveParameters.CurveMoveHorizontalOffset = _characterMoveData._curveMoveHorizontalOffset;
            //�f�o�b�O���[�h�t���O
            _bezierCurveParameters.IsDebugMode = _characterMoveData._isDebug;

            /* �x�W�F�Ȑ�����o�������W�����ݒn�_��
             * �ґ��ړ��p�I�t�Z�b�g�l�����Z����(�P�̔�s�̏ꍇ��+-0)
             */
            this.transform.position = _bezierCurve.CalculateBezierCurve(_bezierCurveParameters) + _movingOffset;
        }
    }

    #region �ړ��֘A���\�b�h

    /// <summary>
    /// <para>CheckCurrentAndNextShotType</para>
    /// <para>���݂̒e�̌��^�C�~���O(�ړ����Ȃ��炩�~�܂��Ă�)�Ǝ��̒e�̌��^�C�~���O���r���A�قȂ��true��Ԃ�����</para>
    /// </summary>
    /// <param name="isShotOnTheMove">���݂̒e�̌��^�C�~���O (true : �ړ����Ȃ��猂�� false : �~�܂��Č���)</param>
    /// <returns>if(isShotOnTheMove != isNextShotOnTheMove) �̌��ʂ�Ԃ�</returns>
    private bool CheckCurrentAndNextShotType(bool isShotOnTheMove)
    {
        //���ʗp�t���O���`
        bool isChangeMoveShotToNextShot = default;

        //���Ɍ��e�̒e�ԍ����`
        int nextShotNumber = GetNextArrayNumber(_currentShotNumber, _currentMovementAndShootingPaterns._isMovingShooting.Length);

        //���̒e���ړ����Ɍ����ۂ��̃t���O���i�[
        bool isNextShotOnTheMove = _currentMovementAndShootingPaterns._isMovingShooting[nextShotNumber];

        //���݂̒e�Ǝ��̒e�̌��^�C�~���O���قȂ邩
        if (isShotOnTheMove && !isNextShotOnTheMove)
        {
            //�قȂ�

            //���ʗp�t���O��true���i�[
            isChangeMoveShotToNextShot = true;
        }
        else
        {
            //����

            //���ʗp�t���O��false���i�[
            isChangeMoveShotToNextShot = false;
        }

        //���ʂ�Ԃ�
        return isChangeMoveShotToNextShot;
    }

    #endregion

    #region �e�֘A���\�b�h

    /// <summary>
    /// <para>SetShotNumber</para>
    /// <para>���˂���e�̒e�ԍ��̕ύX�A���ː��̏��������s��</para>
    /// </summary>
    private void SetShotNumber()
    {
        //���˃C���^�[�o�����t���O��false��
        _isShotInterval = false;

        //���˂���e�̔z��Q�Ɣԍ���ύX
        _currentShotNumber = GetNextArrayNumber(_currentShotNumber, _currentMovementAndShootingPaterns._shots.Length);

        //���ˉ񐔂�0�ɏ�����
        _currentShotCount = 0;
    }

    /// <summary>
    /// <para>SettingShotPrameters</para>
    /// <para>���˂���e�̃p�����[�^�����ƂɘA�ˑ��x�┭�ː����Q�Ƃ��Ĕ��ˏ����Ƃ��̒�~���s��</para>
    /// </summary>
    private void SettingShotPrameters()
    {
        //�C���^�[�o������
        if (_isShotInterval)
        {
            //�C���^�[�o����

            //�������Ȃ�
            return;
        }

        //�e�̍ő唭�ː����i�[
        _maxShotCount = _currentMovementAndShootingPaterns._shotCounts[_currentShotNumber];

        //���݂̔��ː����ő唭�ː����z���Ă��Ȃ���
        if (_currentShotCount <= _maxShotCount)
        {
            //�z���Ă��Ȃ�

            //�b�Ԃɉ����������i�[
            int shotPerSeconds = _currentMovementAndShootingPaterns._shotPerSeconds[_currentShotNumber] + 1;

            //�V���b�g�`�V���b�g�Ԃ̑ҋ@���Ԃ�ݒ�
            _shotInterval = new WaitForSeconds(SECOND / shotPerSeconds);

            //�e���ˏ���
            Shot();
        }
        else
        {
            //�z����

            //�����Ɍ���?
            if (_isShotInSameTime)
            {
                //�����Ɍ���

                //���˂���e�̒e�ԍ��̕ύX�A���ː��̏�����
                SetShotNumber();

                //���̒e�Ɠ����Ɍ����̃t���O���擾
                _isShotInSameTime = _currentMovementAndShootingPaterns._isShotInSameTime[_currentShotNumber];

                //���̃`�F�b�N�|�C���g�Ō����̔��ʃt���O��������
                _isNotShotInThisCheckpoint = false;
            }
        }
    }

    /// <summary>
    /// <para>Shot</para>
    /// <para>�e�̔��ˏ����B ��ѕ��A�p�x����ݒ肷��</para>
    /// </summary>
    private void Shot()
    {

        //���ˊp�̏�����
        _multiShotOffsetAngle = 0;

        //��]�����̗L���̔���Ɗp�x�v�Z
        SwingShotCheck();

        //���݂̒e�̌��������i�[(enum)
        _currentShotPatern = _currentMovementAndShootingPaterns._shotPaterns[_currentShotNumber];

        //�i�[���������������Ƃɏ�������
        switch (_currentShotPatern)           //�e�̌�����
        {
            //�P������
            case CharactorShootingData.ShotPatern.OneShot:

                #region �P������
                //�e�̗L���� or ����
                EnableShot();

                #endregion

                break;

            //�P������������
            case CharactorShootingData.ShotPatern.AllAtOnce:

                #region ��������

                //���������e�����擾
                _maxPelletCount = _currentMovementAndShootingPaterns._pelletCountInShots[_currentShotNumber];

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
            case CharactorShootingData.ShotPatern.MultipleShots:

                #region ��`��������

                //���������e�����擾
                _maxPelletCount = _currentMovementAndShootingPaterns._pelletCountInShots[_currentShotNumber];

                //�ő唭�ˊp
                float maxOffset = 0;

                //���݂̔��ˊp
                float currentAngle = 0;

                //�e�̎U�z�p���擾
                float formedAngle = _currentMovementAndShootingPaterns._multiShotFormedAngles[_currentShotNumber];

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
            case CharactorShootingData.ShotPatern.RadialShots:

                #region ���ˏ󔭎�

                //�V���b�g�`�V���b�g�Ԃ̊p�x�i�[�p
                float currentRadialAngle = 0;

                //���������e�����擾
                _maxPelletCount = _currentMovementAndShootingPaterns._pelletCountInShots[_currentShotNumber];

                //���������e�������[�v
                for (int pelletCount = 0; pelletCount < _maxPelletCount; pelletCount++)
                {
                    //���[�v�������݂̐����e���Ƃ��ēn��
                    _currentPelletCount = pelletCount;

                    //���e��?
                    if (pelletCount == 0)
                    {
                        //���e

                        //���炵�p�̏�����
                        _multiShotOffsetAngle = 0;

                        //�e�ƒe�̊Ԃ̊p�x���Z�o
                        currentRadialAngle = 360 / (_maxPelletCount - 1);
                    }
                    else
                    {
                        //2���ڈȍ~

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

        //�������e�������Z
        _currentShotCount++;

        //�C���^�[�o������
        StartCoroutine(RateOfShot());

    }

    /// <summary>
    /// <para>SwingShotCheck</para>
    /// <para>��]����(��������?)���s�����̔���ƁA�s���ꍇ�̊p�x�v�Z���s��</para>
    /// </summary>
    private void SwingShotCheck()
    {
        //��]���������邩�̃t���O���擾
        bool isSwingShot = _currentMovementAndShootingPaterns._isSwingShots[_currentShotNumber];

        //��]��������?
        if (isSwingShot)
        {
            //����

            //��]�������ɉ񂷊p�x�̎擾
            float centralAngle = _currentMovementAndShootingPaterns._swingShotFormedAngles[_currentShotNumber];

            //��]�������̏��e�̊p�x�̎擾
            float firstAngle = _currentMovementAndShootingPaterns._swingShotFirstAngles[_currentShotNumber];

            //�P�ʊp���Z�o
            float radian = centralAngle / _maxShotCount;


            //���e��?
            if (_currentShotCount <= 0)
            {
                //���e

                //���ˊp�ɏ��e�̊p�x��ݒ�
                _swingShotOffsetAngle = firstAngle;
            }
            else
            {
                //���ˊp�ɒP�ʊp�����Z
                _swingShotOffsetAngle += radian;
            }
        }
        else
        {
            //���Ȃ�

            //�p�x��������
            _swingShotOffsetAngle = 0;
        }
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

        //�V���ɐ�������e�̒e�ԍ����擾(�e�ԍ��Ɣz��v�f���̍����C�����邽�ߎ擾�l -1���i�[)
        int shotNumber = _currentMovementAndShootingPaterns._shots[_currentShotNumber] - 1;

        //�V���ɔ��˂���e�̃I�u�W�F�N�g���擾
        GameObject shotObject = _characterMoveData._shots[shotNumber];

        //�擾�����e�I�u�W�F�N�g��Ή�����v�[���̎q�I�u�W�F�N�g�Ƃ��Đ���
        GameObject newShot = Instantiate(shotObject, _shotPools[_currentShotNumber].transform);

        //�e��̔���
        CheckShotType(newShot.transform);

        //���������e���L�����N�^�[�̈ʒu�Ɉړ�
        newShot.transform.position = this.transform.position;
    }

    /// <summary>
    /// <para>RateOfShot</para>
    /// <para>�e���˂̃C���^�[�o���������s��</para>
    /// </summary>
    private IEnumerator RateOfShot()
    {
        //����SE�����邩
        if (_characterMoveData._shotSoundEffect != null)
        {
            //����

            //����SE���Đ�
            audioSource.PlayOneShot(_characterMoveData._shotSoundEffect);
        }

        //���˃C���^�[�o�����t���O��true��
        _isShotInterval = true;

        //���ˊԃC���^�[�o������
        yield return _shotInterval;

        //���˃C���^�[�o�����t���O��false��
        _isShotInterval = false;
    }

    /// <summary>
    /// <para>CheckShotType</para>
    /// <para>�e�̎�ނ𔻒肷��B���@�_���t���O�������Ă���ꍇ�ɔ��ˊp�Ƀv���C���[�Ƃ̃x�N�g���p�����Z����</para>
    /// </summary>
    /// <param name="shot">Shot���\�b�h�ŗL����/�������ꂽ�e�B�I�u�W�F�N�g�v�[���T���̍ۂ�Transform�^�Ŏ擾���邽��Transform�^</param>
    private void CheckShotType(Transform shot)
    {
        //���@�_���e���ۂ����i�[����t���O
        bool isTargetingPlayer = _currentMovementAndShootingPaterns._isTargetingPlayer[_currentShotNumber];

        //���@�_���e��
        if (!isTargetingPlayer)
        {
            //�ʏ�e

            //�^�����^�[�Q�b�g���W��
            _targetingPosition = Vector2.down;
        }
        else
        {
            //���@�_��

            //���������e�����̏��e��
            if (_currentShotPatern != CharactorShootingData.ShotPatern.OneShot && _currentPelletCount <= 0)
            {
                //�^�[�Q�b�g�Ƃ��Ă��̏u�Ԃ̃v���C���[�̍��W���i�[
                _targetingPosition = _player.transform.position;
            }
            else
            {
                //���������e�ł͂Ȃ��ꍇ�͏�Ƀv���C���[�̍��W���^�[�Q�b�g���W�Ƃ��Ċi�[����

                //�^�[�Q�b�g�Ƃ��Ă��̏u�Ԃ̃v���C���[�̍��W���i�[
                _targetingPosition = _player.transform.position;
            }

        }

        /*
         * _targetingPosition �͎��@�_�����ۂ��œ�����̂��ς��܂�
         * 
         * �ʏ�e     : �L�����N�^�[�̐���(Vector2.down)
         * 
         * ���@�_���e : ���e���ˎ��̃v���C���[�̍��W
         */

        //���݂̍��W�ƃ^�[�Q�b�g�Ƃ��Ċi�[������W�Ԃ̃x�N�g�������߂�
        Vector2 degree = _targetingPosition - (Vector2)this.transform.position;

        //�x�N�g������p�x�ɕϊ�
        float radian = Mathf.Atan2(degree.y, degree.x);

        //���˂���e��ShotMove�R���|�[�l���g���擾
        ShotMove shotMove = shot.GetComponent<ShotMove>();

        /* ���˕����ɓ������ˎ��̉��Z�p(���ˏ�A���̏ꍇ)�Ɖ񂵊p(�񂵌����̏ꍇ)�����Z���Ēe�ɔ��ˊp�Ƃ��Ď󂯓n��
         * 
         * _multiShotOffsetAngle  : ��`�E�~�`�Ɍ��ۂɒe�`�e�Ԃ̌ʓx���i�[
         * _swingShotOffsetAngle  : �񂵌����̍ۂɒe�`�e�Ԃ̌Ǔx���i�[
         */
        shotMove.GetSetShotAngle = radian * Mathf.Rad2Deg + _multiShotOffsetAngle + _swingShotOffsetAngle;
    }

    #endregion

    /// <summary>
    /// <para>GetNextArrayNumber</para>
    /// <para>���̔z��ԍ����Z�o���郁�\�b�h</para>
    /// </summary>
    /// <param name="currentNumber">���݂̔z��Q�Ɣԍ�</param>
    /// <param name="arrayLength">���̔z��̗v�f��</param>
    /// <returns>nextNumber �Z�o�������̔z��ԍ�</returns>
    private int GetNextArrayNumber(int currentNumber, int arrayLength)
    {
        return (currentNumber + 1) % arrayLength;
    }

    /// <summary>
    /// <para>OnHit</para>
    /// <para>�����蔻�� CollisionManager����ł�_isHit�t���O��true�ŌĂяo��</para>
    /// </summary>
    private void OnHit()
    {
        //��e���Đ�
        //_audioSource.PlayOneShot(_characterMoveData._hitSoundEffect);

        //HP�̌��Z����
        _currentHP--;

        //HP��0�ȉ��ɂȂ�����
        if (_currentHP <= 0)
        {
            //HP���ő�l�ɖ߂�
            _currentHP = _characterMoveData._maxHP;

            //�c�@��1���炷
            _currentLife--;
        }

        //�c�@��0�ȉ���
        if(_currentLife <= 0)
        {
            //���S�t���O��true��
            _isDead = true;

            //���g�𖳌���
            this.gameObject.SetActive(false);
        }

    }

    /// <summary>
    /// <para>MovementInterval</para>
    /// <para>�ړ��̃C���^�[�o���������s��</para>
    /// </summary>
    private IEnumerator MovementInterval()
    {
        //�ړ��ҋ@���t���Otrue
        _isMovingInterval = true;

        //�ҋ@���ԕ��ҋ@
        yield return _movingInterval;

        //�ړ��ҋ@���t���Ofalse
        _isMovingInterval = false;

        //���̃`�F�b�N�|�C���g�Ō����̔��ʃt���O��������
        _isNotShotInThisCheckpoint = false;

        _isArrived = false;

        _timer = 0;
    }

}