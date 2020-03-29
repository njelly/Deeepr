using Tofunaut.Core;
using Tofunaut.SharpUnity;
using Tofunaut.UnityUtils;

namespace Tofunaut.Deeepr.Game
{
    public abstract class Tile : SharpGameObject
    {
        public readonly IntVector2 coord;
        public readonly Floor floor;

        public virtual Collision.ELayer SolidLayer => _defaultSolidLayer;

        protected readonly Collision.ELayer _defaultSolidLayer;

        public Tile(Floor floor, IntVector2 coord, Collision.ELayer solidLayer) : base($"FloorTile {coord}")
        {
            this.floor = floor;
            this.coord = coord;

            _defaultSolidLayer = solidLayer;

            LocalPosition = coord.ToUnityVector3_XY();
        }
    }
}