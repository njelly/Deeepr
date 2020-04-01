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

        public Floor(int[,] tileData, int level) : base($"Floor_{level}")
        {
            this.level = level;
            _actors = new List<Actor>();

            for(int y = 0; y < tileData.GetLength(1); y++)
            {
                for(int x = 0; x < tileData.GetLength(0); x++)
                {
                    AddChild(Tile.Create(this, new Core.IntVector2(x, y), (Tile.EType)tileData[x, y]));
                }
            }
        }

        protected override void Build() { }

        public static Floor TestFloor()
        {
            // this will be flipped vertically
            int[,] tileData = { {1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                            { 1, 0, 1, 0, 0, 0, 0, 0, 0, 1 },
                            { 1, 0, 0, 0, 0, 0, 0, 1, 0, 1 },
                            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },};

            return new Floor(tileData, 0);
        }
    }
}