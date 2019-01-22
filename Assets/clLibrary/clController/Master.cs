using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UclController
{
    [DefaultExecutionOrder(0xF)]
    public class Master : MonoBehaviour
    {
        [System.NonSerialized]
        public GameObject GameController = null;
        public GameObject GameFollow = null;
        // 内部以外は読み取り専用にする
        public MainController MainCon { get; private set; }
        public FollowController MainFollow { get; private set; }
        public Controller Con { get; private set; }
        public ButtonObj Button { get; private set; }
        public StickObj Stick { get; private set; }

        public void Start()
        {
            GameController = MainController.GetGameMain(GameController);
            MainCon = GameController.GetComponent<MainController>();
            MainFollow = GameController.GetComponent<FollowController>();
            Update();
        }
        public void Update()
        {
            Con = MainCon.controller;
            Button = Con.Button;
            Stick = Con.Stick;
        }
    }
}