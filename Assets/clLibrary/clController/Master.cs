using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace clController
{
    [DefaultExecutionOrder(0xF)]
    public class Master : MonoBehaviour
    {
        [System.NonSerialized] public GameObject m_gameController = null;
        [System.NonSerialized] public GameObject m_gameFollow = null;
        // 内部以外は読み取り専用にする
        public Controller m_controller { get; private set; }
        public FollowController m_follow { get; private set; }
        public ButtonObj m_button { get; private set; }
        public StickObj m_stick { get; private set; }
        // コントローラーイベントはクラス共通
        public List<ControllerEvent> m_controllerEvents = new List<ControllerEvent>();

        public void Start()
        {
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
                for (int i = 0; i < m_controllerEvents.Count; i++)
                {
                    ControllerEvent e = m_controllerEvents[i];
                    if (m_button.JudgeButton(e.m_buttonName, e.m_buttonMode)) e.m_onEvent.Invoke();
                }
            }
        }
    }
}