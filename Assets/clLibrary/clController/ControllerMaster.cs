using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace clController
{
    [DefaultExecutionOrder(0xF)]
    public class ControllerMaster : MonoBehaviour
    {
        [NonSerialized] public GameObject m_gameController = null;
        // 内部以外は読み取り専用にする
        public Controller m_controller { get; private set; }
        public FollowController m_follow { get; private set; }
        public ButtonObj m_button { get; private set; }
        public StickObj m_stick { get; private set; }
        // コントローラーイベントはクラス共通
        public ControllerEvents m_controllerEvents = new ControllerEvents();

        public void Start()
        {
            m_controllerEvents.ParentObject = gameObject;

            m_gameController = Controller.GetGameMain(m_gameController);
            m_controller = m_gameController.GetComponent<Controller>();
            m_follow = m_gameController.GetComponent<FollowController>();
            Update();
        }
        public void Update()
        {
            if (m_controller != null)
            {
                m_button = m_controller.Button;
                m_stick = m_controller.Stick;
                m_controllerEvents.Update(m_controller);

            }
        }
    }
}