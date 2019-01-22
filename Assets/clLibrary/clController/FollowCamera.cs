using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace clController
{
    /// <summary>
    /// 後になってCinemachineの存在を知ったため、今後はFollowControllerを使います
    /// </summary>
    [DefaultExecutionOrder(0xFFFF)]
    public class FollowCamera : Master
    {
        /// <summary>
        /// コントローラの使用許可、TrueにするとYや右スティックなどが反映されます
        /// </summary>
        public bool PermissionController = true;
        /// <summary>
        /// 強制更新、パラメータの変化を検知して更新します
        /// </summary>
        public bool DoUpdate { get; set; } = false;
        /// <summary>
        /// 自身の座標、もし他の位置を指定したければ他のオブジェクトを置く(継承)
        /// </summary>
        protected GameObject followerObject;
        /// <summary>
        /// 自身のカメラオブジェクト
        /// </summary>
        protected Camera followerCamera;
        /// <summary>追従するオブジェクト</summary>
        [SerializeField]
        GameObject followObject;
        /// <summary>追従するベクター、変化があったときはDoUpdateに発火</summary>
        private Vector3 followVector;
        /// <summary>
        /// 追従したいオブジェクト対象、なければ自身が対象
        /// </summary>
        [SerializeField]
        public GameObject FollowObject
        {
            get
            {
                if (followObject == null)
                    return gameObject;
                else
                    return followObject;
            }
            set
            {
                followObject = value;
            }
        }
        /// <summary>追従先との距離</summary>
        [SerializeField]
        float distance = 10f;
        /// <summary>最大距離</summary>
        public float MaxDistance = 20f;
        /// <summary>最小距離</summary>
        public float MinDistance = 3f;
        /// <summary>距離変化のボリューム</summary>
        public float DistanceVelocity = 4f;
        /// <summary>距離を平面カメラの最大サイズで反映させるか</summary>
        public bool DistanceToSize = true;
        /// <summary>
        /// 追従先との距離、ただし追従先がなければ0を返す
        /// </summary>
        public float Distance
        {
            get
            {
                if (followObject == null)
                    return 0f;
                else
                    return distance;
            }
            set
            {
                if (value > MaxDistance)
                    value = MaxDistance;
                else if (value < MinDistance)
                    value = MinDistance;
                DoUpdate = distance != value;
                distance = value;
            }
        }
        /// <summary>
        /// ベクトルに対しての角度
        /// </summary>
        [SerializeField]
        public Vector3 EulerAngle = Vector3.zero;
        /// <summary>
        /// 相対座標、ターゲットからの座標位置
        /// </summary>
        public Vector3 RelativeOffset = Vector3.zero;
        /// <summary>
        /// デフォルトの角度、リセット時にも適用
        /// </summary>
        [SerializeField]
        public Vector3 DefaultEulerAngle = Vector3.zero;
        /// <summary>
        /// 最大のEulerAngleの値、最小共に0で制限なし
        /// </summary>
        public Vector3 MaxEulerAngle = new Vector3(90f, 0f, 0f);
        /// <summary>
        /// 最小のEulerAngleの値、最大共に0で制限なし
        /// </summary>
        public Vector3 MinEulerAngle = new Vector3(-90f, 0f, 0f);
        /// <summary>
        /// 度数法による角速度、毎秒どのくらいの角度を移動するか
        /// </summary>
        public float AngularVelocityOfDegree = 360f;
        /// <summary>
        /// その他強制更新での更新時間、0.1秒程度
        /// </summary>
        public float SetOtherMaxTimer = 0.1f;
        public bool Auto2D = true;
        [SerializeField]
        protected bool mode2D = false;
        // 画面回転でかける秒数
        const float DEFAULT_TIMER = 0f;
        private float maxTimer = 0f;
        private float countTimer = 0f;
        private Vector3 fromEuler, toEuler;
        private Quaternion fromRotate, toRotate;

        public void SetMode()
        {
            Camera camera = GetComponent<Camera>();
            if (camera != null)
            {
                if (Auto2D)
                    mode2D = camera.orthographic;
            }
        }

        /// <summary>
        /// 対象ベクトルに向かっての配置
        /// </summary>
        public void SeeFollow(Vector3 euler, Vector3 target, float timer = DEFAULT_TIMER)
        {
            if (toEuler != euler || DoUpdate)
            {
                fromEuler = VecComp.AbsCompFloor(followerObject.transform.eulerAngles);
                toEuler = VecComp.EulerShortest(fromEuler, euler);
                if (timer <= 0f)
                {
                    if (AngularVelocityOfDegree != 0f)
                        timer = Vector3.Distance(fromEuler, toEuler) / AngularVelocityOfDegree;
                    else
                        timer = Time.deltaTime;
                }
                countTimer = Time.deltaTime;
                maxTimer = timer;
                if (maxTimer <= 0f) maxTimer = SetOtherMaxTimer;
            }
            if (maxTimer > 0f)
            {
                float ratio = countTimer / maxTimer;
                if (ratio >= 1f)
                {
                    ratio = 1f;
                    maxTimer = 0f;
                }

                Vector3 nextEuler = Vector3.Lerp(fromEuler, toEuler, ratio);
                target += RelativeOffset;
                Transform tf = followerObject.transform;
                if (mode2D) { nextEuler.x = 0; nextEuler.y = 0; }
                tf.eulerAngles = nextEuler;
                Vector3 toVector = target + -tf.forward * Distance;
                if (DistanceToSize && followerCamera.orthographic) followerCamera.orthographicSize = Distance;
                tf.position = toVector;
            }
            DoUpdate = false;
        }
        /// <summary>
        /// 対象のオブジェクトを中心に動くカメラベクトル
        /// </summary>
        public void SeeFollow(Vector3 euler, float timer = DEFAULT_TIMER)
        {
            SeeFollow(euler, FollowObject.transform.position, timer);
        }
        /// <summary>
        /// メンバ変数によって動くカメラベクトル
        /// </summary>
        public void SeeFollow(float timer = DEFAULT_TIMER)
        {
            SeeFollow(EulerAngle, FollowObject.transform.position, timer);
        }
        /// <summary>
        /// 視点リセット、追従オブジェクトの向いている方向を見る
        /// </summary>
        public void SeeForward(float timer = DEFAULT_TIMER)
        {
            Vector3 followForward = Vector3.Scale(FollowObject.transform.forward, new Vector3(1, 0, 1)).normalized;
            EulerAngle = Quaternion.LookRotation(followForward).eulerAngles + DefaultEulerAngle;
            SeeFollow(timer);
        }
        /// <summary>
        /// 視点の範囲制限、0～360で正規化
        /// </summary>
        float SeeLimit(float value, ref float min, ref float max)
        {
            if ((max == 0f) && (min == 0f)) return value;
            if (max < 0) max = VecComp.AbsDeg(max);
            if (min < 0) min = VecComp.AbsDeg(min);
            if (min > max)
            {
                float mid = (min + max) / 2;
                if (max < value && value < min)
                {
                    if (mid >= value)
                    {
                        value = max;
                    } else
                    {
                        value = min;
                    }
                }
            }
            else
            {
                if (max < value) value = max;
                else if (min > value) value = min;
            }
            return value;
        }
        void SeeLimit()
        {
            EulerAngle.x = SeeLimit(EulerAngle.x, ref MinEulerAngle.x, ref MaxEulerAngle.x);
            EulerAngle.y = SeeLimit(EulerAngle.y, ref MinEulerAngle.y, ref MaxEulerAngle.y);
            EulerAngle.z = SeeLimit(EulerAngle.z, ref MinEulerAngle.z, ref MaxEulerAngle.z);
        }
        new void Start()
        {
            if (followerObject == null) followerObject = gameObject;
            followerCamera = GetComponent<Camera>();
            base.Start();
            SetMode();
            SeeForward();
        }
        /// <summary>
        /// サブモジュール、継承してなんやかんやするならここ
        /// </summary>
        protected void SubUpdate()
        {
            if (PermissionController)
            {
                Vector3 stick_move = m_stick[PosType.Rot];
                float comp = 4f;
                if (stick_move != Vector3.zero)
                {
                    Vector3 angle_arrow = new Vector3
                    {
                        x = -stick_move.y * comp,
                        y = stick_move.x * comp
                    };
                    EulerAngle += angle_arrow;
                }
                Distance -= m_controller.Scroll * DistanceVelocity;
                if (m_button.JudgeButton(ButtonType.Y)) { SeeForward(); }
            }
        }
        new void Update()
        {
            base.Update();
            if (maxTimer > 0f) countTimer += Time.deltaTime;
            SubUpdate();
            if (followVector != FollowObject.transform.position) DoUpdate = true;
            followVector = FollowObject.transform.position;
            SeeLimit();
            SeeFollow();
            EulerAngle = VecComp.AbsDeg(EulerAngle);
        }
    }
}