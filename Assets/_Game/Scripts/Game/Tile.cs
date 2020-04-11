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
            LadderUp,
            LadderDown,
            Door,
        }

        public readonly IntVector2 coord;
        public readonly Floor floor;

        public virtual Collision.ELayer SolidLayer => _defaultSolidLayer;
        public IReadOnlyCollection<Actor> Occupants => new List<Actor>(_occupants).AsReadOnly();

        protected virtual string ViewPath => string.Empty;
        protected Collision.ELayer _defaultSolidLayer;
        protected HashSet<Actor> _occupants;

        private TileView _instantiatedView;

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
                        _instantiatedView = Object.Instantiate(payload).GetComponent<TileView>();
                        _instantiatedView.Initialize(this);
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
                case EType.LadderUp:
                    return new LadderUpTile(floor, coord);
                case EType.LadderDown:
                    return new LadderDownTile(floor, coord);
                case EType.Door:
                    return new DoorTile(floor, coord);
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

        public virtual void InteractWith(Actor interactor) { }
    }

    public class WallTile : Tile
    {
        protected override string ViewPath => "WallView";

        public WallTile(Floor floor, IntVector2 coord) : base(floor, coord) 
        {
            _defaultSolidLayer = Collision.ELayer.All;
        }
    }

    public class LadderUpTile : Tile
    {
        protected override string ViewPath => "LadderUpView";

        public LadderUpTile(Floor floor, IntVector2 coord) : base(floor, coord)
        {
            _defaultSolidLayer = Collision.ELayer.Ground;
        }
    }
    public class LadderDownTile : Tile
    {
        protected override string ViewPath => "LadderDownView";

        public LadderDownTile(Floor floor, IntVector2 coord) : base(floor, coord)
        {
            _defaultSolidLayer = Collision.ELayer.Ground;
        }
    }
    public class DoorTile : Tile
    {
        protected override string ViewPath => "DoorView";

        public bool IsClosed { get; private set; }

        public override Collision.ELayer SolidLayer => IsClosed ? _defaultSolidLayer : Collision.ELayer.Ground;

        public DoorTile(Floor floor, IntVector2 coord) : base(floor, coord)
        {
            _defaultSolidLayer = Collision.ELayer.All;
            IsClosed = true;
        }

        public override void InteractWith(Actor interactor)
        {
            IsClosed = !IsClosed;
        }
    }
}