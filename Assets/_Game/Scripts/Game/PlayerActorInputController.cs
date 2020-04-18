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
            if(GameManager.IsGenerating)
            {
                _input.actionButton.wasDown = _input.actionButton;
                _input.actionButton.timeDown = 0f;
                _input.direction.SetDirection(IntVector2.Zero, Time.deltaTime);

                return;
            }

            _input.actionButton.wasDown = _input.actionButton;
            _input.actionButton.timeDown = Input.GetKey(KeyCode.Space) ? _input.actionButton.timeDown + Time.deltaTime : 0f;

            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                _input.direction.SetDirection(IntVector2.Up, Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                _input.direction.SetDirection(IntVector2.Down, Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                _input.direction.SetDirection(IntVector2.Left, Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                _input.direction.SetDirection(IntVector2.Right, Time.deltaTime);
            }
            else
            {
                _input.direction.SetDirection(IntVector2.Zero, Time.deltaTime);
            }
        }
    }
}