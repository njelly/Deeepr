using Tofunaut.Core;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeepr.Game
{
    public class PlayerActorInputController : AutomaticSingletonBehaviour<PlayerActorInputController>
    {
        public Actor currentTarget;

        private ActorInput _input = new ActorInput();

        private void Update()
        {
            PollInput();

            if(currentTarget != null)
            {
                currentTarget.ReceiveInput(_input);
            }
        }

        private void PollInput()
        {
            _input.actionButton.wasDown = _input.actionButton;
            _input.actionButton.timeDown = Input.GetKey(KeyCode.Space) ? _input.actionButton.timeDown + Time.deltaTime : 0f;

            _input.direction = IntVector2.Zero;

            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                _input.direction = IntVector2.Up;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                _input.direction = IntVector2.Down;
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                _input.direction = IntVector2.Left;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                _input.direction = IntVector2.Right;
            }
        }
    }
}