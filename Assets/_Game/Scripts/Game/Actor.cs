using System;
using Tofunaut.Core;
using Tofunaut.SharpUnity;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeepr.Game
{
    public class Actor : SharpGameObject, ICollider
    {
        public CollisionInfo CollisionInfo => _config.collisionInfo;

        private ActorInput _input;
        private readonly ActorConfig _config;
        private readonly GameManager _gameManager;
        private GameObject _instantiatedView;
        private IntVector2 _interactOffset;

        public Actor(GameManager gameManager, ActorConfig config) : base("Actor")
        {
            _gameManager = gameManager;
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