using UnityEngine;

public class BezierCurve
{
    private Vector2 _fixedRelayPoint = default;         //�x�W�F�Ȑ��̒��ԓ_�i�[�p

    public Vector2 GetFixedRelayPoint
    {
        //_fixedRelayPoint��Ԃ�
        get { return _fixedRelayPoint; }
    }

    private Vector2 _relayPointY = default;

    /// <summary>
    /// <para>CalcuateBezierCurve</para>
    /// <para>�J�[�u�ړ��̃x�W�F�Ȑ��𐶐��E���ݒn�_���Z�o����X�N���v�g</para>
    /// </summary>
    /// <param name="bezierCurvePositions">�x�W�F�Ȑ��̎Z�o�ɕK�v�Ȏn�_, �I�_, ���ԓ_��X���W/Y���W�p�����[�^, �o�ߎ��Ԃ��i�[����Ă���\����</param>
    /// <returns></returns>
    public Vector2 CalculateBezierCurve(BezierCurveParameters bezierCurvePositions)
    {

        //�e/�L�����N�^�[�̌��ݍ��W
        Vector2 currentMoveCheckpoint = bezierCurvePositions.StartingPosition;

        //�e/�L�����N�^�[�̖ڕW���W
        Vector2 nextMoveCheckpoint = bezierCurvePositions.TargetPosition;

        //���ݍ��W - �ڕW���W�Ԃ̃x�N�g�����Z�o
        Vector2 relayPointVector = currentMoveCheckpoint - nextMoveCheckpoint;


        /*�x�N�g�����0.0�`1.0�ŃI�t�Z�b�g�������ԓ_���Z�o
         * 0.0 : ���ݍ��W
         * 0.5 : ���ݍ��W�ƖڕW���W�Ԃ̒���
         * 1.0 : �ڕW���W
         */
        _relayPointY = Vector2.Lerp(currentMoveCheckpoint, nextMoveCheckpoint, bezierCurvePositions.CurveMoveVerticalOffset);

        /*�ړ��O���̍��E�l�ɉ����Čv�Z����x�N�g���̌�����ύX����
         * 
         * ���ɔ�΂��ꍇ��relayPointVector�ɑ΂��č������̐����x�N�g���ɑ΂��č��E�l��������
         * �E�ɔ�΂��ꍇ��relayPointVector�ɑ΂��ĉE�����̐����x�N�g���ɑ΂��č��E�l��������
         * 
         * _relayPointY�ŋ��߂��x�N�g����̒��Ԓn�_�����Ƃɐ����x�N�g�����o��
         */

        //�x�N�g���ɑ΂��鉡���I�t�Z�b�g�l��ݒ�
        float horizontalAxisOffset = bezierCurvePositions.CurveMoveHorizontalOffset;

        //���E�l���}�C�i�X(�������ł��邩)
        if (horizontalAxisOffset < 0)
        {
            //�������ł���

            //���݂̃`�F�b�N�|�C���g�`���̃`�F�b�N�|�C���g�ԃx�N�g���ɑ΂��鍶�����ɐ����ȃx�N�g�������߂�
            Vector2 leftPointingVector = new Vector2(-relayPointVector.y, relayPointVector.x);

            //�Z�o�����x�N�g���ɑ΂��Ē��Ԓn�_��Y���I�t�Z�b�g��X���I�t�Z�b�g�l�𑫂��A���Ԓn�_�̍��W�����߂�
            _fixedRelayPoint = _relayPointY + leftPointingVector * Mathf.Abs(horizontalAxisOffset);
        }
        else
        {
            //�E�����ł���

            //���݂̃`�F�b�N�|�C���g�`���̃`�F�b�N�|�C���g�ԃx�N�g���ɑ΂���E�����ɐ����ȃx�N�g�������߂�
            Vector2 rightPointingVector = new Vector2(relayPointVector.y, -relayPointVector.x);

            //�Z�o�����x�N�g���ɑ΂��Ē��Ԓn�_��Y���I�t�Z�b�g��X���I�t�Z�b�g�l�𑫂��A���Ԓn�_�̍��W�����߂�
            _fixedRelayPoint = _relayPointY + rightPointingVector * Mathf.Abs(horizontalAxisOffset);
        }

        /* ���݂̃`�F�b�N�|�C���g�`���ԓ_�A���ԓ_�`���̃`�F�b�N�|�C���g���q���������Lerp�ړ������AfirstVector�AsecodVector2�̈ړ�������W�����߂�B
         * 
         * firstVector�AsecodVector2�̍��W�Ԃ�Lerp�ړ������A�Ȑ���̍��WcurrentCurvePos�����߂�B
         * 
         * �Z�o����currentCurvePos��Ԃ�
         */

        //���݂̃`�F�b�N�|�C���g�`���ԓ_�Ԃ̃x�N�g����𓙑������^��������
        Vector2 firstVector = Vector2.Lerp(currentMoveCheckpoint, _fixedRelayPoint, bezierCurvePositions.Timer);

        //���ԓ_�`���̃`�F�b�N�|�C���g�Ԃ̃x�N�g����𓙑������^��������
        Vector2 secondtVector = Vector2.Lerp(_fixedRelayPoint, nextMoveCheckpoint, bezierCurvePositions.Timer);

        //firstVector�`secondVector�Ԃ̃x�N�g����𓙑������^��������W�����߂�
        Vector2 currentCurvePos = Vector2.Lerp(firstVector, secondtVector, bezierCurvePositions.Timer);

        //�Z�o�������W��Ԃ��l�Ƃ��ĕԂ�
        return currentCurvePos;
    }
}

/// <summary>
/// <para>BezierCurveParameters</para>
/// <para>�x�W�F�Ȑ��̎Z�o(CalculateBezierCurve)�ɕK�v�ȃp�����[�^���i�[����\����</para>
/// </summary>
public struct BezierCurveParameters
{
    public Vector2 StartingPosition;
    public Vector2 TargetPosition;
    public float CurveMoveVerticalOffset;
    public float CurveMoveHorizontalOffset;
    public float Timer;
    public bool IsDebugMode;
}