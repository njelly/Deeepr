using System.Collections.Generic;
using Tofunaut.Core;
using Tofunaut.SharpUnity;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeepr.Game
{
    public class Tile : SharpGameObject
    {
        public enum EType
        {
            Empty,
            Wall,
        }

        public readonly IntVector2 coord;
        public readonly Floor floor;

        public virtual Collision.ELayer SolidLayer => _defaultSolidLayer;

        protected virtual string ViewPath => string.Empty;
        protected Collision.ELayer _defaultSolidLayer;
        protected HashSet<Actor> _occupants;

        private GameObject _instantiatedView;

        protected Tile(Floor floor, IntVector2 coord) : base($"Tile {coord}")
        {
            this.floor = floor;
            this.coord = coord;

            _defaultSolidLayer = Collision.ELayer.None;

            LocalPosition = coord.ToUnityVector3_XY();

            _occupants = new HashSet<Actor>();
        }

        protected override void Build()
        {
            if(!string.IsNullOrEmpty(ViewPath))
            {
                AppManager.AssetManager.Load(ViewPath, (bool succesful, GameObject payload) =>
                {
                    if(succesful)
                    {
                        _instantiatedView = Object.Instantiate(payload);
                        _instantiatedView.transform.SetParent(Transform, false);
                    }
                });
            }
        }

        public static Tile Create(Floor floor, IntVector2 coord, EType type)
        {
            switch (type)
            {
                case EType.Empty:
                    return new Tile(floor, coord);
                case EType.Wall:
                    return new WallTile(floor, coord);
                default:
                    Debug.LogError($"unhandled tile type {type}, returning null");
                    return null;
            }
        }

        public bool TrySetOccupant(Actor actor)
        {
            bool canOccupy = false;

            canOccupy |= !actor.CollisionInfo.DoesCollideWith(SolidLayer);
            foreach(Actor occupant in _occupants)
            {
                canOccupy |= !actor.CollisionInfo.DoesCollideWith(occupant.CollisionInfo);
            }

            if(canOccupy)
            {
                _occupants.Add(actor);
            }

            return canOccupy;
        }

        public bool CanSetOccupant(Actor actor)
        {
            bool canOccupy = false;

            canOccupy |= !actor.CollisionInfo.DoesCollideWith(SolidLayer);
            foreach (Actor occupant in _occupants)
            {
                canOccupy |= !actor.CollisionInfo.DoesCollideWith(occupant.CollisionInfo);
            }

            return canOccupy;
        }

        public void RemoveOccupant(Actor occupant)
        {
            _occupants.Remove(occupant);
        }
    }

    public class WallTile : Tile
    {
        protected override string ViewPath => "WallView";

        public WallTile(Floor floor, IntVector2 coord) : base(floor, coord) 
        {
            _defaultSolidLayer = Collision.ELayer.All;
        }
    }
}