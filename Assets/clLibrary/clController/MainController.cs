/// <summary>
/// いわゆるメイン関数、シーン内のゲームオブジェクトのうちの一つに組み込む
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace clController
{
    public class MainController : MonoBehaviour
    {
        public static ConType DefaultConType = ConType.Default;
        // 現在のコントローラ
        [SerializeField] ConType currentConType = DefaultConType;
        [System.NonSerialized] public Controller controller = null;

        public ButtonObj m_button;

        [System.Serializable]
        public struct InputInspector
        {
            public int PressButtonInt;
            public string PressButton;
            public Vector2 TouchPosition, TouchVector;
            public Vector2 MoveStick, RotStick;
            public Vector2 LeftStick, RightStick;
        }
        [SerializeField] public InputInspector m_inputInspector = new InputInspector();
        [SerializeField] public Controller.PropertyClass m_controllerProperty;

        // コントローラの切り替えは再生成にする、複数体作るときのメモリ軽減
        public ConType CurrentConType
        {
            get { return currentConType; }
            set
            {
                bool doChange;
                int joystickID = 0;
                if (controller == null) doChange = true;
                else
                {
                    joystickID = controller.JoystickID;
                    doChange = controller.ConType != currentConType;
                }
                if (doChange)
                {
                    currentConType = value;
                    if (controller == null)
                        controller = Controller.Create(currentConType, joystickID);
                    else
                        controller.ConType = currentConType;
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
                    if (CurrentConType == ConType.Default) controller.BuildOfType(CurrentConType);
                    activeJoystickCount = value;
                }
            }
        }

        private int controllerCount;
        public string[] ControllerNames;

        // Unityの終了命令
        static public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        static public GameObject GetGameMain(GameObject gameMain = null)
        {
            // 操作親を設定する、デフォルトでgameMainを取得、なければ自分で作る
            if (gameMain == null)
            {
                gameMain = GameObject.Find("MainController");
                if (gameMain == null)
                {
                    gameMain = new GameObject("MainController");
                    gameMain.AddComponent<MainController>();
                }
            }
            return gameMain;
        }
        void Start()
        {
            controller = Controller.Create(currentConType);
            m_controllerProperty = controller.Property;
            activeJoystickCount = Controller.JoystickCount();
            Update();
        }

        private void OnValidate()
        {
            CurrentConType = currentConType;
        }
        void Update()
        {
            ActiveJoystickCount = Controller.JoystickCount();
            ControllerNames = Input.GetJoystickNames();
            controller.Update();

            m_inputInspector.TouchPosition = controller.TouchesPosition[0];
            m_inputInspector.TouchVector = controller.TouchesVector[0];
            m_inputInspector.LeftStick = controller.Stick[PosType.Left];
            m_inputInspector.RightStick = controller.Stick[PosType.Right];
            m_inputInspector.MoveStick = controller.Stick[PosType.Move];
            m_inputInspector.RotStick = controller.Stick[PosType.Rot];

            m_button = controller.Button;
            m_inputInspector.PressButtonInt = (int)m_button[ButtonMode.Press];
            m_inputInspector.PressButton = ButtonObj.ResultButton((ButtonType)m_inputInspector.PressButtonInt);

            if (m_button.JudgeButton((ButtonType)13056, ButtonMode.Delay, true)) { Quit(); }
            if (m_button.JudgeButton(ButtonType.ESC, ButtonMode.DelayDown)) { Quit(); }
        }
    }
}
