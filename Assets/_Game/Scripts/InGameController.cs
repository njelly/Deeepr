using Tofunaut.Core;
using Tofunaut.UnityUtils;
using Tofunaut.Deeepr.Game;

namespace Tofunaut.Deeepr
{
    public class InGameController : ControllerBehaviour
    {
        private static class State
        {
            public static string Loading;
            public static string InGame;
        }

        private TofuStateMachine _stateMachine;
        private Game.Game _game;

        private void Awake()
        {
            _stateMachine.Register(State.Loading, null, null, null);
            _stateMachine.Register(State.InGame, null, null, null);
        }

        private void OnEnable()
        {
            _stateMachine.ChangeState(State.Loading);
        }

        private void Loading_Enter()
        {
            // TODO load from save data / configuration?
            _game = new Game.Game();
        }
    }
}