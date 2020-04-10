using System;
using System.Collections.Generic;
using Tofunaut.Animation;
using Tofunaut.Core;
using Tofunaut.SharpUnity;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeepr.Game
{
    public class Actor : SharpGameObject, ICollider
    {
        public event EventHandler InteractionBegan;
        public event EventHandler InteractionEnded;

        private const float DirectionInputHysteresisPeriod = 0.1f; // allow the actor to tap input dir to face direction without moving in that direction

        public enum EFacing
        {
            Right,
            Left,
        }

        public CollisionInfo CollisionInfo => _config.collisionInfo;
        public IntVector2 InteractOffset { get; private set; }
        public const float MoveHysterisisPeriod = 0.1f;
        public EFacing Facing { get; private set; }
        public Tile CurrentTile { get; private set; }

        private ActorInput _input;
        private readonly ActorConfig _config;
        private readonly GameManager _gameManager;
        private ActorView _instantiatedView;
        private IntVector2 _targetCoord;
        private TofuAnimation _moveAnimation;

        public Actor(GameManager gameManager, ActorConfig config) : base("Actor")
        {
            _gameManager = gameManager;
            _config = config;
            InteractOffset = IntVector2.Right;
        }

        protected override void Build()
        {
            if (!string.IsNullOrEmpty(_config.viewPath))
            {
                AppManager.AssetManager.Load(_config.viewPath, (bool succesful, GameObject payload) =>
                {
                    if (succesful)
                    {
                        _instantiatedView = UnityEngine.Object.Instantiate(payload).GetComponent<ActorView>();
                        _instantiatedView.transform.SetParent(Transform, false);
                        _instantiatedView.Initialize(this);
                    }
                });
            }
        }

        protected override void PostRender()
        {
            base.PostRender();

            _targetCoord = Transform.position.ToIntVector2_XY();
            if(!TryOccupyCoord(_targetCoord))
            {
                Debug.LogError($"Actor could not occupy the tile it spawned on {_targetCoord}");
            }
        }

        public void ReceiveInput(ActorInput input)
        {
            _input = input;

            if(input.direction)
            {
                InteractOffset = input.direction;

                // set facing direction based on the interact offset
                if (InteractOffset.x > 0)
                {
                    Facing = EFacing.Right;
                }
                else if (InteractOffset.x < 0)
                {
                    Facing = EFacing.Left;
                }

                if (_input.direction.timeDown > DirectionInputHysteresisPeriod)
                {
                    if (_moveAnimation == null && TryOccupyCoord(_targetCoord + _input.direction))
                    {
                        Vector3 velocity = _input.direction.Direction.ToUnityVector3_XY() * _config.moveSpeed;
                        _targetCoord += _input.direction;
                        _moveAnimation = new TofuAnimation()
                            .WaitUntil(() =>
                            {
                                Vector3 step = velocity * Time.deltaTime;
                                Vector3 toTarget = _targetCoord.ToUnityVector3_XY() - Transform.position;
                                if (toTarget.sqrMagnitude < step.sqrMagnitude)
                                {
                                    if (_input.direction)
                                    {
                                        Transform.position = _targetCoord.ToUnityVector3_XY();

                                        if (!TryOccupyCoord(_targetCoord + _input.direction))
                                        {
                                            return true;
                                        }

                                        _targetCoord += _input.direction;
                                        Transform.position += (_targetCoord.ToUnityVector3_XY() - Transform.position).normalized * (toTarget.magnitude - step.magnitude);
                                        velocity = _input.direction.Direction.ToUnityVector3_XY() * _config.moveSpeed;

                                        return false;
                                    }
                                    else
                                    {
                                        Transform.position = _targetCoord.ToUnityVector3_XY();
                                        return true;
                                    }
                                }
                                else
                                {
                                    Transform.position += step;
                                    return false;
                                }
                            })
                            .Then()
                            .Execute(() =>
                            {
                                _moveAnimation = null;
                            })
                            .Play();
                    }
                }
            }

            if(input.actionButton.Pressed)
            {
                BeginInteraction();
            }
            else if (input.actionButton.Released)
            {
                EndInteraction();
            }
        }

        private void BeginInteraction()
        {
            InteractionBegan?.Invoke(this, EventArgs.Empty);
        }

        private void EndInteraction()
        {
            InteractionEnded?.Invoke(this, EventArgs.Empty);
        }

        public bool TryOccupyCoord(IntVector2 coord)
        {
            Tile tile = _gameManager.CurrentFloor.GetTile(coord);
            if(tile == null)
            {
                return false;
            }

            bool toReturn = tile.TrySetOccupant(this);
            if(toReturn)
            {
                CurrentTile = tile;
            }

            return toReturn;
        }

        public bool CanOccupyCoord(IntVector2 coord)
        {
            Tile tile = _gameManager.CurrentFloor.GetTile(coord);
            if (tile == null)
            {
                return false;
            }

            return tile.CanSetOccupant(this);
        }
    }

    [Serializable]
    public class ActorConfig
    {
        public string viewPath;
        public float moveSpeed;
        public CollisionInfo collisionInfo;
    }

    public class ActorInput
    {
        public DirectionButton direction = new DirectionButton();
        public Button actionButton = new Button();

        public class Button
        {
            public bool wasDown;
            public float timeDown;

            public bool Released => timeDown <= float.Epsilon && wasDown;
            public bool Held => timeDown > float.Epsilon && wasDown;
            public bool Pressed => timeDown > float.Epsilon && !wasDown;

            public static implicit operator bool(Button button)
            {
                return button.timeDown > float.Epsilon;
            }
        }

        public class DirectionButton : Button
        {
            public IntVector2 Direction { get; private set; } = IntVector2.Zero;

            public void SetDirection(IntVector2 direction, float deltaTime)
            {
                wasDown = this;
                timeDown = direction.Equals(IntVector2.Zero) ? 0f : timeDown + deltaTime;

                Direction = direction;
            }
            public static implicit operator IntVector2(DirectionButton button)
            {
                return button.timeDown > float.Epsilon ? button.Direction : IntVector2.Zero;
            }
        }
    }
}