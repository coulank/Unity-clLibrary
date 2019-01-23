using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

namespace clController
{
    [DefaultExecutionOrder(0xFFFF)]
    public class FollowController : ControllerMaster
    {
        /// <summary>追従するオブジェクト</summary>
        [SerializeField]
        GameObject followObject;
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
        /// <summary>
        /// 相対座標、ターゲットからの座標位置
        /// </summary>
        public Vector3 RelativeOffset = Vector3.zero;
        protected Quaternion targetRotation = Quaternion.identity;
        protected Quaternion toRotation = Quaternion.identity;
        public float DefaultAngle = 20f;
        public bool DefaultResetRotY = true;
        [System.NonSerialized]
        public ButtonMode resetRotButtonMode = ButtonMode.Down;
        [SerializeField]
        protected bool rotMoving = false;
        public Vector3 BaseAxis = Vector3.up;
        /// 最大の移動角度、最小共に0で制限なし
        /// </summary>
        public float MaxBaseAngle = 90f;
        /// <summary>
        /// 最小の移動角度、最大共に0で制限なし
        /// </summary>
        public float MinBaseAngle = -90f;
        /// <summary>
        public Quaternion BaseRotation { get; protected set; } = Quaternion.identity;
        
        public CinemachineBrain CmBrain { get; protected set; } = null;
        public CinemachineVirtualCamera CmCamera { get; protected set; } = null;
        public string CmName { get; protected set; } = "";

        public float Smooth = 16f;
        /// <summary>
        /// コントローラの使用許可、TrueにするとYや右スティックなどが反映されます
        /// </summary>
        public bool PermissionController = true;
        /// <summary>
        /// 視点リセット時に使用するボタン
        /// </summary>
        public ButtonType ViewForwardButton = ButtonType.Y;
        /// <summary>
        /// カムバックボタン、最初の位置に戻る
        /// </summary>
        public ButtonType RespawnButton = ButtonType.F5;
        public Vector3 RespawnPosition = Vector3.zero;
        public bool SwitchControllFromCameara = true;

        static public bool QuaternionEqual(Quaternion a, Quaternion b, float EqualDot = 0.9999f)
        {
            return (Mathf.Abs(Quaternion.Dot(a, b)) > EqualDot);
        }
        public Quaternion GetForwardRotation(bool resetRotY = true)
        {
            float forwardAngle;
            if (resetRotY)
            {
                forwardAngle = DefaultAngle;
            } else
            {
                forwardAngle = 0f;
            }
            Quaternion forwardRot =
                Quaternion.AngleAxis(forwardAngle, FollowObject.transform.right) *
                FollowObject.transform.rotation;
            return forwardRot;
        }
        public void RotForward(bool orAdd = true, float angle = 2f)
        {
            if (rotMoving || orAdd)
            {
                Quaternion targetRot = FollowObject.transform.rotation;
                if (!QuaternionEqual(targetRotation, targetRot))
                {
                    toRotation = GetForwardRotation(DefaultResetRotY);
                    targetRotation = targetRot;
                }
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * Smooth);
                rotMoving = !QuaternionEqual(toRotation, transform.rotation);
            }

        }
        new void Start()
        {
            base.Start();
            toRotation = GetForwardRotation();
            targetRotation = FollowObject.transform.rotation;
            RespawnPosition = transform.position;
        }
        /// <summary>
        /// サブモジュール、継承してなんやかんやするならここ
        /// </summary>
        protected void SubUpdate()
        {
            if (PermissionController)
            {
                // クォータニオンで角度回転
                Vector3 stick_move = m_stick[PosType.Rot];
                transform.rotation =
                    Quaternion.Lerp(transform.rotation,
                        Quaternion.AngleAxis(stick_move.x, new Vector3(0, 1, 0))
                        * Quaternion.AngleAxis(stick_move.y, transform.right)
                        * transform.rotation, Time.deltaTime * Smooth * 8);
                // 上下90度をキープする、yとzの中間を取る
                transform.rotation = VecComp.RotLimit(transform.rotation, 
                    Vector3.right, MinBaseAngle, MaxBaseAngle);
                // Yボタンで視点リセット
                RotForward(m_button.JudgeButton(ViewForwardButton, resetRotButtonMode));
                // カムバックボタン
                if (m_button.JudgeButton(RespawnButton, ButtonMode.Down)) FollowObject.transform.position = RespawnPosition;
            }
        }
        private void PositionUpdate()
        {
            if (followObject != null)
            {
                transform.position = FollowObject.transform.position + RelativeOffset;
            }
            BaseRotation = VecComp.ExtractionAxis(transform.rotation, BaseAxis);

            if (Camera.main != null) {
                CinemachineBrain cmBrain = Camera.main.GetComponent<CinemachineBrain>();
                if (CmBrain != cmBrain)
                {
                    CmBrain = cmBrain;
                }
            }
            if (CmBrain != null)
            {
                if (CmBrain.ActiveVirtualCamera != null)
                {
                    string cmName = CmBrain.ActiveVirtualCamera.Name;
                    if (cmName != CmName)
                    {
                        CmName = cmName;
                        var findCamera = GameObject.Find(CmName);
                        if (findCamera != null) CmCamera = findCamera.GetComponent<CinemachineVirtualCamera>();
                    }
                }
            }
            else
                CmCamera = null;
            if (CmCamera != null)
            {
                // コントローラのアクティブ化、基本はFollowを参照、LookAtはFollowがNullのときだけ
                if (SwitchControllFromCameara && (m_controller != null))
                {
                    if (CmCamera.m_Follow != null)
                        m_controller.Active = (CmCamera.m_Follow.gameObject == gameObject);
                    else
                    {
                        if (CmCamera.m_Follow != null)
                            m_controller.Active = (CmCamera.m_LookAt.gameObject == gameObject);
                        else
                            m_controller.Active = false;
                    }
                }
            }
        }
        private void OnValidate()
        {
            PositionUpdate();
        }
        new void Update()
        {
            base.Update();
            PositionUpdate();
            SubUpdate();
            
        }
    }
}