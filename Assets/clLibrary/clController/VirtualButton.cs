using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace clController
{
    public class VirtualButton : Master
    {
        public enum ButtonSwitch
        {
            Normal = 1,
            Toggle = 2,
            Click = 4
        }
        [System.NonSerialized]
        bool pressFlag = false, clickFlag = false;
        public bool PressFlag
        {
            get { return pressFlag; }
            protected set { pressFlag = value; UpdateDisplay(); }
        }
        [SerializeField]
        protected ButtonType PressButton = ButtonType.NONE;
        [SerializeField]
        protected ButtonSwitch SwitchMode = ButtonSwitch.Normal;
        [SerializeField]
        protected Sprite PressImage = null;
        [SerializeField]
        public Color PressColor = Color.white;
        private Image ImageObj = null;
        private Sprite beforeImage = null;
        private Color beforeColor = Color.white;
        [SerializeField]
        public OnEventClass m_onEvent = new OnEventClass();
        [System.Serializable]
        public class OnEventClass
        {
            public UnityEvent m_onDown = new UnityEvent();
            public UnityEvent m_onUp = new UnityEvent();
            public UnityEvent m_onEnter = new UnityEvent();
            public UnityEvent m_onExit = new UnityEvent();
            public UnityEvent m_onClick = new UnityEvent();
        }

        public void SetUp(object vbn = null)
        {
            if (vbn == null) vbn = this;
            VirtualButton _vbn = (VirtualButton)vbn;
            // まず押しっぱなしと離したときの定義を行う
            EventTrigger trigger = GetComponent<EventTrigger>();
            if (trigger == null) trigger = gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry;

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) => _vbn._onDown((PointerEventData)data));
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.callback.AddListener((data) => _vbn._onUp((PointerEventData)data));
            entry.eventID = EventTriggerType.PointerUp;
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.callback.AddListener((data) => _vbn._onEnter((PointerEventData)data));
            entry.eventID = EventTriggerType.PointerEnter;
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.callback.AddListener((data) => _vbn._onExit((PointerEventData)data));
            entry.eventID = EventTriggerType.PointerExit;

            entry = new EventTrigger.Entry();
            entry.callback.AddListener((data) => _vbn._onClick((PointerEventData)data));
            entry.eventID = EventTriggerType.PointerClick;
            trigger.triggers.Add(entry);
        }

        new void Start()
        {
            base.Start();
            SetUp();
        }
        protected void Start(object vbn = null)
        {
            base.Start();
            SetUp(vbn);
        }
        public static int GetFingerFromData(Vector2 position)
        {
            if (Input.touchSupported)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch t = Input.touches[i];
                    if (position == t.position) return t.fingerId;
                }
            }
            return -1;
        }
        public void OnDown() { }
        private void _onDown(PointerEventData data)
        {
            m_controller.SetFingerLock(GetFingerFromData(data.position));
            PressFlag ^= true;
            switch (SwitchMode)
            {
                case ButtonSwitch.Click:
                    clickFlag = true;
                    break;
            }
            m_onEvent.m_onDown.Invoke();
            OnDown();
        }
        public void OnUp() { }
        private void _onUp(PointerEventData data)
        {
            m_controller.SetFingerUnLock(GetFingerFromData(data.position));
            switch (SwitchMode)
            {
                case ButtonSwitch.Toggle:
                    break;
                default:
                    PressFlag = false;
                    clickFlag = false;
                    break;
            }
            m_onEvent.m_onUp.Invoke();
            OnUp();
        }
        public void OnEnter() { }
        private void _onEnter(PointerEventData data)
        {
            switch (SwitchMode)
            {
                case ButtonSwitch.Click:
                    if (clickFlag) PressFlag = true;
                    break;
            }
            m_onEvent.m_onEnter.Invoke();
            OnEnter();
        }
        public void OnExit() { }
        private void _onExit(PointerEventData data)
        {
            switch (SwitchMode)
            {
                case ButtonSwitch.Click:
                    if (clickFlag) PressFlag = false;
                    break;
            }
            m_onEvent.m_onExit.Invoke();
            OnExit();
        }
        public void OnClick() { }
        private void _onClick(PointerEventData data)
        {
            switch (SwitchMode)
            {
                case ButtonSwitch.Click:
                    m_controller.SetVirtualButton(PressButton);
                    PressFlag = false;
                    break;
            }
            m_onEvent.m_onClick.Invoke();
            OnClick();
        }
        public void UpdateDisplay()
        {
            ImageObj = GetComponent<Image>();
            if (ImageObj != null)
            {
                if (pressFlag)
                {
                    beforeImage = ImageObj.sprite;
                    beforeColor = ImageObj.color;
                    if (PressImage != null) ImageObj.sprite = PressImage;
                    ImageObj.color = PressColor;
                }
                else
                {
                    ImageObj.sprite = beforeImage;
                    ImageObj.color = beforeColor;

                }
            }
        }
        new void Update()
        {
            base.Update();
            if (PressFlag)
            {
                if (!clickFlag) m_controller.SetVirtualButton(PressButton);
            }
        }
    }
}