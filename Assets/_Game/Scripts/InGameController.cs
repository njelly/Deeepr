using Tofunaut.Core;
using Tofunaut.UnityUtils;
using Tofunaut.Deeepr.Game;
using UnityEngine;

namespace Tofunaut.Deeepr
{
    public class InGameController : ControllerBehaviour
    {
        private static class State
        {
            public const string Loading = "loading";
            public const string InGame = "in_game";
        }

        private TofuStateMachine _stateMachine;
        private GameManager _game;

        private void Awake()
        {
            _stateMachine = new TofuStateMachine();
            _stateMachine.Register(State.Loading, Loading_Enter, Loading_Update, null);
            _stateMachine.Register(State.InGame, InGame_Enter, null, null);
        }

        private void OnEnable()
        {
            _stateMachine.ChangeState(State.Loading);
        }

        private void Update()
        {
            _stateMachine.Update(Time.deltaTime);
        }

        private void Loading_Enter()
        {
            AppManager.AssetManager.Load<ActorConfigAsset>("PlayerActorConfig");
        }

        private void Loading_Update(float deltaTime)
        {
            if(AppManager.AssetManager.Ready)
            {
                _stateMachine.ChangeState(State.InGame);
            }
        }

        private void InGame_Enter()
        {
            _game = gameObject.RequireComponent<GameManager>();
        }
    }
}