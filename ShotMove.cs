using UnityEngine;

public class ShotMove : MonoBehaviour
{
    [Label("�V���b�g���[�u�f�[�^")]
    public ShotMoveData _shotMoveData = default;            //�e�O���̃X�N���v�^�u���I�u�W�F�N�g

    private SpriteRenderer _spriteRenderer = default;       //���g��SpriteRenderer�i�[�p

    private Animator _animator = default;                   //���g��Animator�i�[�p

    private EnemyCharacterMove _characterMove = default;    //�G�L�����N�^�[�̋����X�N���v�g�̊i�[�p

    private PlayerMove _playerMove = default;               //�v���C���[�̋����X�N���v�g�̊i�[�p

    private GameObject _shooter = default;                  //�ˎ�i�[�p

    private float _timer = default;                         //�o�ߎ��Ԍv���p

    private float _initialVelocity = default;               //�����l

    private float _speed = default;                         //���x�i�[�p

    private float _colliderRadius = default;                //�����蔻��̑傫�����i�[

    private float _shotAngle = default;                     //���ˊp�i�[�p

    private int _shotCounter = default;                     //���̒e�𔭎˂��������i�[����(CharactorData����󂯎��)

    private int _maxShotCount = default;                    //���̒e�̍ő唭�ː����i�[����(CharactorData����󂯎��)

    private int _pelletCounter = default;                   //���ݐ������ꂽ�e�̐����i�[����(CharactorData����󂯎��)

    private int _maxPelletCount = default;                  //�����ɐ�������e�̐����i�[����(CharactorData����󂯎��)

    private bool _isVelocityChangePerShot = false;          //���˂��Ƃɏ��������������邩���i�[����

    private Vector2 _shooterPosition = default;             //�ˎ��Position�i�[�p

    private Vector2 _shotVector = default;                  //�e�̔��˃x�N�g���i�[�p

    private Vector2 _targetPosition = default;              //_target��Position�i�[�p

    BezierCurve _bezierCurve = default;                     //�x�W�F�Ȑ������N���X�̃C���X�^���X�擾�p

    BezierCurveParameters _bezierCurveParameters = default; //�x�W�F�Ȑ������̈����i�[�p�\���̂̃C���X�^���X�擾�p

    private const string PLAYER_TAG = "Player";             //�v���C���[�̃^�O

    private const string BOSS_TAG = "Boss";                 //�{�X�̃^�O

    #region Getter, Setter

    public float GetColliderRadius
    {
        //_colliderRadius��Ԃ�
        get { return _colliderRadius; }
    }

    public GameObject GetSetshooter
    {
        //_shooter��Ԃ�
        get { return _shooter; }
        //�n���ꂽ�l��_shooter�Ɋi�[
        set { _shooter = value; }
    }

    public float GetSetShotAngle
    {
        //_shotAngle��Ԃ�
        get { return _shotAngle; }
        //�n���ꂽ�l��_shotAngle�ɐݒ�
        set
        {
            _shotAngle = value;
        }
    }

    #endregion

    private void Awake()
    {
        //�R���|�[�l���g���擾
        GetComponents();

        //�C���X�^���X�������N���X�̍쐬
        CreateInstances();

        //�ŏ��Ɉ�x�����Q�Ƃ���p�����[�^�̏����ݒ�
        SetupParameters();

        //�p�����[�^�̏����ݒ�
        ResetParameters();
    }

    private void OnEnable()
    {
        //����������
        ResetParameters();

        //�����̐ݒ�
        SetInitialVelocity();
    }

    private void Update()
    {
        _timer += Time.deltaTime;                        //���Ԃ̉��Z

        Vector2 currentPos = this.transform.position;   //���݂̎��g�̍��W���擾

        //���x�̌v�Z
        CalculateVelocity();

        //�v�Z���ʂ̍��W�����g�̍��W�ɂ���
        this.transform.position = CalculateMoving(currentPos);
    }

    /// <summary>
    /// <para>OnBecameInvisible</para>
    /// <para>��ʊO�ɏo���e����������</para>
    /// </summary>
    private void OnBecameInvisible()
    {
        //�e�̖���������
        ObjectDisabler();
    }

    /// <summary>
    /// <para>GetComponents</para>
    /// <para>�R���|�[�l���g�擾�p���\�b�h</para>
    /// </summary>
    private void GetComponents()
    {
        //SpriteRenderer�擾
        _spriteRenderer = this.GetComponent<SpriteRenderer>();

        //���g��Animator���擾
        _animator = this.GetComponent<Animator>();
    }

    /// <summary>
    /// <para>ResetParameters</para>
    /// <para>�ϐ��������p���\�b�h �f�o�b�O���ɂ��Ăяo����悤�Ƀ��\�b�h�����Ă܂�</para>
    /// </summary>
    private void ResetParameters()
    {
        //�o�ߎ��Ԃ����Z�b�g
        _timer = 0;

        //�ˎ�̍��W���Đݒ�
        _shooterPosition = _shooter.transform.position;

        //���g���ˎ�̍��W�Ɉړ�
        this.transform.position = _shooterPosition;

        //�X�v���C�g�ύX
        _spriteRenderer.sprite = _shotMoveData._shotSprite;

        //�����蔻��̑傫����_shotMoveData�̂��̂ƈقȂ邩
        if (this._colliderRadius != _shotMoveData._colliderRadius)
        {
            //�قȂ�

            //�����蔻��̃T�C�Y���Đݒ�
            this._colliderRadius = _shotMoveData._colliderRadius;
        }

        //�e�̉�]�t���O��true��
        if (_shotMoveData._isSpinning)
        {
            //true�Ȃ烉���_���ŉ�]������
            this.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }

        //�ˎ肪�v���C���[��
        if (_shotMoveData._shooterType != ShotMoveData.ShooterType.Player)
        {
            //�v���C���[�ł͂Ȃ�

            //�e�̃A�j���[�V������������
            _animator.SetTrigger("Enable");
        }

        //2�F�ڂ�����e��
        if (_shotMoveData._hasAlternativeColor)
        {
            //2�F�ڂ�����Ȃ�

            //���݉����ڂ̒e�����擾
            _shotCounter = _characterMove.GetCurrentShotCount;

            //�������
            if (_shotCounter % 2 == 1)
            {
                //��Ȃ�

                //�f�t�H���g�̃X�v���C�g��
                _animator.SetBool("AltColor", false);
            }
            else
            {
                //�����Ȃ�

                //2�F�ڂ̃X�v���C�g��
                _animator.SetBool("AltColor", true);
            }
        }

        //�������Ƃɉ���/�������Ȃ��e�Ȃ�
        if (_shotMoveData._shotVelocity == ShotMoveData.ShotVelocity.Nomal)
        {
            return;
        }

        //�ȉ��������Ƃɉ���������e�̏ꍇ

        //�v���C���[�ł͖���?
        if (_shotMoveData._shooterType != ShotMoveData.ShooterType.Player)
        {
            //�v���C���[�ł͂Ȃ�

            //���˂��ƂɌ������邩���擾
            _isVelocityChangePerShot = _characterMove.GetIsChangeSpeedPerShot;

            //���˂��Ƃɉ��������邩?
            if (!_isVelocityChangePerShot)
            {
                //���Ȃ�(�������Ƃɉ�����)

                //_characterMove���猻�݂̐����e�����󂯎��
                _pelletCounter = _characterMove.GetCurrentPelletCount;

                //_characterMove���瓯�������e�����󂯎��
                _maxPelletCount = _characterMove.GetMaxPelletCount;
            }
            else
            {
                //����

                //_characterMove���猻�݂̐����e�����󂯎��
                _shotCounter = _characterMove.GetCurrentShotCount;

                //_characterMove����ő唭�˒e�����󂯎��
                _maxShotCount = _characterMove.GetMaxShotCount;
            }
        }
        else
        {
            //�v���C���[�ł���

            //���˂��ƂɌ������邩���擾
            _isVelocityChangePerShot = _playerMove.GetIsChengeSpeedPerShot;

            //���˂��Ƃɉ��������邩?
            if (!_isVelocityChangePerShot)
            {
                //���Ȃ�(�������Ƃɉ�����)

                //_playerMove���猻�݂̐����e�����󂯎��
                _pelletCounter = _playerMove.GetCurrentPelletCount;

                //_playerMove���瓯�������e�����󂯎��
                _maxPelletCount = _playerMove.GetMaxPelletCount;
            }
            else
            {
                //����

                //_playerMove���猻�݂̔��˒e�����󂯎��
                _shotCounter = _playerMove.GetCurrentShotCount;

                //_playerMove����ő唭�˒e�����󂯎��
                _maxShotCount = _playerMove.GetMaxShotCount;
            }
        }

    }

    /// <summary>
    /// <para>SetupParameters</para>
    /// <para>�ϐ������ݒ�p���\�b�h �ŏ��ɏ����ݒ�������炠�Ƃ���ύX����Ȃ��ϐ��̏��������s��</para>
    /// </summary>
    private void SetupParameters()
    {
        //�����蔻��̃T�C�Y��ݒ�
        this._colliderRadius = _shotMoveData._colliderRadius;

        //�e�����L�������擾(�v���C���[�ƃ{�X�̂�)
        GetShooter();

        //����/���˂��Ƃ̉�����������e��?
        if (_shotMoveData._shotVelocity == ShotMoveData.ShotVelocity.Nomal)
        {
            return;
        }

        //���̒e�̎ˎ�͓G��?
        if (_shotMoveData._shooterType != ShotMoveData.ShooterType.Player)
        {
            //�G

            //�ˎ��EnemyCharactorMove���擾
            _characterMove = _shooter.GetComponent<EnemyCharacterMove>();

            //���˂��ƂɌ������邩���擾
            _isVelocityChangePerShot = _characterMove.GetIsChangeSpeedPerShot;
        }
        else
        {
            //�v���C���[

            //PlayerMove���擾
            _playerMove = _shooter.GetComponent<PlayerMove>();

            //���˂��ƂɌ������邩���擾
            _isVelocityChangePerShot = _playerMove.GetIsChengeSpeedPerShot;
        }
    }

    /// <summary>
    /// <para>CreateInstances</para>
    /// <para>�C���X�^���X�����p���\�b�h</para>
    /// </summary>
    private void CreateInstances()
    {
        _bezierCurve = new BezierCurve();

        _bezierCurveParameters = new BezierCurveParameters();
    }

    /// <summary>
    /// <para>SetInitialVelocity/para>
    /// <para>�����̐ݒ���s�����\�b�h</para>
    /// </summary>
    private void SetInitialVelocity()
    {
        /*�e�̏����v�Z��������
         * 
         * Nomal      : �����v�Z����(�f�t�H���g�l���̂܂�)
         * FastToSlow : ���� or �������Ƃɏ���������
         * SlowToFast : ���� or �������Ƃɏ���������
         */

        switch (_shotMoveData._shotVelocity)        //�e�̏����̐ݒ�ɉ����ď�������
        {
            //�ʏ�e(�����ω��Ȃ�)
            case ShotMoveData.ShotVelocity.Nomal:

                //�����Ƀf�t�H���g�l�̏�����ݒ�
                this._initialVelocity = _shotMoveData._initialVelocity;

                break;


            //���� or �������Ƃɏ������ω�
            case ShotMoveData.ShotVelocity.DynamicInitialVelocity:

                //���ː����Ƃɉ���/��������������ݒ肷��
                DynamicInitialVelocity();

                break;
        }
    }

    /// <summary>
    /// <para>DynamicInitialVelocity</para>
    /// <para>���˂��Ƃɏ������ω�������e�̏����v�Z����</para>
    /// </summary>
    private void DynamicInitialVelocity()
    {

        //�����ω������p���[�J���ϐ�

        int shotLength = 0;         //��x�ɐ�������e�� or ���˂���e�� + 1�i�[�p

        int shotCount = 0;          //��������e�̓����݉����ڂ����i�[����

        //���˂��Ƃɉ���������e��?
        if (!_isVelocityChangePerShot)
        {
            //�Ⴄ

            //��x�ɐ�������e�� + 1���i�[
            shotLength = _maxPelletCount + 1;

            //���݉����ڂ̐��������i�[
            shotCount = _pelletCounter;
        }
        else
        {
            //���˂��Ƃɉ���������e

            //�A�˂���e�� + 1���i�[
            shotLength = _maxShotCount + 1;

            //���݉����ڂ����i�[
            shotCount = _shotCounter;
        }

        //���� or �������Ƃ̌����l���Z�o
        float velocityChangeRate = _shotMoveData._initialVelocity / (shotLength * _shotMoveData._shotVelocityRate);

        //���� or �������Ƃ̌����l * ���ˁE���������v�Z(�f�t�H���g�̏����ɉ��Z���鑬�x)
        float calculatedVelocity = velocityChangeRate * shotCount;

        if (_shotMoveData._isAcceleration)
        {
            //�f�t�H���g�l������Z�������x�����̒e�̏����Ƃ��Ċi�[
            this._initialVelocity = _shotMoveData._initialVelocity + calculatedVelocity;
        }
        else
        {
            //�f�t�H���g�l������Z�������x�����̒e�̏����Ƃ��Ċi�[
            this._initialVelocity = _shotMoveData._initialVelocity - calculatedVelocity;
        }

    }

    /// <summary>
    /// <para>CalculateVelocity</para>
    /// <para></para>
    /// </summary>
    private void CalculateVelocity()
    {
        /*����O��    ��ɑ��x�֘A
         * Nomal                     : ����O������
         * Acceleration_Deceleration : ������
         * Laser                     : ���[�U�[
         */

        switch (_shotMoveData._shotSettings)
        {
            //���x�ω��Ȃ�
            case ShotMoveData.ShotSettings.Nomal:

                //���x�ɏ����l���i�[
                _speed = this._initialVelocity;

                break;


            //���ˌ�ɉ�����
            case ShotMoveData.ShotSettings.Acceleration_Deceleration:

                //�A�j���[�V�����J�[�u�̒l * �����l�ŎZ�o�������x���i�[
                _speed = _shotMoveData._speedCurve.Evaluate((float)_timer / _shotMoveData._timeToSpeedChange) * this._initialVelocity;

                break;

            case ShotMoveData.ShotSettings.Laser:       //���[�U�[�e

                break;
        }
    }

    /// <summary>
    /// <para>CalculateMoving</para>
    /// <para>�e�̋�������</para>
    /// </summary>
    /// <param name="currentPos">���݂̒e�̍��W</param>
    /// <returns>�v�Z��̒e�̍��W��Ԃ�</returns>
    private Vector2 CalculateMoving(Vector2 currentPos)
    {

        /*�V���b�g�̔�ѕ�
         * Straight       : ���i
         * Curve          : �J�[�u
         */

        switch (_shotMoveData._shotType)        //�e�̋O���ݒ�ɉ����ď�������
        {
            //���i�e
            case ShotMoveData.ShotType.Straight:

                //���ˊp�������������ɂ����ۂ̊p�x�ɏC��
                float radian = _shotAngle * (Mathf.PI / 180);

                //�ϊ������p�x���x�N�g���ɕϊ�
                _shotVector = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;

                //���߂��x�N�g���ɑ��x�A�o�ߎ��Ԃ��|�����l�����ݒn�ɂ���
                currentPos += _shotVector * _speed * Time.deltaTime;

                break;


            //�J�[�u�e
            case ShotMoveData.ShotType.Curve:

                //�o�ߎ��Ԃ�1�b������
                if (_timer < 1)
                {
                    //1�b����

                    currentPos = CurveShot();
                }
                else
                {
                    //1�b�ȏ�o��

                    //�ŏI�I�Ȍ����ɒ�����ɔ��

                    currentPos += StraightShotCalculateVector(_bezierCurve.GetFixedRelayPoint, _targetPosition) * _speed * Time.deltaTime;
                }

                break;
        }

        return currentPos;
    }

    /// <summary>
    /// <para>CurveShot</para>
    /// <para>�x�W�F�Ȑ����v�Z���A���݂̍��W�ɔ��f������B</para>
    /// <para>�C���X�^���X���������\���̂Ɍv�Z�ɗp����ϐ���n���A�������C���X�^���X���������x�W�F�Ȑ��v�Z�N���X�Ɉ����Ƃ��ēn��</para>
    /// </summary>
    /// <returns></returns>
    private Vector2 CurveShot()
    {
        //�\���̂ɕK�v�ϐ���n��

        //�J�n�n�_
        _bezierCurveParameters.StartingPosition = _shooterPosition;
        //�I�_
        _bezierCurveParameters.TargetPosition = _targetPosition;
        //�o�ߎ��Ԍv���^�C�}�[
        _bezierCurveParameters.Timer = _timer;
        //���ԓ_X���W
        _bezierCurveParameters.CurveMoveVerticalOffset = _shotMoveData._curveShotVerticalOffset;
        //���ԓ_Y���W
        _bezierCurveParameters.CurveMoveHorizontalOffset = _shotMoveData._curveShotHorizontalOffset;
        //�f�o�b�O���[�h�t���O
        _bezierCurveParameters.IsDebugMode = _shotMoveData._isDebug;


        //�x�W�F�Ȑ������ߌ��ʂ����݂̍��W�Ƃ���
        return _bezierCurve.CalculateBezierCurve(_bezierCurveParameters);
    }

    /// <summary>
    /// <para>ObjectDisabler</para>
    /// <para>setActive(false)���s���B �f�o�b�O�p�ɏ�����Ԃɖ߂����̕�����܂�</para>
    /// </summary>
    private void ObjectDisabler()
    {
        //�f�o�b�O���[�h��
        if (!_shotMoveData._isDebug)
        {
            //�ʏ���

            //�e�𖳌�������
            this.gameObject.SetActive(false);
        }
        else
        {
            //�f�o�b�O���[�h

            //���ˎ��̏�Ԃɖ߂�
            ResetParameters();
        }
    }

    /// <summary>
    /// <para>StraightShotCalculateVector</para>
    /// <para>�ڕW�n�_ - ���˒n�_�Ԃ̃x�N�g�������߂�</para>
    /// </summary>
    /// <param name="shotPos">���˒n�_</param>
    /// <param name="targetPos">�ڕW�n�_</param>
    /// <returns>direction = ���˒n�_�`�^�[�Q�b�g�n�_�Ԃ̃x�N�g��</returns>
    private Vector2 StraightShotCalculateVector(Vector2 shotPos, Vector2 targetPos)
    {
        //���˒n�_�`�ڕW�n�_�Ԃ̃x�N�g�������߂�
        Vector2 direction = (targetPos - shotPos).normalized;

        //�Ԃ茌�Ƃ��ĕԂ�
        return direction;
    }

    /// <summary>
    /// <para>GetShooter</para>
    /// <para>�ˎ��ݒ肷��B �v���C���[�A�{�X�̓L�����𒼐ړ����B ����ȊO�̎G���G�̓L��������󂯎��</para>
    /// </summary>
    private void GetShooter()
    {
        //�ˎ�̃^�C�v�ŏ�������
        switch (_shotMoveData._shooterType)
        {
            case ShotMoveData.ShooterType.Player:       //�v���C���[

                //�v���C���[�^�O�̃I�u�W�F�N�g���擾
                GetSetshooter = GameObject.FindGameObjectWithTag(PLAYER_TAG);

                break;

            case ShotMoveData.ShooterType.Boss:         //�{�X

                //�{�X�^�O�̃I�u�W�F�N�g���擾
                GetSetshooter = GameObject.FindGameObjectWithTag(BOSS_TAG);

                break;

            case ShotMoveData.ShooterType.Common:       //���̑��G���G

                break;
        }


    }

    /// <summary>
    /// <para>AnimEvent_Disable</para>
    /// <para>����������(�A�j���[�V�����C�x���g�p)</para>
    /// </summary>
    public void AnimEvent_Disable()
    {
        //���g�𖳌���
        this.gameObject.SetActive(false);
    }
}