using System;
using System.Collections.Generic;
using UnityEngine;
namespace clController
{
    public class RigidPlayerController : ControllerMaster
    {
        [Serializable]
        public struct Property
        {
            public ButtonType m_jumpButton;
            public ButtonMode m_jumpMode;
            public Vector3 m_jumpVector;
            public int m_hoverMaxCount;
            public Vector3 m_hoverVector;
            public Vector3 m_moveToVector;
            public Vector3 m_forceVector;
            public Vector3 m_biasForceVector;
            public Vector3 m_maxVelocity;
            public Vector3 m_biasMaxVelocity;
        }
        public Property m_property = new Property();
        private Rigidbody m_rigid = null;
        private Rigidbody2D m_rigid2 = null;

        public void AddForceWithMove()
        {
            Vector3 forceVector;
            Property ep = m_property;
            forceVector = Vector3.Scale(ep.m_moveToVector, m_controller.m_stick[PosType.Move]);
            if (m_button.JudgeButton(m_property.m_jumpButton, m_property.m_jumpMode))
            {
                if (m_property.m_jumpVector == Vector3.zero) m_property.m_jumpVector = Vector3.up * 500;
                forceVector = m_property.m_jumpVector + forceVector;
            }
            forceVector = Vector3.Scale(ep.m_forceVector, forceVector);
            if (m_rigid != null)
            {
                forceVector = VecComp.LimitForce(forceVector, m_rigid.velocity, ep.m_maxVelocity, ep.m_biasMaxVelocity);
                m_rigid.AddForce(forceVector);
            }
            else if (m_rigid2 != null)
            {
                forceVector = VecComp.LimitForce(forceVector, m_rigid2.velocity, ep.m_maxVelocity, ep.m_biasMaxVelocity);
                m_rigid2.AddForce(forceVector);
            }
        }
        new void Start()
        {
            m_rigid = GetComponent<Rigidbody>();
            m_rigid2 = GetComponent<Rigidbody2D>();
            base.Start();
        }
        new void Update()
        {
            base.Update();
            AddForceWithMove();
        }
    }
}
