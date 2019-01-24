using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace clController
{
    [Serializable]
    public class ControllerEvents
    {
        [NonSerialized]
        protected GameObject m_parentObject = null;
        public GameObject ParentObject
        {
            get { return m_parentObject; }
            set
            {
                m_parentObject = value;
            }
        }
        public ControllerEvents(GameObject gameObject = null)
        {
            m_parentObject = gameObject;
        }
        public enum presetType
        {
            MoveForce,
        }
        [Serializable]
        public class ButtonEvent
        {
            [Serializable]
            public struct Property
            {
                public ButtonType m_buttonName;
                public ButtonMode m_buttonMode;
            }
            public Property m_property = new Property();
            public UnityEvent m_onEvent = new UnityEvent();
        }

        public List<ButtonEvent> m_buttonEvents = new List<ButtonEvent>();

        public void Update(Controller con)
        {
            for (int i = 0; i < m_buttonEvents.Count; i++)
            {
                ButtonEvent e = m_buttonEvents[i];
                ButtonEvent.Property ep = e.m_property;
                if (con.Button.JudgeButton(ep.m_buttonName, ep.m_buttonMode)) e.m_onEvent.Invoke();
            }
        }
    }
}
