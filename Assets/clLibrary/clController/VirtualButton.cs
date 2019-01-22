using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UclController
{
    public class VirtualButton : Master
    {
        public enum ButtonSwitch
        {
            Normal = 1,
            Toggle = 2,
            Click = 4
        }
        [SerializeField]
        bool pressFlag = false, clickFlag = false, swipeLock = false;
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
        protected Color PressColor = Color.white;
        private Image ImageObj = null;
        private Sprite beforeImage = null;
        private Color beforeColor = Color.white;

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
            entry.callback.AddListener((data) => _vbn._onDown());
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.callback.AddListener((data) => _vbn._onUp());
            entry.eventID = EventTriggerType.PointerUp;
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.callback.AddListener((data) => _vbn._onEnter());
            entry.eventID = EventTriggerType.PointerEnter;
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.callback.AddListener((data) => _vbn._onExit());
            entry.eventID = EventTriggerType.PointerExit;

            entry = new EventTrigger.Entry();
            entry.callback.AddListener((data) => _vbn._onClick());
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
        public void OnDown() { }
        private void _onDown()
        {
            PressFlag ^= true;
            swipeLock = true;
            switch (SwitchMode)
            {
                case ButtonSwitch.Click:
                    clickFlag = true;
                    break;
            }
            OnDown();
        }
        public void OnUp() { }
        private void _onUp()
        {
            swipeLock = false;
            switch (SwitchMode)
            {
                case ButtonSwitch.Toggle:
                    break;
                default:
                    PressFlag = false;
                    clickFlag = false;
                    break;
            }
            OnUp();
        }
        public void OnEnter() { }
        private void _onEnter()
        {
            switch (SwitchMode)
            {
                case ButtonSwitch.Click:
                    if (clickFlag) PressFlag = true;
                    break;
            }
            OnEnter();
        }
        public void OnExit() { }
        private void _onExit()
        {
            switch (SwitchMode)
            {
                case ButtonSwitch.Click:
                    if (clickFlag) PressFlag = false;
                    break;
            }
            OnExit();
        }
        public void OnClick() { }
        private void _onClick()
        {
            switch (SwitchMode)
            {
                case ButtonSwitch.Click:
                    Con.SetVirtualButton(PressButton);
                    PressFlag = false;
                    break;
            }
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
                if (clickFlag)
                    Con.SetVirtualButton(ButtonType.NONE);
                else
                    Con.SetVirtualButton(PressButton, swipeLock);
            }
            else if (swipeLock)
            {
                Con.SetVirtualButton(ButtonType.NONE);
            }
        }
    }
}