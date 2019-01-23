/// <summary>
/// コントローラーなどのデバイスを管理するクラス
/// 必ずStart関数以降の実行にすること
/// 依存関係はComp.csとVecComp.csは必須
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

namespace clController
{
    public enum ButtonMode
    {
        Press = 1,
        Repeat = 2,
        Down = 4,
        Up = 8,
        Delay = 16,
        DelayRepeat = Delay | Repeat,
        DelayDown = Delay | Down,
        DelayUp = Delay | Up,
        Double = 32,
        Click = 64,
        DoubleClick = Double | Down,
        DoubleUp = Double | Up,
        None = 0
    }
    /// <summary>Set ConType</summary>
    public enum ConType
    {
        Default = 0,
        Xinput = 1,
        Direct = 2,
        Switch = 3,
        Android = 4,
        Other = 255
    }
    /// <summary>KeyInput or JoyAxis</summary>
    public enum KeyType
    {
        Key = 1,
        JoyKey = 2,
        Axis = 4,
        JoyAxis = 8,
        Other = 0
    }
    /// <summary>
    /// 方向と動かすのに使うベクトルが入っている
    /// </summary>
    public enum PosType
    {
        Left = 1,
        Right = 2,
        Center = 4,
        Move = 16,
        Rot = 32,
        None = 0
    }
    public enum PointType
    {
        X = 1,
        Y = 2,
        Z = 4
    }
    public enum ButtonType
    {
        UP = 0x1,
        DOWN = 0x2,
        LEFT = 0x4,
        RIGHT = 0x8,
        A = 0x10,
        B = 0x20,
        X = 0x40,
        Y = 0x80,
        L = 0x100,
        R = 0x200,
        ZL = 0x400,
        ZR = 0x800,
        PLUS = 0x1000,
        MINUS = 0x2000,
        STARTMENU = 0x3000,
        PUSHSL = 0x4000,
        PUSHSR = 0x8000,
        PUSHSTICK = 0xC000,
        R_UP = 0x10000,
        R_DOWN = 0x20000,
        R_LEFT = 0x40000,
        R_RIGHT = 0x80000,
        CTRL = 0x100000,
        ALT = 0x200000,
        SHIFT = 0x400000,
        F1 = 0x1000000,
        F2 = 0x2000000,
        F3 = 0x3000000,
        F4 = 0x4000000,
        F5 = 0x5000000,
        F6 = 0x6000000,
        F7 = 0x7000000,
        F8 = 0x8000000,
        F9 = 0x9000000,
        F10 = 0xA000000,
        F11 = 0xB000000,
        F12 = 0xC000000,
        PRSCR = 0xD000000,
        HOME = 0xE000000,
        ESC = 0xF000000,
        ARROW = UP | DOWN | LEFT | RIGHT,
        R_ARROW = R_UP | R_DOWN | R_LEFT | R_RIGHT,
        ANY_ARROW = ARROW | R_ARROW,
        NONE = 0,
        ANY = ~0,
        BITJADGE = 0xFFFFFF
    }

    /// <summary>
    /// ボタンクラス、ビット形式にしたものをDictionary配列に格納する
    /// </summary>
    public class ButtonObj : Dictionary<ButtonMode, ButtonType>
    {
        // Key Controler
        public ButtonObj()
        {
            foreach (ButtonMode Type in Enum.GetValues(typeof(ButtonMode))) Add(Type, 0);
        }

        /// <summary>調べたいボタンを調べる</summary>
        /// <param name="judge_button">調べたいボタンの値</param>
        /// <param name="press_button">現在のボタンの値</param>
        /// <param name="AndMode">Trueなら調べたいボタンが全て押されていればTrueを返し、
        /// Falseならどれか一つだけでも押されていればTrueを返す</param>
        static public bool JudgeButton(ButtonType judge_button, ButtonType press_button, bool AndMode = false)
        {
            if (judge_button == ButtonType.NONE) return false;
            // まずBit判定とEqual判定に分離する
            ButtonType judge_high = judge_button & ~ButtonType.BITJADGE;
            judge_button &= ButtonType.BITJADGE;
            ButtonType press_high = press_button & ~ButtonType.BITJADGE;
            press_button &= ButtonType.BITJADGE;
            // Equal判定
            bool judge = AndMode;
            if (judge_high != ButtonType.NONE) judge = (judge_high == press_high);

            // Bit判定を行って、Equalと結合する
            if (AndMode)
            {
                return judge && ((press_button & judge_button) == judge_button);
            }
            else
            {
                return judge || ((press_button & judge_button) != ButtonType.NONE);
            }
        }
        /// <summary>調べたいボタンを調べる</summary>
        /// <param name="judge_button">調べたいボタンの値</param>
        /// <param name="btype">調べたいボタンのモード</param>
        /// <param name="AndMode">Trueなら調べたいボタンが全て押されていればTrueを返し、
        /// Falseならどれか一つだけでも押されていればTrueを返す</param>
        public bool JudgeButton(ButtonType judge_button, ButtonMode btype = ButtonMode.Down, bool AndMode = false)
        {
            return JudgeButton(judge_button, this[btype], AndMode);
        }
        /// <summary>押しっぱなしのときに発生</summary>
        public ButtonType Press { get { return this[ButtonMode.Press]; } }
        /// <summary>押しっぱなしのときに断続的に発生</summary>
        public ButtonType Repeat { get { return this[ButtonMode.Repeat]; } }
        /// <summary>押した瞬間だけ発生</summary>
        public ButtonType Down { get { return this[ButtonMode.Down]; } }
        /// <summary>離した瞬間だけ発生</summary>
        public ButtonType Up { get { return this[ButtonMode.Up]; } }
        /// <summary>長押し + 押しっぱなしのときに発生</summary>
        public ButtonType Delay { get { return this[ButtonMode.Press]; } }
        /// <summary>長押し + 押しっぱなしのときに断続的に発生</summary>
        public ButtonType DelayRepeat { get { return this[ButtonMode.Repeat]; } }
        /// <summary>長押し + 押した瞬間だけ発生</summary>
        public ButtonType DelayDown { get { return this[ButtonMode.Down]; } }
        /// <summary>長押し + 離した瞬間だけ発生</summary>
        public ButtonType DelayUp { get { return this[ButtonMode.Up]; } }
        /// <summary>ダブルクリック + 押し続けているときに発生</summary>
        public ButtonType Double { get { return this[ButtonMode.Double]; } }
        /// <summary>ダブルクリック + 押した瞬間に発生</summary>
        public ButtonType DoubleClick { get { return this[ButtonMode.DoubleClick]; } }
        /// <summary>ダブルクリック + 離した瞬間に発生</summary>
        public ButtonType DoubleDown { get { return this[ButtonMode.DoubleUp]; } }

        /// <summary>押されたボタンの一覧を文字列で出力する</summary>
        static public string ResultButton(ButtonType btn)
        {
            bool boolOutput;
            ButtonType highbtn = btn & ~ButtonType.BITJADGE;
            btn &= ButtonType.BITJADGE;
            List<string> OutputStrList = new List<string>();
            foreach (ButtonType ibtn in Controller.BTNNUMLIST)
            {
                if ((ibtn & ButtonType.BITJADGE) != ButtonType.NONE)
                {
                    boolOutput = ((btn & ibtn) != ButtonType.NONE);
                }
                else
                {
                    boolOutput = (highbtn == ibtn);
                }
                if (boolOutput) OutputStrList.Add(Controller.BTNSTRDIC[ibtn]);
            }
            return string.Join(",", OutputStrList);
        }

        public string ToString(ButtonMode buttonMode = ButtonMode.Press)
        {
            return ResultButton(this[buttonMode]);
        }
    }
    /// <summary>スティック入力クラス</summary>
    public class StickObj : Dictionary<PosType, Vector2>
    {
        public void SetMost(PosType pos, Vector2 addvc2, float magnitude = 1f)
        {
            if (ContainsKey(pos))
            {
                this[pos] = VecComp.AbsMax(this[pos], addvc2) * magnitude;
            }
        }
        public void PosAdd(PosType pos, Vector2 vc2 = new Vector2())
        {
            Add(pos, new Vector2());
        }
        /// <summary>左スティック</summary>
        public Vector2 Left { get { return this[PosType.Left]; } }
        /// <summary>右スティック</summary>
        public Vector2 Right { get { return this[PosType.Right]; } }
        /// <summary>ホイールボタンスクロール</summary>
        public Vector2 Center { get { return this[PosType.Center]; } }
        /// <summary>左スティックなどから移動に定義されたベクトル</summary>
        public Vector2 Move { get { return this[PosType.Move]; } }
        /// <summary>右スティックなどからカメラに定義されたベクトル</summary>
        public Vector2 Rot { get { return this[PosType.Rot]; } }
        public void PosClear()
        {
            Vector2 vc2;
            foreach (PosType key in new List<PosType>(Keys))
            {
                vc2 = this[key]; vc2.Set(0f, 0f); this[key] = vc2;
            }
        }

        public StickObj()
        {
            PosAdd(PosType.Left);
            PosAdd(PosType.Right);
            PosAdd(PosType.Center);
            PosAdd(PosType.Move);
            PosAdd(PosType.Rot);
        }
    }

    [Serializable]
    public class ControllerEvent
    {
        public ButtonType m_buttonName = ButtonType.NONE;
        public ButtonMode m_buttonMode = ButtonMode.Press;
        public UnityEvent m_onEvent = new UnityEvent();
    }

    public class ConTemplate
    {
        public KeyType keytype;
        public string keyname;
        public ButtonType button;
        public bool reverse;
        public float dead;
        public PosType posType;
        public PointType pntType;
        public float num = 0;
        public KeyCode key = KeyCode.Space;

        public ConTemplate(KeyType _keytype = KeyType.Key, ButtonType _button = 0, float _num = 1, string _keyname = "", bool _reverse = false, float _dead = 0.1f,
            PosType _pos = PosType.Left, PointType _pnt = PointType.X)
        {
            keytype = _keytype;
            if (_keyname == "")
            {
                switch (keytype)
                {
                    case KeyType.JoyKey:
                        _keyname = "button " + _num.ToString();
                        break;
                    case KeyType.Axis:
                    case KeyType.JoyAxis:
                        _keyname = "axis " + _num.ToString();
                        break;
                    case KeyType.Key:
                        key = (KeyCode)_num;
                        break;
                    default:
                        break;
                }
            }
            num = _num;
            keyname = _keyname;
            button = _button;
            reverse = _reverse;
            dead = _dead;
            posType = _pos;
            pntType = _pnt;
        }
    }


    /// <summary>
    /// コントローラーのボタンやスティックなどの単体オブジェクト
    /// JoyAxisはjoystick axis 1 ～ joystick axis 10 を取得するように固定しています
    /// よってJoystickAxis.prisetで動作します。
    /// コントローラー親を第一引数にする
    /// </summary>
    public class ConObj : ConTemplate
    {
        public static List<string> joysticks = new List<string>
    {
        "joystick ", "joystick1 ", "joystick2 ", "joystick3 ", "joystick4 "
    };
        public Controller parent;

        public ConObj(Controller _controller, KeyType _keytype = KeyType.Key, ButtonType _button = 0, float _num = 1, string _keyname = "", bool _reverse = false, float _dead = 0.1f,
            PosType _pos = PosType.Left, PointType _pnt = PointType.X)
            : base(_keytype, _button, _num, _keyname, _reverse, _dead, _pos, _pnt)
        {
            parent = _controller;
        }
        public ConObj(Controller _controller, ConTemplate _contemp)
            : this(_controller, _contemp.keytype, _contemp.button, 0, _contemp.keyname, _contemp.reverse, _contemp.dead, _contemp.posType, _contemp.pntType) { }

        public ButtonType UpdateButton(bool KeyToCon)
        {
            ButtonType retval = 0;
            float axisval = 0f;
            bool keypush = false;
            Vector3 vc3;
            switch (keytype)
            {
                case KeyType.Axis:
                case KeyType.JoyAxis:
                    if (parent.JoystickID < 0) break;
                    axisval = Input.GetAxisRaw(((keytype == KeyType.JoyAxis) ? joysticks[parent.JoystickID] : "") + keyname);
                    if (reverse) axisval *= -1;
                    PosType LocalPosType;
                    if ((posType & PosType.Left) == PosType.Left)
                    {
                        LocalPosType = PosType.Left;
                    }
                    else if ((posType & PosType.Right) == PosType.Right)
                    {
                        LocalPosType = PosType.Right;
                    }
                    else
                    {
                        LocalPosType = PosType.Center;
                    }
                    // 座標が最大じゃないコントローラー向けに補正
                    if (parent.Property.CompFlag) axisval = Controller.CompObj.DoComp(axisval, parent.Sys_ConType, LocalPosType, pntType);
                    if (axisval > dead) retval = button;
                    vc3 = parent.Stick[LocalPosType];
                    switch (pntType)
                    {
                        case PointType.X:
                            vc3.x = axisval;
                            break;
                        case PointType.Y:
                            vc3.y = axisval;
                            break;
                        case PointType.Z:
                            vc3.z = axisval;
                            break;
                    }

                    parent.Stick.SetMost(posType, vc3);;
                    if ((posType & PosType.Move) == PosType.Move) parent.Stick.SetMost(PosType.Move, vc3);
                    if ((posType & PosType.Rot) == PosType.Rot) parent.Stick.SetMost(PosType.Rot, vc3);
                    break;
                case KeyType.Key:
                    if (KeyToCon)
                    {
                        if ((parent.JoystickID > 0) && !parent.Property.MultPlayKeyboard) break;
                        if (keyname != "")
                            keypush = Input.GetKey(keyname);
                        else
                            keypush = Input.GetKey(key);
                        if (keypush) retval = button;
                    }
                    break;
                case KeyType.JoyKey:
                    if (parent.JoystickID < 0) break;
                    keypush = Input.GetKey(joysticks[parent.JoystickID] + keyname);
                    if (keypush) retval = button;
                    break;
                default:
                    if (keyname != "")
                        keypush = Input.GetKey(keyname);
                    else
                        keypush = Input.GetKey(key);
                    if (keypush) retval = button;
                    break;
            }
            return retval;
        }
    }

    [Serializable]
    /// <summary>リピートクラス、単位は秒、小数点以下で指定すること</summary>
    public class KeyRepeatClass
    {
        public bool lock_enable = false;
        public bool enable = false;
        public bool started = false;
        public bool first = false;
        public bool last = false;
        public float pushing;
        public float lock_start;
        public float start;
        public float interval;
        public bool double_enable = false;
        public bool double_press = false;
        public float double_time;
        public float double_latency;
        public KeyRepeatClass(float _lock_start = 0f, float _start = 0.4f, 
            float _interval = 0.2f, float _double_latency = 1f)
        {
            lock_start = _lock_start; start = _start; interval = _interval; double_latency = _double_latency;
            double_time = double_latency + 1;
        }
        public bool Check(bool press)
        {
            bool RetBool = false;
            first = false; last = false;
            if (double_time < double_latency) double_time += Time.deltaTime;
            if (press)
            {
                if (enable)
                {
                    pushing += Time.deltaTime;
                    if (started)
                    {
                        if ((interval > 0f) && (pushing > interval))
                        {
                            RetBool = true; // ButtonMode.Repeat
                            pushing = 0f;
                        }
                    }
                    else
                    {
                        if ((start > 0f) && (pushing > start))
                        {
                            RetBool = true;
                            started = true;
                            pushing = 0f;
                        }
                    }
                }
                else
                {
                    if (lock_enable)
                    {
                        pushing += Time.deltaTime;
                    }
                    else
                    {
                        lock_enable = true;
                        pushing = 0f;
                    }
                    if (pushing >= lock_start)
                    {
                        first = true;       // ButtonMode.Down
                        RetBool = true;
                        enable = true;
                        started = false;
                        double_press = (double_time <= double_latency);
                        if (double_press)
                            double_time = double_latency + 1f;
                        else
                            double_time = 0f;
                    }
                }

            }
            else
            {
                if (enable)
                {
                    if (double_time <= double_latency)
                        double_time = 0f;
                    else
                        double_time = double_latency + 1f;
                    last = true;    // ButtonMode.Up
                    started = false;
                    enable = false;
                    lock_enable = false;
                } else
                {
                    double_press = false;
                }
            }
            return RetBool;
        }
    }

    /// <summary>キーリピート配列を予め生成する</summary>
    public class KeyRepeatDict : Dictionary<ButtonType, KeyRepeatClass>
    {
        public KeyRepeatDict(float lock_start = 0f)
        {
            foreach (ButtonType con in Controller.BTNNUMLIST)
            {
                Add(con, new KeyRepeatClass(lock_start));
            }
        }
    }

    public class TouchPhaseCount : Dictionary<TouchPhase, int>
    {
        public static int PhaseCount(Touch[] touches, TouchPhase touchPhase, bool notequal = false)
        {
            int count = 0;
            foreach (Touch touch in touches)
            {
                if (notequal ^ (touch.phase == touchPhase)) count++;
            }
            return count;
        }
        public static int GetNotEndedCount(Touch[] touches)
        {
            int notEnded = PhaseCount(touches, TouchPhase.Ended, true);
            notEnded -= PhaseCount(touches, TouchPhase.Canceled);
            return notEnded;
        }
        public TouchPhaseCount() { }
        public TouchPhaseCount(Touch[] touches)
        {
            foreach (TouchPhase touchPhase in Enum.GetValues(typeof(TouchPhase))) {
                Add(touchPhase, PhaseCount(touches, touchPhase));
            }
        }
    }

    /// <summary>コントローラー設定のテンプレートクラス</summary>
    public class ConTempSet : List<ConTemplate>
    {
        /// <summary>テンプレート配列から生成する、システム用</summary>
        public static List<ConObj> OutUseList(Controller parentCon, ConTempSet templates)
        {
            List<ConObj> conList = new List<ConObj>();
            foreach (ConTemplate template in templates)
            {
                conList.Add(new ConObj(parentCon, template));
            }
            return conList;
        }
        /// <summary>コピペ用</summary>
        public static ConTempSet Template = new ConTempSet()
    {
        new ConTemplate(),
    };

        /// <summary>最初に読み込むユーザ設定</summary>
        public static ConTempSet UserFirstTemplate = new ConTempSet();
        /// <summary>最後に読み込むユーザ設定、こちらは優先度高い</summary>
        public static ConTempSet UserLastTemplate = new ConTempSet();

        /// <summary>コントローラの共通登録</summary>
        public static ConTempSet CommonTemplate = new ConTempSet()
    {
        new ConTemplate(KeyType.JoyKey, ButtonType.L, 4),
        new ConTemplate(KeyType.JoyKey, ButtonType.R, 5),
    };
        /// <summary>キーボードのシステム部分登録</summary>
        public static ConTempSet SysTemplate = new ConTempSet()
    {
        new ConTemplate(KeyType.Key, ButtonType.UP, 0, "up"),
        new ConTemplate(KeyType.Key, ButtonType.DOWN, 0, "down"),
        new ConTemplate(KeyType.Key, ButtonType.LEFT, 0, "left"),
        new ConTemplate(KeyType.Key, ButtonType.RIGHT, 0, "right"),

        new ConTemplate(KeyType.Key, ButtonType.A, 0, "return"),
        new ConTemplate(KeyType.Key, ButtonType.A, 0, "enter"),
        new ConTemplate(KeyType.Key, ButtonType.A, 0, "space"),
        new ConTemplate(KeyType.Key, ButtonType.B, 0, "backspace"),
        new ConTemplate(KeyType.Key, ButtonType.ESC, 0, "escape"),
        new ConTemplate(KeyType.Key, ButtonType.PRSCR, (float)KeyCode.Print),

        new ConTemplate(KeyType.Axis, ButtonType.CTRL, 0, "ctrl"),
        new ConTemplate(KeyType.Axis, ButtonType.ALT, 0, "alt"),
        new ConTemplate(KeyType.Axis, ButtonType.SHIFT, 0, "shift"),

        new ConTemplate(KeyType.Key, ButtonType.F1, 0, "f1"),
        new ConTemplate(KeyType.Key, ButtonType.F2, 0, "f2"),
        new ConTemplate(KeyType.Key, ButtonType.F3, 0, "f3"),
        new ConTemplate(KeyType.Key, ButtonType.F4, 0, "f4"),
        new ConTemplate(KeyType.Key, ButtonType.F5, 0, "f5"),
        new ConTemplate(KeyType.Key, ButtonType.F6, 0, "f6"),
        new ConTemplate(KeyType.Key, ButtonType.F7, 0, "f7"),
        new ConTemplate(KeyType.Key, ButtonType.F8, 0, "f8"),
        new ConTemplate(KeyType.Key, ButtonType.F9, 0, "f9"),
        new ConTemplate(KeyType.Key, ButtonType.F10, 0, "f10"),
        new ConTemplate(KeyType.Key, ButtonType.F11, 0, "f11"),
        new ConTemplate(KeyType.Key, ButtonType.F12, 0, "f12"),
    };

        /// <summary>キーボード登録</summary>
        public static ConTempSet KeyboardTemplate = new ConTempSet()
    {
        new ConTemplate(KeyType.Key, ButtonType.UP, 0, "w"),
        new ConTemplate(KeyType.Key, ButtonType.DOWN, 0, "s"),
        new ConTemplate(KeyType.Key, ButtonType.LEFT, 0, "a"),
        new ConTemplate(KeyType.Key, ButtonType.RIGHT, 0, "d"),

        new ConTemplate(KeyType.Key, ButtonType.R_UP, 0, "i"),
        new ConTemplate(KeyType.Key, ButtonType.R_DOWN, 0, "k"),
        new ConTemplate(KeyType.Key, ButtonType.R_LEFT, 0, "j"),
        new ConTemplate(KeyType.Key, ButtonType.R_RIGHT, 0, "l"),

        new ConTemplate(KeyType.Key, ButtonType.A, 0, "z"),
        new ConTemplate(KeyType.Key, ButtonType.B, 0, "x"),
        new ConTemplate(KeyType.Key, ButtonType.X, 0, "c"),
        new ConTemplate(KeyType.Key, ButtonType.Y, 0, "v"),
        new ConTemplate(KeyType.Key, ButtonType.L, 0, "q"),
        new ConTemplate(KeyType.Key, ButtonType.R, 0, "e"),
        new ConTemplate(KeyType.Key, ButtonType.ZL, 0, "1"),
        new ConTemplate(KeyType.Key, ButtonType.ZR, 0, "4"),
        new ConTemplate(KeyType.Key, ButtonType.PLUS, 0, "2"),
        new ConTemplate(KeyType.Key, ButtonType.MINUS, 0, "3"),
        new ConTemplate(KeyType.Key, ButtonType.PUSHSL, 0, "b"),
        new ConTemplate(KeyType.Key, ButtonType.PUSHSR, 0, "n"),
    };

        /// <summary>Xinput用のテンプレ、読み込み専用</summary>
        public static ConTempSet XinputTemplate
        {
            get
            {
                return new ConTempSet()
            {
                new ConTemplate(KeyType.JoyKey, ButtonType.B, 0),
                new ConTemplate(KeyType.JoyKey, ButtonType.A, 1),
                new ConTemplate(KeyType.JoyKey, ButtonType.Y, 2),
                new ConTemplate(KeyType.JoyKey, ButtonType.X, 3),
                new ConTemplate(KeyType.JoyAxis, ButtonType.UP, 7, _reverse: false, _pos: PosType.Move, _pnt: PointType.Y),
                new ConTemplate(KeyType.JoyAxis, ButtonType.DOWN, 7, _reverse: true, _pos: PosType.Move, _pnt: PointType.Y),
                new ConTemplate(KeyType.JoyAxis, ButtonType.RIGHT, 6, _reverse: false, _pos: PosType.Move, _pnt: PointType.X),
                new ConTemplate(KeyType.JoyAxis, ButtonType.LEFT, 6, _reverse: true, _pos: PosType.Move, _pnt: PointType.X),
                new ConTemplate(KeyType.JoyAxis, ButtonType.ZL, 9, _reverse: false),
                new ConTemplate(KeyType.JoyAxis, ButtonType.ZR, 10, _reverse: false),
                new ConTemplate(KeyType.JoyKey, ButtonType.MINUS, 6),
                new ConTemplate(KeyType.JoyKey, ButtonType.PLUS, 7),
                new ConTemplate(KeyType.JoyKey, ButtonType.PUSHSL, 8),
                new ConTemplate(KeyType.JoyKey, ButtonType.PUSHSR, 9),
                new ConTemplate(KeyType.JoyAxis, _num: 1, _pos: PosType.Left | PosType.Move, _pnt: PointType.X),
                new ConTemplate(KeyType.JoyAxis, _num: 2, _pos: PosType.Left | PosType.Move, _pnt: PointType.Y, _reverse: Controller.Yreverse),
                new ConTemplate(KeyType.JoyAxis, _num: 4, _pos: PosType.Right | PosType.Rot, _pnt: PointType.X),
                new ConTemplate(KeyType.JoyAxis, _num: 5, _pos: PosType.Right | PosType.Rot, _pnt: PointType.Y, _reverse: Controller.Yreverse),
            };
            }
        }

        /// <summary>DirectInput用テンプレ、読み込み専用</summary>
        public static ConTempSet DirectTemplate
        {
            get
            {
                return new ConTempSet()
            {
        new ConTemplate(KeyType.JoyKey, ButtonType.Y, 0),
        new ConTemplate(KeyType.JoyKey, ButtonType.B, 1),
        new ConTemplate(KeyType.JoyKey, ButtonType.A, 2),
        new ConTemplate(KeyType.JoyKey, ButtonType.X, 3),
        new ConTemplate(KeyType.JoyAxis, ButtonType.UP, 6, _reverse: false, _pos: PosType.Move, _pnt: PointType.Y),
        new ConTemplate(KeyType.JoyAxis, ButtonType.DOWN, 6, _reverse: true, _pos: PosType.Move, _pnt: PointType.Y),
        new ConTemplate(KeyType.JoyAxis, ButtonType.RIGHT, 5, _reverse: false, _pos: PosType.Move, _pnt: PointType.X),
        new ConTemplate(KeyType.JoyAxis, ButtonType.LEFT, 5, _reverse: true, _pos: PosType.Move, _pnt: PointType.X),
        new ConTemplate(KeyType.JoyKey, ButtonType.ZL, 6),
        new ConTemplate(KeyType.JoyKey, ButtonType.ZR, 7),
        new ConTemplate(KeyType.JoyKey, ButtonType.MINUS, 8),
        new ConTemplate(KeyType.JoyKey, ButtonType.PLUS, 9),
        new ConTemplate(KeyType.JoyKey, ButtonType.PUSHSL, 10),
        new ConTemplate(KeyType.JoyKey, ButtonType.PUSHSR, 11),
        new ConTemplate(KeyType.JoyAxis, _num: 3, _pos: PosType.Left | PosType.Move, _pnt: PointType.X),
        new ConTemplate(KeyType.JoyAxis, _num: 4, _pos: PosType.Left | PosType.Move, _pnt: PointType.Y, _reverse: Controller.Yreverse),
        new ConTemplate(KeyType.JoyAxis, _num: 1, _pos: PosType.Right | PosType.Rot, _pnt: PointType.X),
        new ConTemplate(KeyType.JoyAxis, _num: 2, _pos: PosType.Right | PosType.Rot, _pnt: PointType.Y, _reverse: Controller.Yreverse),
            };
            }
        }

        /// <summary>Switchのコントローラ用テンプレ、読み込み専用</summary>
        public static ConTempSet SwitchTemplate
        {
            get
            {
                return new ConTempSet()
            {
                new ConTemplate(KeyType.JoyKey, ButtonType.B, 0),
                new ConTemplate(KeyType.JoyKey, ButtonType.A, 1),
                new ConTemplate(KeyType.JoyKey, ButtonType.Y, 2),
                new ConTemplate(KeyType.JoyKey, ButtonType.X, 3),
                new ConTemplate(KeyType.JoyAxis, ButtonType.UP, 10, _reverse: false, _pos: PosType.Move, _pnt: PointType.Y),
                new ConTemplate(KeyType.JoyAxis, ButtonType.DOWN, 10, _reverse: true, _pos: PosType.Move, _pnt: PointType.Y),
                new ConTemplate(KeyType.JoyAxis, ButtonType.RIGHT, 9, _reverse: false, _pos: PosType.Move, _pnt: PointType.X),
                new ConTemplate(KeyType.JoyAxis, ButtonType.LEFT, 9, _reverse: true, _pos: PosType.Move, _pnt: PointType.X),
                new ConTemplate(KeyType.JoyKey, ButtonType.ZL, 6),
                new ConTemplate(KeyType.JoyKey, ButtonType.ZR, 7),
                new ConTemplate(KeyType.JoyKey, ButtonType.ZL, 14),
                new ConTemplate(KeyType.JoyKey, ButtonType.ZR, 15),
                new ConTemplate(KeyType.JoyKey, ButtonType.MINUS, 8),
                new ConTemplate(KeyType.JoyKey, ButtonType.PLUS, 9),
                new ConTemplate(KeyType.JoyKey, ButtonType.PUSHSL, 10),
                new ConTemplate(KeyType.JoyKey, ButtonType.PUSHSR, 11),
                new ConTemplate(KeyType.JoyKey, ButtonType.HOME, 12),
                new ConTemplate(KeyType.JoyKey, ButtonType.PRSCR, 13),
                new ConTemplate(KeyType.JoyAxis, _num: 2, _pos: PosType.Left | PosType.Move, _pnt: PointType.X),
                new ConTemplate(KeyType.JoyAxis, _num: 4, _pos: PosType.Left | PosType.Move, _pnt: PointType.Y, _reverse: Controller.Yreverse),
                new ConTemplate(KeyType.JoyAxis, _num: 7, _pos: PosType.Right | PosType.Rot, _pnt: PointType.X),
                new ConTemplate(KeyType.JoyAxis, _num: 8, _pos: PosType.Right | PosType.Rot, _pnt: PointType.Y, _reverse: Controller.Yreverse),
            };
            }
        }
        public static ConTempSet AndroidTemplate
        {
            get
            {
                return new ConTempSet()
                {
        new ConTemplate(KeyType.JoyKey, ButtonType.B, 0),
        new ConTemplate(KeyType.JoyKey, ButtonType.A, 1),
        new ConTemplate(KeyType.JoyKey, ButtonType.Y, 2),
        new ConTemplate(KeyType.JoyKey, ButtonType.X, 3),
        new ConTemplate(KeyType.JoyAxis, ButtonType.UP, 6, _reverse: true, _pos: PosType.Move, _pnt: PointType.Y),
        new ConTemplate(KeyType.JoyAxis, ButtonType.DOWN, 6, _reverse: false, _pos: PosType.Move, _pnt: PointType.Y),
        new ConTemplate(KeyType.JoyAxis, ButtonType.RIGHT, 5, _reverse: false, _pos: PosType.Move, _pnt: PointType.X),
        new ConTemplate(KeyType.JoyAxis, ButtonType.LEFT, 5, _reverse: true, _pos: PosType.Move, _pnt: PointType.X),
        new ConTemplate(KeyType.JoyKey, ButtonType.PLUS, 10),
        new ConTemplate(KeyType.JoyKey, ButtonType.PUSHSL, 8),
        new ConTemplate(KeyType.JoyKey, ButtonType.PUSHSR, 9),
        new ConTemplate(KeyType.JoyAxis, _num: 1, _pos: PosType.Left | PosType.Move, _pnt: PointType.X),
        new ConTemplate(KeyType.JoyAxis, _num: 2, _pos: PosType.Left | PosType.Move, _pnt: PointType.Y, _reverse: Controller.Yreverse),
        new ConTemplate(KeyType.JoyAxis, _num: 3, _pos: PosType.Right | PosType.Rot, _pnt: PointType.X),
        new ConTemplate(KeyType.JoyAxis, _num: 4, _pos: PosType.Right | PosType.Rot, _pnt: PointType.Y, _reverse: Controller.Yreverse),
                };
            }
        }
    }

    public class Controller : MonoBehaviour
    {
        public List<ConObj> m_controller = new List<ConObj>();
        public List<ControllerEvent> m_globalControllerEvents = new List<ControllerEvent>();

        static protected List<ButtonType> GetListButtonType()
        {
            var list = new List<ButtonType>();
            int bitjadge = (int)ButtonType.BITJADGE;
            foreach (ButtonType value in Enum.GetValues(typeof(ButtonType)))
            {
                switch (value)
                {
                    case ButtonType.BITJADGE:
                    case ButtonType.ANY:
                        continue;
                }
                if ((value & ~ButtonType.BITJADGE) == 0)
                {

                    if ((bitjadge & (int)value) != 0)
                    {
                        list.Add(value);
                        bitjadge &= ~(int)value;
                    }
                }
                else
                {
                    list.Add(value);
                }
            }
            return list;
        }
        /// <summary>
        /// Enumを文字列化、中身が重複してるケースを一意にするために生成
        /// </summary>
        static protected Dictionary<ButtonType, string> GetDicButtonType()
        {
            var dic = new Dictionary<ButtonType, string>
            {
                { ButtonType.A, "A" }, { ButtonType.B, "B" },
                { ButtonType.X, "X" }, { ButtonType.Y, "Y" },
                { ButtonType.PLUS, "PLUS" }, { ButtonType.MINUS, "MINUS" }, 
            };
            foreach (ButtonType value in Enum.GetValues(typeof(ButtonType)))
            {
                if (!dic.ContainsKey(value))
                {
                    dic.Add(value, value.ToString());
                }
            }
            return dic;
        }
        /// <summary>定数配列、登録してるボタンは以下の通り、Switchのコントローラーが基準になります</summary>
        static public List<ButtonType> BTNNUMLIST = GetListButtonType();
        static public Dictionary<ButtonType, string> BTNSTRDIC = GetDicButtonType();

        /// <summary>コントローラーが現在アクティブなのかどうか、これがFalseならUpdateは初期化だけになる</summary>
        [NonSerialized] public bool Active = true;
        /// <summary>スティックのYの正負を入れ替えるかどうか、デフォで入れ替える</summary>
        static public bool Yreverse = true;

        /// <summary>タッチされているか</summary>
        public bool TouchedPress { get; private set; }
        /// <summary>全体的にクリックされたか</summary>
        public bool TouchedDown { get; private set; }
        /// <summary>全体的にクリック終了したかどうか</summary>
        public bool TouchedUp { get; private set; }

        protected const int USE_TOUCHESCOUNT = 3;

        /// <summary>クリックした座標</summary>
        [SerializeField]
        public Vector2[] TouchesPosition { get; private set; } = new Vector2[USE_TOUCHESCOUNT];
        /// <summary>前回クリックした座標</summary>
        public Vector2[] TouchesBeforePosition { get; private set; } = new Vector2[USE_TOUCHESCOUNT];
        /// <summary>TouchesCursorの最初と現在の間のベクトル</summary>
        public Vector2[] TouchesVector { get; private set; } = new Vector2[USE_TOUCHESCOUNT];
        /// <summary>TouchesVectorの差分ベクトル</summary>
        public Vector2[] TouchesDeltaVector { get; private set; } = new Vector2[USE_TOUCHESCOUNT];
        /// <summary>前回のTouchesVector、確認用とかベクトル決めるときとかに使う</summary>
        public Vector2[] TouchesBeforeVector { get; private set; } = new Vector2[USE_TOUCHESCOUNT];
        /// <summary>指の本数ごとの入力値、とりあえず3つまで</summary>
        public bool[] TouchesPress { get; private set; } = new bool[USE_TOUCHESCOUNT];
        public bool[] TouchesDown { get; private set; } = new bool[USE_TOUCHESCOUNT];
        public bool[] TouchesUp { get; private set; } = new bool[USE_TOUCHESCOUNT];
        public bool[] TouchesDouble { get; private set; } = new bool[USE_TOUCHESCOUNT];
        public Vector2[] TouchesDownPosition { get; private set; } = new Vector2[USE_TOUCHESCOUNT];
        public Vector2[] TouchesUpPosition { get; private set; } = new Vector2[USE_TOUCHESCOUNT];
        /// <summary>マルチタッチのときの座標の基準</summary>
        public List<int> TouchesFocusID { get; private set; } = new List<int>(3);
        /// <summary>スワイプ判定</summary>
        public bool[] TouchesSwipeMode { get; private set; } = new bool[USE_TOUCHESCOUNT];
        /// <summary>スクリーンをタップしていた時間</summary>
        public float[] TouchesTime { get; private set; } = new float[USE_TOUCHESCOUNT];
        public Dictionary<int, bool> FingerLock { get; private set; } = new Dictionary<int, bool>();

        /// <summary>前回のスクロール量、Scrollで差分になる調整に使う</summary>
        public float BeforeScroll { get; private set; } = 0f;
        /// <summary>スクロール量、ズームとかに使う</summary>
        public float Scroll { get; private set; } = 0f;

        [Serializable]
        public struct InputInspector
        {
            public int PressButtonInt;
            public string PressButton;
            public Vector2 TouchPosition, TouchVector;
            public Vector2 MoveStick, RotStick;
            public Vector2 LeftStick, RightStick, neko;
        }
        [SerializeField] public InputInspector m_inputInspector = new InputInspector();

        /// <summary>
        /// ここで指定するプロパティは動的に複数管理できることを想定
        /// </summary>
        public PropertyClass Property { get { return m_property; } set { m_property = value; } }
        [SerializeField] private PropertyClass m_property = new PropertyClass();
        /// <summary>
        /// プロパティ、まとめて設定を管理できる
        /// </summary>
        [Serializable]
        public class PropertyClass
        {
            /// <summary>名前、付けなくても良い</summary>
            public string Name;
            /// <summary>ZLZRとLRを入れ替えるかどうか</summary>
            public bool ButtonZLR_Reverse = false;
            /// <summary>キーボードをコントローラーにするかどうか</summary>
            public bool ConKeyboard = true;
            /// <summary>joystickIDが0よりも大きいときにキーボードコントローラ有効にするか</summary>
            public bool MultPlayKeyboard = false;
            /// <summary>スティックの補正を行うかのフラグ</summary>
            public bool CompFlag = true;
            /// <summary>TouchDeltaVectorの倍率</summary>
            public float TouchDeltaMagnitude = 5f;
            /// <summary>タッチの際に取得したRotの反転、倍率の補正</summary>
            public Vector2 TouchRotReverseComp = new Vector2(1, 1);
            public bool TouchRotReverseX { get { return TouchRotReverseComp.x < 0; } set { TouchRotReverseComp.x = value ? -1 : 1; } }
            public bool TouchRotReverseY { get { return TouchRotReverseComp.y < 0; } set { TouchRotReverseComp.y = value ? -1 : 1; } }
            /// <summary>タッチの際にRotを取る対象で値を反転させるかどうか</summary>
            public Vector2 RotReverseComp = new Vector2(1, -1);
            public bool RotReverseX { get { return RotReverseComp.x < 0; } set { RotReverseComp.x = value ? -1 : 1; } }
            public bool RotReverseY { get { return RotReverseComp.y < 0; } set { RotReverseComp.y = value ? -1 : 1; } }

            /// <summary>スクリーン全体をタップしたときのボタン発生を有効にするか</summary>
            public bool TouchButtonFlag = true;
            /// <summary>タップしたときのボタンアクション</summary>
            public ButtonType[] TouchesButtonName = new ButtonType[USE_TOUCHESCOUNT]
                { ButtonType.A, ButtonType.NONE, ButtonType.NONE };
            /// <summary>
            /// タップしたときのボタンの挙動、Noneは動作無し
            /// スワイプモードになったとき、Clickは無効になる仕様です（一般的なタップになる）
            /// </summary>
            public ButtonMode[] TouchesButtonMode = new ButtonMode[USE_TOUCHESCOUNT]
                { ButtonMode.Click, ButtonMode.Press, ButtonMode.Press };
            /// <summary>指に対しての動かす対象</summary>
            public PosType[] TouchVectorMode = new PosType[USE_TOUCHESCOUNT]
            { PosType.Move, PosType.Rot, PosType.Center};

            /// <summary>矢印キーを移動ベクトルにするか</summary>
            public bool ArrowToMove = true;
            /// <summary>矢印キーの移動ベクトルにおける倍率</summary>
            public float ArrowKeyStrange = 1f;
            /// <summary>移動ベクトルを矢印キーにするか</summary>
            public bool MoveToArrow = true;
            /// <summary>移動ベクトルを矢印キーとみなす下限</summary>
            public float MoveArrowDead = 0.3f;

            /// <summary>タッチパネルを移動ベクトルにするか</summary>
            public bool SwipeToMove = true;
            /// <summary>スワイプしたとみなす距離、超えたらスワイプモードになる</summary>
            public float SwipeDead = 20f;
            /// <summary>スワイプの最大半径、この範囲内を0～1で表現</summary>
            public float SwipeMaxRadius = 40f;
        }

        /// <summary>ボタンをBoolディクショナリ配列形式にしたもの、これにまず格納する</summary>
        public Dictionary<ButtonType, bool> btnlist { get; private set; }
        /// <summary>リピートクラス</summary>
        public KeyRepeatDict Repeat = new KeyRepeatDict();
        static float StartLongRepeat = 1f;
        /// <summary>長押しリピートクラス</summary>
        public KeyRepeatDict LongRepeat = new KeyRepeatDict(StartLongRepeat);
        /// <summary>
        /// ボタンオブジェクト
        /// .Natural 押しっぱなしのときに発生
        /// .Repeat 押しっぱなしのときに断続的に発生
        /// .Down 押した瞬間だけ発生
        /// .Up 離した瞬間だけ発生
        /// </summary>
        public ButtonObj Button { get { return m_button; } set { m_button = value; } }
        protected ButtonObj m_button = new ButtonObj();
        [NonSerialized] public ButtonType VirtualButton = 0;
        /// <summary>スティックオブジェクト</summary>
        public StickObj Stick { get { return m_stick; } set { m_stick = value; } }
        public StickObj m_stick = new StickObj();
        /// <summary>補正クラス</summary>
        static public CompPoint CompObj { get; private set; } = new CompPoint();

        private int controllerCount;
        public string[] ControllerNames;

        public static ConType DefaultConType = ConType.Default;
        // 現在のコントローラ
        [SerializeField] ConType currentConType = DefaultConType;
        // コントローラの切り替えは再生成にする、複数体作るときのメモリ軽減
        public ConType CurrentConType
        {
            get { return currentConType; }
            set
            {
                bool doChange;
                int joystickID = 0;
                joystickID = JoystickID;
                doChange = ConType != currentConType;
                if (doChange)
                {
                    currentConType = value;
                    ConType = currentConType;
                }
            }
        }
        // コントローラの数が変わったときに実行
        private int activeJoystickCount = 0;
        public int ActiveJoystickCount
        {
            get { return activeJoystickCount; }
            private set
            {
                if (activeJoystickCount != value)
                {
                    if (CurrentConType == ConType.Default) BuildOfType(CurrentConType);
                    activeJoystickCount = value;
                }
            }
        }
        static public Dictionary<ConType, string> ConAutoReg = new Dictionary<ConType, string> {
            { ConType.Android, @".*::.*Android.*" },
            { ConType.Switch, @".*Wireless Gamepad.*::.*" },
        };
        public ConType Sys_ConType { get; private set; }
        /// <summary>コントローラーの種類</summary>
        private ConType m_conType;
        public ConType ConType
        {
            set { BuildOfType(value); }
            get { return m_conType; }
        }
        /// <summary>現在のコントローラ名やコントローラID</summary>
        public string JoystickName = "All";
        private int joystickID = 0;
        public int JoystickID
        {
            set
            {
                JoystickName = "All";
                string[] jsns = Input.GetJoystickNames();
                if ((value < ConObj.joysticks.Count) && (value <= jsns.Length))
                {
                    joystickID = value;
                    if (joystickID == 0)
                        JoystickName = "All";
                    else if (joystickID < 0)
                        JoystickName = "Keyboard";
                    else
                        JoystickName = jsns[joystickID - 1];
                }
            }
            get { return joystickID; }
        }

        public void Add(KeyType _keytype = KeyType.Key, ButtonType _button = 0, int _num = 1, string _keyname = "", bool _reverse = false, float _dead = 0.1f,
            PosType _pos = PosType.Left, PointType _pnt = PointType.X)
        {
            m_controller.Add(new ConObj(this, _keytype, _button, _num, _keyname, _reverse, _dead, _pos, _pnt));
        }
        /// <summary>通常のコンストラクタ、コントローラーオブジェクトの初期化</summary>
        public static Controller Create(Controller _controller = null, ConType contype = ConType.Default, 
            int _joystickID = 0)
        {
            if (_controller == null) _controller = new Controller();
            _controller.JoystickID = _joystickID;
            _controller.BuildOfType(contype);
            _controller.btnlist = new Dictionary<ButtonType, bool>();
            foreach (ButtonType con in BTNNUMLIST) _controller.btnlist.Add(con, false);
            return _controller;
        }
        public static Controller Create(ConType contype = ConType.Default, int _joystickID = 0)
        {
            return Create(null, contype, _joystickID);
        }

        /// <summary>有効なスティックの数を出力する</summary>
        public static int JoystickCount()
        {
            int cnt = 0;
            foreach (string stickname in Input.GetJoystickNames())
                if (stickname != "") cnt++;
            return cnt;
        }
        /// <summary>最も番号が小さいスティックを取得する</summary>
        public static string GetJoystickVeteran()
        {
            foreach (string stickname in Input.GetJoystickNames())
                if (stickname != "") return stickname;
            return "";
        }

        /// <summary>キーの交換、何番目のビットなのかで指定</summary>
        static public ButtonType KeySwap(ButtonType btn, int num1, int num2, bool oneway = false)
        {
            ButtonType b1 = (ButtonType)(1 << num1), b2 = (ButtonType)(1 << num2);
            ButtonType _b1 = (ButtonType)((((btn & b1) == b1) ? 1 : 0) << num2);
            ButtonType _b2 = (ButtonType)((((btn & b2) == b2) ? 1 : 0) << num1);
            btn = (btn & ~b1) | _b2;
            if (!oneway) btn = (btn & ~b2) | _b1;
            return btn;
        }

        /// <summary>テンプレ設置、後で追加することもできる</summary>
        public void SetTemp(ConTempSet templates)
        {
            m_controller.AddRange(ConTempSet.OutUseList(this, templates));
        }

        public void KeySwapLocal(ButtonType b1, ButtonType b2, bool oneway = false)
        {
            int num1, num2;
            num1 = (int)Math.Truncate(Math.Log((int)b1, 2));
            num2 = (int)Math.Truncate(Math.Log((int)b2, 2));
            Button[ButtonMode.Press] = KeySwap(Button[ButtonMode.Press], num1, num2, oneway);
            Button[ButtonMode.Repeat] = KeySwap(Button[ButtonMode.Repeat], num1, num2, oneway);
            Button[ButtonMode.Down] = KeySwap(Button[ButtonMode.Down], num1, num2, oneway);
            Button[ButtonMode.Up] = KeySwap(Button[ButtonMode.Up], num1, num2, oneway);
            Button[ButtonMode.Delay] = KeySwap(Button[ButtonMode.Press], num1, num2, oneway);
            Button[ButtonMode.DelayRepeat] = KeySwap(Button[ButtonMode.Repeat], num1, num2, oneway);
            Button[ButtonMode.DelayDown] = KeySwap(Button[ButtonMode.Down], num1, num2, oneway);
            Button[ButtonMode.DelayUp] = KeySwap(Button[ButtonMode.Up], num1, num2, oneway);
        }

        public void SetFingerLock(int fingerID)
        {
            if (!FingerLock.ContainsKey(fingerID)) FingerLock.Add(fingerID, true);
        }
        public void SetFingerUnLock(int fingerID)
        {
            FingerLock.Remove(fingerID);
        }

        /// <summary>仮想ボタン発火、PointerDownを使うのがおすすめ</summary>
        public void SetVirtualButton(ButtonType btn = ButtonType.NONE)
        {
            VirtualButton |= btn;
        }

        public void ControllerUpdate()
        {
            // 初期化
            foreach (ButtonType btnkey in BTNNUMLIST) btnlist[btnkey] = false;
            foreach (ButtonMode Type in Enum.GetValues(typeof(ButtonMode))) Button[Type] = 0;
            Stick.PosClear();
            TouchesPress = Array.ConvertAll(TouchesPress,x => false);
            TouchesDown = Array.ConvertAll(TouchesDown, x => false);
            TouchesUp = Array.ConvertAll(TouchesUp, x => false);
            TouchesDouble = Array.ConvertAll(TouchesDouble, x => false);
            TouchedPress = false;
            TouchedDown = false;
            TouchedUp = false;
            if (!Active) return;

            TouchesBeforePosition = (Vector2[])TouchesPosition.Clone();
            TouchesBeforeVector = (Vector2[])TouchesVector.Clone();

            // タッチ周りのリニューアル
            if (Input.touchSupported && (Input.touchCount > 0))
            {
                int count = 0;
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (count >= USE_TOUCHESCOUNT) break;
                    Touch t = Input.touches[i];
                    if (!FingerLock.ContainsKey(t.fingerId))
                    {
                        TouchesPosition[count] = t.position;
                        switch(t.phase)
                        {
                            case TouchPhase.Began:
                                TouchesDown[count] = true;
                                TouchesPress[count] = true;
                                break;
                            case TouchPhase.Moved:
                            case TouchPhase.Stationary:
                                TouchesPress[count] = true;
                                break;
                            case TouchPhase.Canceled:
                            case TouchPhase.Ended:
                                TouchesPress[count] = true;
                                TouchesUp[count] = true;
                                break;
                        }
                        count++;
                    }
                }
                if (TouchesPress[1] && !TouchesSwipeMode[0])
                {
                    BeforeScroll = Scroll;
                    Scroll = Vector3.Distance(TouchesPosition[0], TouchesPosition[0]) / 100f;
                }
            }
            else
            {
                if (!FingerLock.ContainsKey(-1))
                {
                    for (int i = 0; i < USE_TOUCHESCOUNT; i++)
                    {
                        TouchesPosition[i] = Input.mousePosition;
                        TouchesPress[i] = Input.GetMouseButton(i);
                        TouchesDown[i] = Input.GetMouseButtonDown(i);
                        TouchesUp[i] = Input.GetMouseButtonUp(i);
                        TouchedDown |= TouchesDown[i];
                    }
                    BeforeScroll = 0f;
                    Scroll = Input.GetAxis("Mouse ScrollWheel");
                }
            }
            for (int i = 0; i < USE_TOUCHESCOUNT; i++)
            {
                TouchedPress |= TouchesPress[i];
                TouchedUp |= TouchesUp[i];
            }
            float mag;
            PosType controllVector;
            if (TouchedPress)
            {
                for (int i = 0; i < USE_TOUCHESCOUNT; i++)
                {
                    controllVector = m_property.TouchVectorMode[i];
                    if (TouchesDown[i])
                    {
                        TouchesTime[i] = 0;
                        TouchesSwipeMode[i] = false;
                        TouchesDownPosition[i] = TouchesPosition[i];
                        if (m_property.TouchButtonFlag)
                        {
                            if ((m_property.TouchesButtonMode[i]
                                & (ButtonMode.Down)) > 0)
                            {
                                VirtualButton |= m_property.TouchesButtonName[i];
                            }
                        }
                    }

                    if (TouchesPress[i])
                    {
                        TouchesTime[i] += Time.deltaTime;
                        if (m_property.TouchButtonFlag)
                        {
                            if ((m_property.TouchesButtonMode[i]
                                & (ButtonMode.Press | ButtonMode.Repeat | ButtonMode.Double)) > 0)
                            {
                                VirtualButton |= m_property.TouchesButtonName[i];
                            }
                        }
                            
                        TouchesVector[i] = TouchesPosition[i] - TouchesDownPosition[i];
                        TouchesDeltaVector[i] = TouchesPosition[i] - TouchesBeforePosition[i];


                        mag = TouchesVector[i].magnitude;
                        if (!TouchesSwipeMode[i])
                            if (mag > m_property.SwipeDead)
                                TouchesSwipeMode[i] = true;
                        Vector2 vector2 = TouchesVector[i];
                        float delta_mag = 1f;

                        switch (controllVector)
                        {
                            case PosType.Rot:
                                vector2 = TouchesDeltaVector[i] * m_property.TouchRotReverseComp;
                                delta_mag = m_property.TouchDeltaMagnitude;
                                break;
                        }
                        if (controllVector != PosType.None)
                            if (TouchesSwipeMode[i])
                            {
                                Stick.SetMost(controllVector, 
                                    VecComp.ConvMag(vector2, m_property.SwipeMaxRadius), delta_mag);
                            }
                    }
                    if (TouchesUp[i])
                    {
                        if (m_property.TouchButtonFlag)
                        {
                            if ((m_property.TouchesButtonMode[i] & (ButtonMode.Up)) > 0)
                            {
                                VirtualButton |= m_property.TouchesButtonName[i];
                            }
                            else if ((m_property.TouchesButtonMode[i] & (ButtonMode.Click)) > 0)
                            {
                                if (!TouchesSwipeMode[i]) VirtualButton |= m_property.TouchesButtonName[i];
                            }
                        }
                        TouchesUpPosition[i] = TouchesPosition[i];
                        TouchesBeforeVector[i] = TouchesUpPosition[i] - TouchesDownPosition[i];
                        TouchesVector[i].Set(0, 0);
                        TouchesSwipeMode[i] = false;
                        TouchesTime[i] = 0f;
                    }
                }
            }

            // ボタン周り取得する
            foreach (ConObj con in m_controller)
                VirtualButton |= con.UpdateButton(m_property.ConKeyboard);
            if ((VirtualButton & ButtonType.ANY_ARROW) != 0)
                if (m_property.ArrowToMove)
                {
                    Vector2 vc2 = Vector2.zero;
                    float srg = m_property.ArrowKeyStrange;
                    // キー入力をMoveに落とし込む
                    if (ButtonObj.JudgeButton(ButtonType.UP | ButtonType.DOWN, VirtualButton))
                        vc2.y = ((ButtonObj.JudgeButton(ButtonType.UP, VirtualButton)) ? (Controller.Yreverse ? srg : -srg) : 0)
                            + ((ButtonObj.JudgeButton(ButtonType.DOWN, VirtualButton)) ? (Controller.Yreverse ? -srg : srg) : 0);
                    if (ButtonObj.JudgeButton(ButtonType.LEFT | ButtonType.RIGHT, VirtualButton))
                        vc2.x = ((ButtonObj.JudgeButton(ButtonType.LEFT, VirtualButton)) ? -srg : 0)
                            + ((ButtonObj.JudgeButton(ButtonType.RIGHT, VirtualButton)) ? srg : 0);
                    Stick[PosType.Move] = VecComp.SetCurcler(Stick[PosType.Move], vc2);
                    // キー入力をRotに落とし込む
                    vc2 = Vector2.zero;
                    if (ButtonObj.JudgeButton(ButtonType.R_UP | ButtonType.R_DOWN, VirtualButton))
                        vc2.y = ((ButtonObj.JudgeButton(ButtonType.R_UP, VirtualButton)) ? (Controller.Yreverse ? srg : -srg) : 0)
                            + ((ButtonObj.JudgeButton(ButtonType.R_DOWN, VirtualButton)) ? (Controller.Yreverse ? -srg : srg) : 0);
                    if (ButtonObj.JudgeButton(ButtonType.R_LEFT | ButtonType.R_RIGHT, VirtualButton))
                        vc2.x = ((ButtonObj.JudgeButton(ButtonType.R_LEFT, VirtualButton)) ? -srg : 0)
                            + ((ButtonObj.JudgeButton(ButtonType.R_RIGHT, VirtualButton)) ? srg : 0);
                    Stick[PosType.Rot] = VecComp.SetCurcler(Stick[PosType.Rot], vc2);
                }
            if (m_property.MoveToArrow)
            {
                // Moveから方向キーを取得
                Vector2 smov = Stick[PosType.Move];
                float rot = VecComp.ToAbsDeg(smov);
                if (smov.magnitude > m_property.MoveArrowDead)
                {
                    if (160f > rot && rot > 20f) VirtualButton |= Yreverse ? ButtonType.UP : ButtonType.DOWN;
                    if (250f > rot && rot > 110f) VirtualButton |= ButtonType.LEFT;
                    if (340f > rot && rot > 200f) VirtualButton |= Yreverse ? ButtonType.DOWN : ButtonType.UP;
                    if (rot > 290f || 70f > rot) VirtualButton |= ButtonType.RIGHT;
                }
                // Rotから方向キーを取得
                smov = Stick[PosType.Rot];
                rot = VecComp.ToAbsDeg(smov);
                if (smov.magnitude > m_property.MoveArrowDead)
                {
                    if (160f > rot && rot > 20f) VirtualButton |= Yreverse ? ButtonType.R_UP : ButtonType.R_DOWN;
                    if (250f > rot && rot > 110f) VirtualButton |= ButtonType.R_LEFT;
                    if (340f > rot && rot > 200f) VirtualButton |= Yreverse ? ButtonType.R_DOWN : ButtonType.R_UP;
                    if (rot > 290f || 70f > rot) VirtualButton |= ButtonType.R_RIGHT;
                }
            }

            // 各々のボタンステータスの反映
            foreach (ButtonType btnkey in BTNNUMLIST)
            {
                if (ButtonObj.JudgeButton(btnkey,VirtualButton,true))
                {
                    btnlist[btnkey] = true;
                }
            }

            // リピートなどの付与
            foreach (ButtonType btnkey in BTNNUMLIST)
            {
                bool press = btnlist[btnkey];
                KeyRepeatClass rep = Repeat[btnkey];
                if (press)
                {
                    Button[ButtonMode.Press] |= btnkey;
                    if (rep.double_press) Button[ButtonMode.Double] |= btnkey;
                }
                if (rep.Check(press))
                {
                    Button[ButtonMode.Repeat] |= btnkey;
                }
                if (rep.first)
                {
                    Button[ButtonMode.Down] |= btnkey;
                    if (rep.double_press) Button[ButtonMode.DoubleClick] |= btnkey;
                }
                if (rep.last)
                {
                    Button[ButtonMode.Up] |= btnkey;
                    Button[ButtonMode.Click] |= btnkey;
                    if (rep.double_press) Button[ButtonMode.DoubleUp] |= btnkey;
                }
                rep = LongRepeat[btnkey];
                if (rep.enable)
                {
                    Button[ButtonMode.Delay] |= btnkey;
                }
                if (rep.Check(press))
                {
                    Button[ButtonMode.DelayRepeat] |= btnkey;
                }
                if (rep.first)
                {
                    Button[ButtonMode.DelayDown] |= btnkey;
                }
                if (rep.last)
                {
                    Button[ButtonMode.DelayUp] |= btnkey;
                }
            }

            if (m_property.ButtonZLR_Reverse)
            {
                KeySwapLocal(ButtonType.L, ButtonType.ZL);
                KeySwapLocal(ButtonType.R, ButtonType.ZR);
            }
            Stick[PosType.Rot] *= m_property.RotReverseComp;
            VirtualButton = 0;

            for (int i = 0; i < m_globalControllerEvents.Count; i++)
            {
                ControllerEvent e = m_globalControllerEvents[i];
                if (m_button.JudgeButton(e.m_buttonName, e.m_buttonMode)) e.m_onEvent.Invoke();
            }
        }

        // デフォルトで生成されるキーコンフィグデータ
        public void BuildOfType(ConType contype)
        {
            // Defaultは条件によって変化
            if (contype == ConType.Default)
            {
                string joylocal = JoystickName;
                if (joystickID == 0) joylocal = GetJoystickVeteran();
                joylocal += "::" + SystemInfo.operatingSystem;
                foreach (ConType key in new List<ConType>(ConAutoReg.Keys))
                {
                    if (Regex.IsMatch(joylocal, ConAutoReg[key])) contype = key;
                }
                if (contype == ConType.Default) contype = ConType.Xinput;
                BuildOfType(contype);
                m_conType = ConType.Default;
                return;
            }
            m_conType = contype;
            Sys_ConType = m_conType;

            m_controller.Clear();
            if (contype == ConType.Other) return;   // Otherは各自でAddすることを想定
            SetTemp(ConTempSet.UserFirstTemplate);
            SetTemp(ConTempSet.CommonTemplate);
            SetTemp(ConTempSet.SysTemplate);
            SetTemp(ConTempSet.KeyboardTemplate);
            switch (contype)
            {
                case ConType.Xinput:
                    SetTemp(ConTempSet.XinputTemplate);
                    break;
                case ConType.Direct:
                    SetTemp(ConTempSet.DirectTemplate);
                    break;
                case ConType.Switch:
                    SetTemp(ConTempSet.SwitchTemplate);
                    break;
                case ConType.Android:
                    SetTemp(ConTempSet.AndroidTemplate);
                    break;
            }
            SetTemp(ConTempSet.UserLastTemplate);
        }
        // Unityの終了命令
        static public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        const string CONTROLLER_NAME = "MainController";
        const string CONTROLLER_TAG = "GameController";
        static public GameObject GetGameMain(GameObject gameMain = null)
        {
            // 操作親を設定する、デフォルトでgameMainを取得、なければ自分で作る
            if (gameMain == null)
            {
                gameMain = GameObject.FindGameObjectWithTag(CONTROLLER_TAG);
                if (gameMain == null)
                {
                    gameMain = new GameObject(CONTROLLER_NAME);
                    gameMain.tag = CONTROLLER_TAG;
                }
                var _controller = gameMain.GetComponent<Controller>();
                if (_controller == null)
                {
                    gameMain.AddComponent<Controller>();
                }
            }
            return gameMain;
        }
        void Start()
        {
            Create(this, currentConType);
            activeJoystickCount = JoystickCount();
            Update();
        }

        private void OnValidate()
        {
            CurrentConType = currentConType;
        }
        void Update()
        {
            ActiveJoystickCount = JoystickCount();
            ControllerNames = Input.GetJoystickNames();
            ControllerUpdate();

            m_inputInspector.TouchPosition = TouchesPosition[0];
            m_inputInspector.TouchVector = TouchesVector[0];
            m_inputInspector.LeftStick = Stick[PosType.Left];
            m_inputInspector.RightStick = Stick[PosType.Right];
            m_inputInspector.MoveStick = Stick[PosType.Move];
            m_inputInspector.RotStick = Stick[PosType.Rot];

            m_button = Button;
            m_inputInspector.PressButtonInt = (int)m_button[ButtonMode.Press];
            m_inputInspector.PressButton = ButtonObj.ResultButton((ButtonType)m_inputInspector.PressButtonInt);

            if (m_button.JudgeButton((ButtonType)13056, ButtonMode.Delay, true)) { Quit(); }
            if (m_button.JudgeButton(ButtonType.ESC, ButtonMode.DelayDown)) { Quit(); }
        }
    }
}

