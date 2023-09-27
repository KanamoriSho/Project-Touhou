using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Create PlayerMoveData")]
public class PlayerMoveData : ScriptableObject
{
    [Label("�L�����N�^�[��")]
    public string _characterName;

    [Label("HP")]
    public int _maxHP;

    [Label("�V���b�g����SE")]
    public AudioClip _shotSoundEffect = default;

    [Label("��e����SE")]
    public AudioClip _hitSoundEffect = default;

    [Label("�ړ��X�s�[�h")]
    public int _speed;

    [Label("���̃L�������g�p����e")]
    public List<GameObject> _shots = new List<GameObject>();

    //���˂���e�����i�[���郊�X�g
    [Label("�V���b�g��"), Range(1, 10)]
    public int[] _shotCounts;

    //���˂���e�̎��@�_���̗L�����i�[���郊�X�g
    [Label("���@�_��")]
    public bool[] _isTargetingEnemy;

    [Label("�����蔻��̃T�C�Y")]
    public float _colliderRadius = 0.1f;

    [Label("��e�㖳�G����")]
    public float _afterHitInvincibleTime = 5.0f;

    [Label("�{���̃N�[���^�C��")]
    public float _bombCoolTime = 5.0f;

    [Label("���̈ړ��͈�")]
    public float _xLimitOfMoving = 5.0f;

    [Label("�c�̈ړ��͈�")]
    public float _yLimitOfMoving = 10.0f;

    [Label("�e")]
    public Sprite _sprite;

    [Label("�b�Ԕ��ː�"), Range(1, 100)]
    public int[] _shotPerSeconds = default;

    [Label("�v�[���p���������e��")]
    public int _initiallyGeneratedShots = 50;

    [Label("�J�[�u�p�c���I�t�Z�b�g"), Range(0.0f, 1.0f)]
    public float _verticalOffset = default;

    [Label("�J�[�u�p�����I�t�Z�b�g"), Range(-1.0f, 1.0f)]
    public float _horizontalOffset = default;

    public enum ShotPatern
    {
        [EnumLabel("�e�̌�����", "�P��")]
        OneShot,
        [EnumLabel("�e�̌�����", "��x��")]
        AllAtOnce,
        [EnumLabel("�e�̌�����", "������")]
        MultipleShots,
        [EnumLabel("�e�̌�����", "���ˏ�")]
        RadialShots,
    }

    [Label("�e�̌�����"), EnumElements(typeof(ShotPatern))]
    public List<ShotPatern> _shotPaterns = new List<ShotPatern>();

    [Label("��x�ɐ�������e��")]
    public int[] _pelletCountInShots = default;

    [Label("�����������̉E�[�`���[�Ԃ̊p�x")]
    public int[] _multiShotFormedAngles = default;

    [Label("���˂��ƂɌ������邩")]
    [Tooltip("true : �������������邲�ƂɌ��� �A�˂̍ۂɈꔭ�������@\n false: ���������̍ۂɈꔭ������ �A�˂̍ۂ͈ꔭ���Ƃ̌����͂��Ȃ�")]
    public bool[] _isChangeSpeedPerShot = default;
}
