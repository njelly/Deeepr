using Tofunaut.SharpUnity;

namespace Tofunaut.Deeepr.Game
{
    public class Floor : SharpGameObject
    {
        public FloorTile this[int x, int y] { get { return _tiles[x, y]; } }

        public readonly int level;
        private FloorTile[,] _tiles;

        public Floor(FloorTile[,] tiles, int level) : base($"Floor_{level}")
        {
            this.level = level;
            _tiles = tiles;
        }

        protected override void Build()
        {

        }
    }
}