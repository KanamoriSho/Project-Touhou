using System;
using System.Linq;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    [SerializeField, Label("�����蔻��T�C�Y"), Range(0.0f, 1.0f)]
    private float _colliderScale = 0.05f;

    private enum Faction
    {
        [EnumLabel("�h��", "�v���C���[")]
        Player,
        [EnumLabel("�h��", "�G")]
        Enemy,
    }

    [SerializeField, EnumElements(typeof(Faction))]
    private Faction _faction;

    private enum Type
    {
        [EnumLabel("�L���� / �e", "�L�����N�^�[")]
        Charactor,
        [EnumLabel("�L���� / �e", "�e")]
        Shot,
    }

    [SerializeField, EnumElements(typeof(Type))]
    private Type _type;

    [SerializeField, Label("�G�e�̃^�O"), TagFieldDrawer]
    private string _enemyShotTag = default;     //���̃L�����ɂƂ��Ă̓G�̒e�̃^�O���i�[����ϐ�

    private enum ColliderShape
    {
        [EnumLabel("�����蔻��̌`��","��")]
        Sphere,
        [EnumLabel("�����蔻��̌`��","�����`")]
        Square,
    }

    [SerializeField, EnumElements(typeof(ColliderShape))]
    private ColliderShape _selfColliderShape;

    private ColliderShape _shotColliderShape;

    [SerializeField,Label("�l�p�`�̒��_")]
    private Vector2[] _vertices; // �l�p�`�̒��_

    private Vector2[] _normals; // �ӂ̖@���x�N�g��

    private PlayerMove _playerMove = default;   //�v���C���[��PlayerMove�̊i�[�p�ϐ�

    private bool _isHit = false;                //��e����t���O

    public int GetColliderShape
    {
        get { return (int)_selfColliderShape; }
    }

    public float GetColliderScale
    {
        //_colliderRadius��Ԃ�
        get { return _colliderScale; }
    }

    public bool GetSetHitFlag
    {
        //_isHit��Ԃ�
        get { return _isHit; }

        //_isHit�Ɏ󂯎�����l������
        set { _isHit = value; }
    }

    private void Awake()
    {

        switch (_selfColliderShape)
        {
            case ColliderShape.Sphere:

                break;

            case ColliderShape.Square:


                //�����v���ɒ��_���W���`
                _vertices = new Vector2[4]
                {  
                    //����
                    new Vector2(-_colliderScale, -_colliderScale),
                    //�E��
                    new Vector2(_colliderScale, -_colliderScale),
                    //�E��
                    new Vector2(_colliderScale, _colliderScale),
                    //����
                    new Vector2(-_colliderScale, _colliderScale)
                };

                // �ӂ̖@���x�N�g�����v�Z����
                _normals = new Vector2[4];
                for (int i = 0; i < 4; i++)
                {
                    // ���݂̒��_�Ǝ��̒��_���擾����
                    Vector2 current = _vertices[i];
                    Vector2 next = _vertices[(i + 1) % 4];

                    // �ӂ̃x�N�g�����v�Z����
                    Vector2 edge = next - current;

                    // �ӂ̖@���x�N�g�����v�Z����
                    _normals[i] = new Vector2(-edge.y, edge.x).normalized;
                }

                break;
        }

        //�v���C���[��?
        if (_faction == Faction.Player)
        {
            //�v���C���[�ł���

            //�v���C���[��PlayerMove�R���|�[�l���g���擾
            _playerMove = this.gameObject.GetComponent<PlayerMove>();
        }
    }

    private void Update()
    {
        //�����͒e��?
        if(_type == Type.Shot)
        {
            //�e�ł���

            //�������Ȃ�
            return;
        }

        //�ȉ����菈��

        //��ʓ��̑S�G�e���擾
        Transform[] enemyShotsInScene = GameObject.FindGameObjectsWithTag(_enemyShotTag).Select(enemyShot => enemyShot.transform).ToArray();

        //�G�e������Ȃ�
        if (enemyShotsInScene.Length == 0)
        {
            //�����������߂�
            return;
        }

        //�擾�����G�e�������̏Ə��Ƀ\�[�g
        Transform[] sortedByDistance =
                    enemyShotsInScene.OrderBy(enemyShots => Vector3.Distance(enemyShots.transform.position, transform.position)).ToArray();

        CollisionManager currentCollisionManager = sortedByDistance[0].gameObject.GetComponent<CollisionManager>();

        int shotColliderShape = currentCollisionManager.GetColliderShape;

        _shotColliderShape = (ColliderShape)Enum.ToObject(typeof(ColliderShape), shotColliderShape);

        float shotColliderScale = currentCollisionManager.GetColliderScale;

        //��ԋ߂��e�̓����蔻��̔��a + ���g�̓����蔻��̔��a�����߂�
        float sumOfColliderScale = shotColliderScale + this._colliderScale;


        switch (_shotColliderShape)
        {
            case ColliderShape.Sphere:

                //���g���v���C���[���A���G��Ԃ̏ꍇ
                if (_faction == Faction.Player && _playerMove.GetIsInvincible)
                {
                    //���������Ȃ�
                    return;
                }

                //�e�Ƃ̋����������蔻��̔��a�̍��v�����������Ȃ�����
                if (Vector3.Distance(sortedByDistance[0].position, transform.position) <= sumOfColliderScale && !_isHit)
                {
                    //��e�t���O��true��
                    _isHit = true;

                    //��ʓ��̓G�e�̔z���������
                    enemyShotsInScene = new Transform[0];

                    //�������̔z���������
                    sortedByDistance = new Transform[0];
                }


                break;

            case ColliderShape.Square:


                //�����v���ɒ��_���W���`
                _vertices = new Vector2[4]
                {  
                    //����
                    new Vector2(-shotColliderScale, -shotColliderScale),
                    //�E��
                    new Vector2(shotColliderScale, -shotColliderScale),
                    //�E��
                    new Vector2(shotColliderScale, shotColliderScale),
                    //����
                    new Vector2(shotColliderScale, shotColliderScale)
                };

                // �ӂ̖@���x�N�g�����v�Z����
                _normals = new Vector2[4];
                for (int i = 0; i < 4; i++)
                {
                    // ���݂̒��_�Ǝ��̒��_���擾����
                    Vector2 current = _vertices[i];
                    Vector2 next = _vertices[(i + 1) % 4];

                    // �ӂ̃x�N�g�����v�Z����
                    Vector2 edge = next - current;

                    // �ӂ̖@���x�N�g�����v�Z����
                    _normals[i] = new Vector2(-edge.y, edge.x).normalized;
                }


                // �e�ӂ̋������v�Z
                float[] distances = new float[4];
                for (int i = 0; i < 4; i++)
                {
                    Vector2 current = _vertices[i];
                    Vector2 next = _vertices[(i + 1) % 4];
                    Vector2 edge = next - current;
                    Vector2 edgeNormal = _normals[i];
                    distances[i] = Vector2.Dot(this.transform.position - (Vector3)current, edgeNormal);
                }

                //���g���v���C���[���A���G��Ԃ̏ꍇ
                if (_faction == Faction.Player && _playerMove.GetIsInvincible)
                {
                    //���������Ȃ�
                    return;
                }

                // �����蔻�肪�����������ǂ����𔻒f
                if (distances[0] <= 0 && distances[1] <= 0 && distances[2] <= 0 && distances[3] <= 0)
                {
                    //��e�t���O��true��
                    _isHit = true;

                    //��ʓ��̓G�e�̔z���������
                    enemyShotsInScene = new Transform[0];

                    //�������̔z���������
                    sortedByDistance = new Transform[0];
                }

                break;
        }
    

    }

    /// <summary>
    /// <para>OnDrawGizmos</para>
    /// <para>�����蔻��̃T�C�Y��_�ŕ`�悷�郁�\�b�h �f�o�b�O�p</para>
    /// </summary>
    private void OnDrawGizmos()
    {

#if UNITY_EDITOR

        Gizmos.DrawSphere(this.gameObject.transform.position, _colliderScale);
#endif

    }
}
