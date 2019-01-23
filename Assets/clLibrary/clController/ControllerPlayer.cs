using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace clController
{
    public class ControllerPlayer : ControllerMaster
    {
        [SerializeField] private GameObject m_defaultGameController = null;
        new void Start()
        {
            if (m_defaultGameController != null)
                if (m_gameController == null) m_gameController = m_defaultGameController;
            base.Start();
        }
        new void Update()
        {
            base.Update();
            if (m_button.JudgeButton(ButtonType.A)) Debug.Log("test");
        }
    }

}