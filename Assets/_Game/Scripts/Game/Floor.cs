using System.Collections.Generic;
using Tofunaut.SharpUnity;

namespace Tofunaut.Deeepr.Game
{
    public class Floor : SharpGameObject
    {
        public Tile this[int x, int y] { get { return _tiles[x, y]; } }

        public readonly int level;
        private Tile[,] _tiles;
        private List<Actor> _actors;

        public Floor(Tile[,] tiles, int level) : base($"Floor_{level}")
        {
            this.level = level;
            _tiles = tiles;
            _actors = new List<Actor>();
        }

        protected override void Build()
        {

        }
    }
}