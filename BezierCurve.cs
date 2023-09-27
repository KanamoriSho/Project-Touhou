using UnityEngine;

public class BezierCurve
{
    private Vector2 _fixedRelayPoint = default;         //ベジェ曲線の中間点格納用

    public Vector2 GetFixedRelayPoint
    {
        //_fixedRelayPointを返す
        get { return _fixedRelayPoint; }
    }

    private Vector2 _relayPointY = default;

    /// <summary>
    /// <para>CalcuateBezierCurve</para>
    /// <para>カーブ移動のベジェ曲線を生成・現在地点を算出するスクリプト</para>
    /// </summary>
    /// <param name="bezierCurvePositions">ベジェ曲線の算出に必要な始点, 終点, 中間点のX座標/Y座標パラメータ, 経過時間が格納されている構造体</param>
    /// <returns></returns>
    public Vector2 CalculateBezierCurve(BezierCurveParameters bezierCurvePositions)
    {

        //弾/キャラクターの現在座標
        Vector2 currentMoveCheckpoint = bezierCurvePositions.StartingPosition;

        //弾/キャラクターの目標座標
        Vector2 nextMoveCheckpoint = bezierCurvePositions.TargetPosition;

        //現在座標 - 目標座標間のベクトルを算出
        Vector2 relayPointVector = currentMoveCheckpoint - nextMoveCheckpoint;


        /*ベクトル上の0.0～1.0でオフセットした中間点を算出
         * 0.0 : 現在座標
         * 0.5 : 現在座標と目標座標間の中央
         * 1.0 : 目標座標
         */
        _relayPointY = Vector2.Lerp(currentMoveCheckpoint, nextMoveCheckpoint, bezierCurvePositions.CurveMoveVerticalOffset);

        /*移動軌道の左右値に応じて計算するベクトルの向きを変更する
         * 
         * 左に飛ばす場合はrelayPointVectorに対して左向きの垂直ベクトルに対して左右値をかける
         * 右に飛ばす場合はrelayPointVectorに対して右向きの垂直ベクトルに対して左右値をかける
         * 
         * _relayPointYで求めたベクトル上の中間地点をもとに垂直ベクトルを出す
         */

        //ベクトルに対する横軸オフセット値を設定
        float horizontalAxisOffset = bezierCurvePositions.CurveMoveHorizontalOffset;

        //左右値がマイナス(左向きであるか)
        if (horizontalAxisOffset < 0)
        {
            //左向きである

            //現在のチェックポイント～次のチェックポイント間ベクトルに対する左向きに垂直なベクトルを求める
            Vector2 leftPointingVector = new Vector2(-relayPointVector.y, relayPointVector.x);

            //算出したベクトルに対して中間地点のY軸オフセットとX軸オフセット値を足し、中間地点の座標を求める
            _fixedRelayPoint = _relayPointY + leftPointingVector * Mathf.Abs(horizontalAxisOffset);
        }
        else
        {
            //右向きである

            //現在のチェックポイント～次のチェックポイント間ベクトルに対する右向きに垂直なベクトルを求める
            Vector2 rightPointingVector = new Vector2(relayPointVector.y, -relayPointVector.x);

            //算出したベクトルに対して中間地点のY軸オフセットとX軸オフセット値を足し、中間地点の座標を求める
            _fixedRelayPoint = _relayPointY + rightPointingVector * Mathf.Abs(horizontalAxisOffset);
        }

        /* 現在のチェックポイント～中間点、中間点～次のチェックポイントを繋ぐ直線上をLerp移動させ、firstVector、secodVector2つの移動する座標を求める。
         * 
         * firstVector、secodVector2つの座標間をLerp移動させ、曲線上の座標currentCurvePosを求める。
         * 
         * 算出したcurrentCurvePosを返す
         */

        //現在のチェックポイント～中間点間のベクトル上を等速直線運動させる
        Vector2 firstVector = Vector2.Lerp(currentMoveCheckpoint, _fixedRelayPoint, bezierCurvePositions.Timer);

        //中間点～次のチェックポイント間のベクトル上を等速直線運動させる
        Vector2 secondtVector = Vector2.Lerp(_fixedRelayPoint, nextMoveCheckpoint, bezierCurvePositions.Timer);

        //firstVector～secondVector間のベクトル上を等速直線運動する座標を求める
        Vector2 currentCurvePos = Vector2.Lerp(firstVector, secondtVector, bezierCurvePositions.Timer);

        //算出した座標を返し値として返す
        return currentCurvePos;
    }
}

/// <summary>
/// <para>BezierCurveParameters</para>
/// <para>ベジェ曲線の算出(CalculateBezierCurve)に必要なパラメータを格納する構造体</para>
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