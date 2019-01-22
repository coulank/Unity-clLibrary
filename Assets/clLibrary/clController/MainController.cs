/// <summary>
/// いわゆるメイン関数、シーン内のゲームオブジェクトのうちの一つに組み込む
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UclController
{
    public class MainController : MonoBehaviour
    {
        public static ConType DefaultConType = ConType.Default;
        // 現在のコントローラ
        [SerializeField] ConType currentConType = DefaultConType;
        public Controller controller = null;
        
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
                        controller = new Controller(currentConType, joystickID);
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

        public ButtonObj button;
        public int btn0;
        public string currentButton = "";

        [SerializeField] Vector2 ls, rs, mover, rotMover;
        [SerializeField] Vector2 touchCursor, touchDown, touchVector, touchUp;
        public Vector2 Ls
        {
            get { return ls; }
            private set { ls = value; }
        }
        public Vector2 Rs
        {
            get { return rs; }
            private set { rs = value; }
        }
        public Vector2 Mover
        {
            get { return mover; }
            private set { mover = value; }
        }
        public Vector2 RotMover
        {
            get { return rotMover; }
            private set { rotMover = value; }
        }
        public Vector2 TouchCursor { get { return touchCursor; } }
        public Vector2 TouchDown { get { return touchDown; } }
        public Vector2 TouchVector { get { return touchVector; } }
        public Vector2 TouchUp { get { return touchUp; } }

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
            controller = new Controller(currentConType);
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

            touchCursor = controller.TouchesPosition[0];
            touchDown = controller.TouchesDownPosition[0];
            touchVector = controller.TouchesVector[0];
            touchUp = controller.TouchesUpPosition[0];

            button = controller.Button;
            btn0 = (int)button[ButtonMode.Press];
            currentButton = ButtonObj.ResultButton((ButtonType)btn0);
            Ls = controller.Stick[PosType.Left];
            Rs = controller.Stick[PosType.Right];
            Mover = controller.Stick[PosType.Move];
            RotMover = controller.Stick[PosType.Rot];

            if (button.JudgeButton((ButtonType)13056, ButtonMode.Delay, true)) { Quit(); }
            if (button.JudgeButton(ButtonType.ESC, ButtonMode.DelayDown)) { Quit(); }
        }
    }
}