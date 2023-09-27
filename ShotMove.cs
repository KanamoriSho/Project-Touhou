using UnityEngine;

public class ShotMove : MonoBehaviour
{
    [Label("ショットムーブデータ")]
    public ShotMoveData _shotMoveData = default;            //弾軌道のスクリプタブルオブジェクト

    private SpriteRenderer _spriteRenderer = default;       //自身のSpriteRenderer格納用

    private Animator _animator = default;                   //自身のAnimator格納用

    private EnemyCharacterMove _characterMove = default;    //敵キャラクターの挙動スクリプトの格納用

    private PlayerMove _playerMove = default;               //プレイヤーの挙動スクリプトの格納用

    private GameObject _shooter = default;                  //射手格納用

    private float _timer = default;                         //経過時間計測用

    private float _initialVelocity = default;               //初速値

    private float _speed = default;                         //速度格納用

    private float _colliderRadius = default;                //当たり判定の大きさを格納

    private float _shotAngle = default;                     //発射角格納用

    private int _shotCounter = default;                     //その弾を発射した数を格納する(CharactorDataから受け取り)

    private int _maxShotCount = default;                    //その弾の最大発射数を格納する(CharactorDataから受け取り)

    private int _pelletCounter = default;                   //現在生成された弾の数を格納する(CharactorDataから受け取り)

    private int _maxPelletCount = default;                  //同時に生成する弾の数を格納する(CharactorDataから受け取り)

    private bool _isVelocityChangePerShot = false;          //発射ごとに初速を加減速するかを格納する

    private Vector2 _shooterPosition = default;             //射手のPosition格納用

    private Vector2 _shotVector = default;                  //弾の発射ベクトル格納用

    private Vector2 _targetPosition = default;              //_targetのPosition格納用

    BezierCurve _bezierCurve = default;                     //ベジェ曲線処理クラスのインスタンス取得用

    BezierCurveParameters _bezierCurveParameters = default; //ベジェ曲線処理の引数格納用構造体のインスタンス取得用

    private const string PLAYER_TAG = "Player";             //プレイヤーのタグ

    private const string BOSS_TAG = "Boss";                 //ボスのタグ

    #region Getter, Setter

    public float GetColliderRadius
    {
        //_colliderRadiusを返す
        get { return _colliderRadius; }
    }

    public GameObject GetSetshooter
    {
        //_shooterを返す
        get { return _shooter; }
        //渡された値を_shooterに格納
        set { _shooter = value; }
    }

    public float GetSetShotAngle
    {
        //_shotAngleを返す
        get { return _shotAngle; }
        //渡された値を_shotAngleに設定
        set
        {
            _shotAngle = value;
        }
    }

    #endregion

    private void Awake()
    {
        //コンポーネントを取得
        GetComponents();

        //インスタンス化したクラスの作成
        CreateInstances();

        //最初に一度だけ参照するパラメータの初期設定
        SetupParameters();

        //パラメータの初期設定
        ResetParameters();
    }

    private void OnEnable()
    {
        //初期化処理
        ResetParameters();

        //初速の設定
        SetInitialVelocity();
    }

    private void Update()
    {
        _timer += Time.deltaTime;                        //時間の加算

        Vector2 currentPos = this.transform.position;   //現在の自身の座標を取得

        //速度の計算
        CalculateVelocity();

        //計算結果の座標を自身の座標にする
        this.transform.position = CalculateMoving(currentPos);
    }

    /// <summary>
    /// <para>OnBecameInvisible</para>
    /// <para>画面外に出た弾を消す処理</para>
    /// </summary>
    private void OnBecameInvisible()
    {
        //弾の無効化処理
        ObjectDisabler();
    }

    /// <summary>
    /// <para>GetComponents</para>
    /// <para>コンポーネント取得用メソッド</para>
    /// </summary>
    private void GetComponents()
    {
        //SpriteRenderer取得
        _spriteRenderer = this.GetComponent<SpriteRenderer>();

        //自身のAnimatorを取得
        _animator = this.GetComponent<Animator>();
    }

    /// <summary>
    /// <para>ResetParameters</para>
    /// <para>変数初期化用メソッド デバッグ時にも呼び出せるようにメソッド化してます</para>
    /// </summary>
    private void ResetParameters()
    {
        //経過時間をリセット
        _timer = 0;

        //射手の座標を再設定
        _shooterPosition = _shooter.transform.position;

        //自身を射手の座標に移動
        this.transform.position = _shooterPosition;

        //スプライト変更
        _spriteRenderer.sprite = _shotMoveData._shotSprite;

        //当たり判定の大きさが_shotMoveDataのものと異なるか
        if (this._colliderRadius != _shotMoveData._colliderRadius)
        {
            //異なる

            //当たり判定のサイズを再設定
            this._colliderRadius = _shotMoveData._colliderRadius;
        }

        //弾の回転フラグがtrueか
        if (_shotMoveData._isSpinning)
        {
            //trueならランダムで回転させる
            this.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }

        //射手がプレイヤーか
        if (_shotMoveData._shooterType != ShotMoveData.ShooterType.Player)
        {
            //プレイヤーではない

            //弾のアニメーションを初期化
            _animator.SetTrigger("Enable");
        }

        //2色目がある弾か
        if (_shotMoveData._hasAlternativeColor)
        {
            //2色目があるなら

            //現在何発目の弾かを取得
            _shotCounter = _characterMove.GetCurrentShotCount;

            //奇遇判定
            if (_shotCounter % 2 == 1)
            {
                //奇数なら

                //デフォルトのスプライトに
                _animator.SetBool("AltColor", false);
            }
            else
            {
                //遇数なら

                //2色目のスプライトに
                _animator.SetBool("AltColor", true);
            }
        }

        //生成ごとに加速/減速しない弾なら
        if (_shotMoveData._shotVelocity == ShotMoveData.ShotVelocity.Nomal)
        {
            return;
        }

        //以下生成ごとに加減速する弾の場合

        //プレイヤーでは無い?
        if (_shotMoveData._shooterType != ShotMoveData.ShooterType.Player)
        {
            //プレイヤーではない

            //発射ごとに減速するかを取得
            _isVelocityChangePerShot = _characterMove.GetIsChangeSpeedPerShot;

            //発射ごとに加減速するか?
            if (!_isVelocityChangePerShot)
            {
                //しない(生成ごとに加減速)

                //_characterMoveから現在の生成弾数を受け取る
                _pelletCounter = _characterMove.GetCurrentPelletCount;

                //_characterMoveから同時生成弾数を受け取る
                _maxPelletCount = _characterMove.GetMaxPelletCount;
            }
            else
            {
                //する

                //_characterMoveから現在の生成弾数を受け取る
                _shotCounter = _characterMove.GetCurrentShotCount;

                //_characterMoveから最大発射弾数を受け取る
                _maxShotCount = _characterMove.GetMaxShotCount;
            }
        }
        else
        {
            //プレイヤーである

            //発射ごとに減速するかを取得
            _isVelocityChangePerShot = _playerMove.GetIsChengeSpeedPerShot;

            //発射ごとに加減速するか?
            if (!_isVelocityChangePerShot)
            {
                //しない(生成ごとに加減速)

                //_playerMoveから現在の生成弾数を受け取る
                _pelletCounter = _playerMove.GetCurrentPelletCount;

                //_playerMoveから同時生成弾数を受け取る
                _maxPelletCount = _playerMove.GetMaxPelletCount;
            }
            else
            {
                //する

                //_playerMoveから現在の発射弾数を受け取る
                _shotCounter = _playerMove.GetCurrentShotCount;

                //_playerMoveから最大発射弾数を受け取る
                _maxShotCount = _playerMove.GetMaxShotCount;
            }
        }

    }

    /// <summary>
    /// <para>SetupParameters</para>
    /// <para>変数初期設定用メソッド 最初に初期設定をしたらあとから変更されない変数の初期化を行う</para>
    /// </summary>
    private void SetupParameters()
    {
        //当たり判定のサイズを設定
        this._colliderRadius = _shotMoveData._colliderRadius;

        //弾を撃つキャラを取得(プレイヤーとボスのみ)
        GetShooter();

        //生成/発射ごとの加減速をする弾か?
        if (_shotMoveData._shotVelocity == ShotMoveData.ShotVelocity.Nomal)
        {
            return;
        }

        //この弾の射手は敵か?
        if (_shotMoveData._shooterType != ShotMoveData.ShooterType.Player)
        {
            //敵

            //射手のEnemyCharactorMoveを取得
            _characterMove = _shooter.GetComponent<EnemyCharacterMove>();

            //発射ごとに減速するかを取得
            _isVelocityChangePerShot = _characterMove.GetIsChangeSpeedPerShot;
        }
        else
        {
            //プレイヤー

            //PlayerMoveを取得
            _playerMove = _shooter.GetComponent<PlayerMove>();

            //発射ごとに減速するかを取得
            _isVelocityChangePerShot = _playerMove.GetIsChengeSpeedPerShot;
        }
    }

    /// <summary>
    /// <para>CreateInstances</para>
    /// <para>インスタンス生成用メソッド</para>
    /// </summary>
    private void CreateInstances()
    {
        _bezierCurve = new BezierCurve();

        _bezierCurveParameters = new BezierCurveParameters();
    }

    /// <summary>
    /// <para>SetInitialVelocity/para>
    /// <para>初速の設定を行うメソッド</para>
    /// </summary>
    private void SetInitialVelocity()
    {
        /*弾の初速計算処理分け
         * 
         * Nomal      : 初速計算無し(デフォルト値そのまま)
         * FastToSlow : 発射 or 生成ごとに初速が減速
         * SlowToFast : 発射 or 生成ごとに初速が加速
         */

        switch (_shotMoveData._shotVelocity)        //弾の初速の設定に応じて処理分け
        {
            //通常弾(初速変化なし)
            case ShotMoveData.ShotVelocity.Nomal:

                //初速にデフォルト値の初速を設定
                this._initialVelocity = _shotMoveData._initialVelocity;

                break;


            //発射 or 生成ごとに初速が変化
            case ShotMoveData.ShotVelocity.DynamicInitialVelocity:

                //発射数ごとに加速/減速した初速を設定する
                DynamicInitialVelocity();

                break;
        }
    }

    /// <summary>
    /// <para>DynamicInitialVelocity</para>
    /// <para>発射ごとに初速が変化さする弾の初速計算処理</para>
    /// </summary>
    private void DynamicInitialVelocity()
    {

        //初速変化処理用ローカル変数

        int shotLength = 0;         //一度に生成する弾数 or 発射する弾数 + 1格納用

        int shotCount = 0;          //生成する弾の内現在何発目かを格納する

        //発射ごとに加減速する弾か?
        if (!_isVelocityChangePerShot)
        {
            //違う

            //一度に生成する弾数 + 1を格納
            shotLength = _maxPelletCount + 1;

            //現在何発目の生成かを格納
            shotCount = _pelletCounter;
        }
        else
        {
            //発射ごとに加減速する弾

            //連射する弾数 + 1を格納
            shotLength = _maxShotCount + 1;

            //現在何発目かを格納
            shotCount = _shotCounter;
        }

        //発射 or 生成ごとの減速値を算出
        float velocityChangeRate = _shotMoveData._initialVelocity / (shotLength * _shotMoveData._shotVelocityRate);

        //発射 or 生成ごとの減速値 * 発射・生成数を計算(デフォルトの初速に加算する速度)
        float calculatedVelocity = velocityChangeRate * shotCount;

        if (_shotMoveData._isAcceleration)
        {
            //デフォルト値から加算した速度をこの弾の初速として格納
            this._initialVelocity = _shotMoveData._initialVelocity + calculatedVelocity;
        }
        else
        {
            //デフォルト値から加算した速度をこの弾の初速として格納
            this._initialVelocity = _shotMoveData._initialVelocity - calculatedVelocity;
        }

    }

    /// <summary>
    /// <para>CalculateVelocity</para>
    /// <para></para>
    /// </summary>
    private void CalculateVelocity()
    {
        /*特殊軌道    主に速度関連
         * Nomal                     : 特殊軌道無し
         * Acceleration_Deceleration : 加減速
         * Laser                     : レーザー
         */

        switch (_shotMoveData._shotSettings)
        {
            //速度変化なし
            case ShotMoveData.ShotSettings.Nomal:

                //速度に初速値を格納
                _speed = this._initialVelocity;

                break;


            //発射後に加減速
            case ShotMoveData.ShotSettings.Acceleration_Deceleration:

                //アニメーションカーブの値 * 初速値で算出した速度を格納
                _speed = _shotMoveData._speedCurve.Evaluate((float)_timer / _shotMoveData._timeToSpeedChange) * this._initialVelocity;

                break;

            case ShotMoveData.ShotSettings.Laser:       //レーザー弾

                break;
        }
    }

    /// <summary>
    /// <para>CalculateMoving</para>
    /// <para>弾の挙動処理</para>
    /// </summary>
    /// <param name="currentPos">現在の弾の座標</param>
    /// <returns>計算後の弾の座標を返す</returns>
    private Vector2 CalculateMoving(Vector2 currentPos)
    {

        /*ショットの飛び方
         * Straight       : 直進
         * Curve          : カーブ
         */

        switch (_shotMoveData._shotType)        //弾の軌道設定に応じて処理分け
        {
            //直進弾
            case ShotMoveData.ShotType.Straight:

                //発射角を下方向を軸にした際の角度に修正
                float radian = _shotAngle * (Mathf.PI / 180);

                //変換した角度をベクトルに変換
                _shotVector = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;

                //求めたベクトルに速度、経過時間を掛けた値を現在地にする
                currentPos += _shotVector * _speed * Time.deltaTime;

                break;


            //カーブ弾
            case ShotMoveData.ShotType.Curve:

                //経過時間が1秒未満か
                if (_timer < 1)
                {
                    //1秒未満

                    currentPos = CurveShot();
                }
                else
                {
                    //1秒以上経過

                    //最終的な向きに直線状に飛ぶ

                    currentPos += StraightShotCalculateVector(_bezierCurve.GetFixedRelayPoint, _targetPosition) * _speed * Time.deltaTime;
                }

                break;
        }

        return currentPos;
    }

    /// <summary>
    /// <para>CurveShot</para>
    /// <para>ベジェ曲線を計算し、現在の座標に反映させる。</para>
    /// <para>インスタンス生成した構造体に計算に用いる変数を渡し、同じくインスタンス生成したベジェ曲線計算クラスに引数として渡す</para>
    /// </summary>
    /// <returns></returns>
    private Vector2 CurveShot()
    {
        //構造体に必要変数を渡す

        //開始地点
        _bezierCurveParameters.StartingPosition = _shooterPosition;
        //終点
        _bezierCurveParameters.TargetPosition = _targetPosition;
        //経過時間計測タイマー
        _bezierCurveParameters.Timer = _timer;
        //中間点X座標
        _bezierCurveParameters.CurveMoveVerticalOffset = _shotMoveData._curveShotVerticalOffset;
        //中間点Y座標
        _bezierCurveParameters.CurveMoveHorizontalOffset = _shotMoveData._curveShotHorizontalOffset;
        //デバッグモードフラグ
        _bezierCurveParameters.IsDebugMode = _shotMoveData._isDebug;


        //ベジェ曲線を求め結果を現在の座標とする
        return _bezierCurve.CalculateBezierCurve(_bezierCurveParameters);
    }

    /// <summary>
    /// <para>ObjectDisabler</para>
    /// <para>setActive(false)を行う。 デバッグ用に初期状態に戻すかの分岐も含む</para>
    /// </summary>
    private void ObjectDisabler()
    {
        //デバッグモードか
        if (!_shotMoveData._isDebug)
        {
            //通常状態

            //弾を無効化する
            this.gameObject.SetActive(false);
        }
        else
        {
            //デバッグモード

            //発射時の状態に戻す
            ResetParameters();
        }
    }

    /// <summary>
    /// <para>StraightShotCalculateVector</para>
    /// <para>目標地点 - 発射地点間のベクトルを求める</para>
    /// </summary>
    /// <param name="shotPos">発射地点</param>
    /// <param name="targetPos">目標地点</param>
    /// <returns>direction = 発射地点～ターゲット地点間のベクトル</returns>
    private Vector2 StraightShotCalculateVector(Vector2 shotPos, Vector2 targetPos)
    {
        //発射地点～目標地点間のベクトルを求める
        Vector2 direction = (targetPos - shotPos).normalized;

        //返り血として返す
        return direction;
    }

    /// <summary>
    /// <para>GetShooter</para>
    /// <para>射手を設定する。 プレイヤー、ボスはキャラを直接入れる。 それ以外の雑魚敵はキャラから受け取り</para>
    /// </summary>
    private void GetShooter()
    {
        //射手のタイプで処理分け
        switch (_shotMoveData._shooterType)
        {
            case ShotMoveData.ShooterType.Player:       //プレイヤー

                //プレイヤータグのオブジェクトを取得
                GetSetshooter = GameObject.FindGameObjectWithTag(PLAYER_TAG);

                break;

            case ShotMoveData.ShooterType.Boss:         //ボス

                //ボスタグのオブジェクトを取得
                GetSetshooter = GameObject.FindGameObjectWithTag(BOSS_TAG);

                break;

            case ShotMoveData.ShooterType.Common:       //その他雑魚敵

                break;
        }


    }

    /// <summary>
    /// <para>AnimEvent_Disable</para>
    /// <para>無効化処理(アニメーションイベント用)</para>
    /// </summary>
    public void AnimEvent_Disable()
    {
        //自身を無効化
        this.gameObject.SetActive(false);
    }
}