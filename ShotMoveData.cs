using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/Create ShotMoveData")]
public class ShotMoveData : ScriptableObject
{

    [Label("�e")]
    public Sprite _shotSprite = default;

    [Label("�U���e")]
    public bool _isLockOnShot = false;

    [Label("��]")]
    public bool _isSpinning = false;

    [Label("�F�Ⴂ")]
    public bool _hasAlternativeColor = false;

    [Label("2�F�ڃX�v���C�g")]
    public Sprite _alternativeColorSprite = default;

    [Label("�����蔻��傫��"), Range(0.00f, 1.00f)]
    public float _colliderRadius = 0.05f;

    public enum ShooterType
    {
        Player,             //�v���C���[
        Boss,               //�{�X
        Common,             //�G���G
    }

    [Label("�e�����L����")]
    public ShooterType _shooterType;

    [Label("����")]
    public float _initialVelocity = 3;

    public enum ShotType
    {
        [EnumLabel("�e�̎��", "���i")]
        Straight,           //���i
        [EnumLabel("�e�̎��", "�J�[�u")]
        Curve,              //�J�[�u
        [EnumLabel("�e�̎��", "�ǔ�")]
        Homing,             //�ǔ�
    }

    [Label("�e�̎��"), EnumElements(typeof(ShotType))]
    public ShotType _shotType;

    public enum ShotSettings
    {
        [EnumLabel("����O��", "����")]
        Nomal,                           //����O������
        [EnumLabel("����O��", "������")]
        Acceleration_Deceleration,       //������
        [EnumLabel("����O��", "���[�U�[")]
        Laser,                           //���[�U�[
    }

    [Label("����O��"), EnumElements(typeof(ShotSettings))]
    public ShotSettings _shotSettings;

    [Label("�������̎���")]
    public int _timeToSpeedChange = default;

    public enum ShotVelocity
    {
        [EnumLabel("�e�̏���", "����")]
        Nomal,
        [EnumLabel("�e�̏���", "�������ƂɌ���")]
        FastToSlow,
        [EnumLabel("�e�̏���", "�������Ƃɉ���")]
        SlowToFast,
        [EnumLabel("�e�̏���", "�������Ƃɉ�����")]
        DynamicInitialVelocity,
    }

    [EnumElements(typeof(ShotVelocity))]
    public ShotVelocity _shotVelocity;

    public bool _isAcceleration = default;

    [Label("�������̉������"), Range(0.1f, 5.0f)]
    public float _shotVelocityRate = 1.5f;

    [Label("�������J�[�u"), HideInInspector]
    public AnimationCurve _speedCurve;          //�������J�[�u

    [Label("�J�[�u�p�c���I�t�Z�b�g"), Range(0.0f, 1.0f), HideInInspector]
    public float _curveShotVerticalOffset = default;

    [Label("�J�[�u�p�����I�t�Z�b�g"), Range(-1.0f, 1.0f), HideInInspector]
    public float _curveShotHorizontalOffset = default;

    [Label("�f�o�b�O���[�h")]
    public bool _isDebug = false;

}
