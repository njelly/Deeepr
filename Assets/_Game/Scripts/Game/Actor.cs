using Tofunaut.Core;
using Tofunaut.SharpUnity;

namespace Tofunaut.Deeepr.Game
{
    public class Actor : SharpGameObject, ICollider
    {
        public CollisionInfo CollisionInfo => _collisionInfo;

        private ActorInput _input;
        private CollisionInfo _collisionInfo;

        public Actor(string name) : base(name)
        {

        }

        protected override void Build()
        {

        }

        public void ReceiveInput(ActorInput input)
        {
            _input = input;
        }
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