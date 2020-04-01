using System;
using Tofunaut.Animation;
using Tofunaut.Core;
using Tofunaut.SharpUnity;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeepr.Game
{
    public class Actor : SharpGameObject, ICollider
    {
        public CollisionInfo CollisionInfo => _config.collisionInfo;
        public const float MoveHysterisisPeriod = 0.1f;

        private ActorInput _input;
        private readonly ActorConfig _config;
        private readonly GameManager _gameManager;
        private GameObject _instantiatedView;
        private IntVector2 _interactOffset;
        private Vector3 _targetPosition;
        private IntVector2 _prevInputDirection;
        private TofuAnimation _inputDirectionHysterisisTimer;

        public Actor(GameManager gameManager, ActorConfig config) : base("Actor")
        {
            _gameManager = gameManager;
            _config = config;
            _interactOffset = IntVector2.Right;
        }

        protected override void Build()
        {
            if (!string.IsNullOrEmpty(_config.viewPath))
            {
                AppManager.AssetManager.Load(_config.viewPath, (bool succesful, GameObject payload) =>
                {
                    if (succesful)
                    {
                        _instantiatedView = UnityEngine.Object.Instantiate(payload);
                        _instantiatedView.transform.SetParent(Transform, false);
                    }
                });
            }
        }

        public void ReceiveInput(ActorInput input)
        {
            _input = input;

            _interactOffset = input.direction;

            Debug.Log(_interactOffset);
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
        public IntVector2 direction = IntVector2.Zero;
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
    }
}