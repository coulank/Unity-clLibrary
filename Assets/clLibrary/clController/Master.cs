﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace clController
{
    [DefaultExecutionOrder(0xF)]
    public class Master : MonoBehaviour
    {
        [System.NonSerialized]
        public GameObject m_gameController = null;
        public GameObject m_gameFollow = null;
        // 内部以外は読み取り専用にする
        public MainController m_mainController { get; private set; }
        public FollowController m_mainFollow { get; private set; }
        public Controller m_controller { get; private set; }
        public ButtonObj m_button { get; private set; }
        public StickObj m_stick { get; private set; }

        public void Start()
        {
            m_gameController = MainController.GetGameMain(m_gameController);
            m_mainController = m_gameController.GetComponent<MainController>();
            m_mainFollow = m_gameController.GetComponent<FollowController>();
            Update();
        }
        public void Update()
        {
            m_controller = m_mainController.controller;
            m_button = m_controller.Button;
            m_stick = m_controller.Stick;
        }
    }
}