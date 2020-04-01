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

        private GameObject _instantiatedView;

        protected Tile(Floor floor, IntVector2 coord) : base($"Tile {coord}")
        {
            this.floor = floor;
            this.coord = coord;

            _defaultSolidLayer = Collision.ELayer.None;

            LocalPosition = coord.ToUnityVector3_XY();
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