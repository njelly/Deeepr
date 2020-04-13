using System.Collections.Generic;
using Tofunaut.Core;
using Tofunaut.SharpUnity;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeepr.Game
{
    public class Floor : SharpGameObject
    {
        public Tile this[int x, int y] => GetTile(x, y);
        public readonly Tile ladderUp;
        public readonly Tile ladderDown;
        public readonly int level;

        private Tile[,] _tiles;
        private HashSet<Actor> _actors;

        public Floor(int[,] tileData, int level) : base($"Floor_{level}")
        {
            this.level = level;
            _actors = new HashSet<Actor>();
            _tiles = new Tile[tileData.GetLength(0), tileData.GetLength(1)];

            for (int y = 0; y < tileData.GetLength(1); y++)
            {
                for(int x = 0; x < tileData.GetLength(0); x++)
                {
                    Tile.EType tileType = (Tile.EType)tileData[x, y];
                    _tiles[x, y] = Tile.Create(this, new IntVector2(x, y), tileType);

                    switch (tileType)
                    {
                        case Tile.EType.LadderUp:
                            ladderUp = _tiles[x, y];
                            break;
                        case Tile.EType.LadderDown:
                            ladderDown = _tiles[x, y];
                            break;
                    }

                    AddChild(_tiles[x, y]);
                }
            }
        }

        public void AddActor(Actor actor, IntVector2 coord)
        {
            if(!actor.TryOccupyCoord(this, coord, true))
            {
                Debug.LogError($"can't add actor to floor {Name} at coord {coord}");
                return;
            }
            else
            {
                // we've succesfully occupied this coord, so move to it
                actor.Transform.position = coord.ToUnityVector3_XY();
            }

            _actors.Add(actor);
            AddChild(actor, false);
        }

        public void RemoveActor(Actor actor)
        {
            _actors.Remove(actor);
            RemoveChild(actor, false);
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

        protected override void Build()  { }
    }
}