using System.Collections.Generic;
using Tofunaut.Core;
using Tofunaut.SharpUnity;
using UnityEngine;

namespace Tofunaut.Deeepr.Game
{
    public class Floor : SharpGameObject
    {
        public Tile this[int x, int y] => GetTile(x, y);

        public readonly int level;
        private Tile[,] _tiles;
        private List<Actor> _actors;

        public Floor(int[,] tileData, int level) : base($"Floor_{level}")
        {
            this.level = level;
            _actors = new List<Actor>();
            _tiles = new Tile[tileData.GetLength(0), tileData.GetLength(1)];

            for (int y = 0; y < tileData.GetLength(1); y++)
            {
                for(int x = 0; x < tileData.GetLength(0); x++)
                {
                    _tiles[x, y] = Tile.Create(this, new Core.IntVector2(x, y), (Tile.EType)tileData[x, y]);
                    AddChild(_tiles[x, y]);
                }
            }
        }

        public Tile GetTile(IntVector2 coord) => GetTile(coord.x, coord.y);
        public Tile GetTile(int x, int y)
        {
            if(x < 0 || x >= _tiles.GetLength(0) || y < 0 || y >= _tiles.GetLength(1))
            {
                return null;
            }

            return _tiles[x, y];
        }

        protected override void Build() { }

        public static Floor TestFloor()
        {
            // this will be rotated 90 counter clockwise
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