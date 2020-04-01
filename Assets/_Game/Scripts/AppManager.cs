using Tofunaut.Core;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeepr
{
    public class AppManager : SingletonBehaviour<AppManager>
    {
        public static class State
        {
            public const string Initialize = "initialize";
            public const string StartMenu = "start_menu";
            public const string InGame = "in_game";
        }

        [SerializeField] private ColorPallet _colorPallet;

        [Header("Debug")]
        [SerializeField] private bool _skipStartMenu;

        public static Version AppVersion { get; private set; }
        public static AssetManager AssetManager { get; private set; }
        public static ColorPallet ColorPallet => _instance._colorPallet;

        protected override bool SetDontDestroyOnLoad => true;

        private TofuStateMachine _stateMachine;

        protected override void Awake()
        {
            base.Awake();

            AppVersion = new Version($"{Application.version}.{BuildNumberUtil.ReadBuildNumber()}");
            Debug.Log($"Deeepr {AppVersion} (c) 2020 Tofunaut");

            _stateMachine = new TofuStateMachine();
            _stateMachine.Register(State.Initialize, Initalize_Enter, Initalize_Update, null);
            _stateMachine.Register(State.StartMenu, StartMenu_Enter, null, null);
            _stateMachine.Register(State.InGame, InGame_Enter, null, InGame_Exit);

            _stateMachine.ChangeState(State.Initialize);
        }

        private void Update()
        {
            _stateMachine.Update(Time.deltaTime);
        }

        private void Initalize_Enter()
        {
            AssetManager = new AssetManager();

            // TODO: load necessary assets like fonts and things here
        }

        private void Initalize_Update(float deltaTime)
        {
            if (AssetManager.Ready)
            {
#if UNITY_EDITOR
                if (_skipStartMenu)
                {
                    _stateMachine.ChangeState(State.InGame);
                }
                else
                {
                    _stateMachine.ChangeState(State.StartMenu);
                }
#else
                _stateMachine.ChangeState(State.StartMenu)
#endif
            }
        }

        private void StartMenu_Enter()
        {
            Debug.Log("StartMenu_Enter");
        }

        private void InGame_Enter()
        {
            InGameController inGameController = gameObject.RequireComponent<InGameController>();
            inGameController.enabled = true;
            inGameController.Completed += InGameController_Completed;
        }

        private void InGame_Exit()
        {
            InGameController inGameController = gameObject.RequireComponent<InGameController>();
            inGameController.enabled = false;
            inGameController.Completed -= InGameController_Completed;
        }

        private void InGameController_Completed(object sender, ControllerCompletedEventArgs e)
        {

        }
    }
}