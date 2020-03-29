using Tofunaut.Core;
using Tofunaut.SharpUnity;
using Tofunaut.UnityUtils;

namespace Tofunaut.Deeepr.Game
{
    public class FloorTile : SharpGameObject
    {
        public Actor Occupant => _occupant;

        public readonly IntVector2 coord;
        public readonly Floor floor;

        private Actor _occupant;

        public FloorTile(Floor floor, IntVector2 coord) : base($"FloorTile {coord}")
        {
            this.floor = floor;
            this.coord = coord;

            LocalPosition = coord.ToUnityVector3_XY();
        }

        protected override void Build()
        {

        }

        public void SetOccupant(Actor actor)
        {
            if (actor != _occupant && _occupant != null)
            {
                RemoveChild(_occupant, false);
            }

            if (actor != null)
            {
                AddChild(actor, true);
                _occupant = actor;
            }
        }
    }
}