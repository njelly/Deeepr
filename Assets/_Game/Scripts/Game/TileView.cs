using System;
using System.Collections.Generic;
using Tofunaut.Core;
using Tofunaut.SharpUnity;
using UnityEngine;
using UnityEngine.U2D;

namespace Tofunaut.Deeepr.Game
{
    public abstract class TileView : SharpSprite
    {


        private const string SpriteAtlasPath = "sprite_atlas_16";
        protected static Sprite[] _sprites;

        public static int NumSprites => _sprites.Length;

        protected readonly Tile _tile;

        protected TileView(Tile tile) : base($"{tile?.Name ?? "NULL"}_View", null)
        {
            _tile = tile;
        }

        public static void PreloadSpriteAtlas(Action onComplete)
        {
            AppManager.AssetManager.LoadList(SpriteAtlasPath, (bool succesful, List<Sprite> payload) =>
            {
                if (succesful)
                {
                    _sprites = payload.ToArray();
                    onComplete?.Invoke();
                }
            });
        }

        public virtual void SetSprite() { }
    }

    public class WallTileView : TileView
    {
        [Flags]
        public enum EAdjacentTileFlags
        {
            TopLeft = 1 << 0,
            Top = 1 << 1,
            TopRight = 1 << 2,
            Left = 1 << 3,
            Right = 1 << 4,
            BottomLeft = 1 << 5,
            Bottom = 1 << 6,
            BottomRight = 1 << 7,
        }

        public static bool IsOuterCornerBottomLeft(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Left) != EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Bottom) != EAdjacentTileFlags.Bottom
                && (v & EAdjacentTileFlags.Right) == EAdjacentTileFlags.Right;
        }

        public static bool IsSideBottom(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Left) == EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Bottom) != EAdjacentTileFlags.Bottom
                && (v & EAdjacentTileFlags.Right) == EAdjacentTileFlags.Right;
        }

        public static bool IsOuterCornerBottomRight(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Left) == EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Bottom) != EAdjacentTileFlags.Bottom
                && (v & EAdjacentTileFlags.Right) != EAdjacentTileFlags.Right;
        }

        public static bool IsSideLeft(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Left) != EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Bottom) == EAdjacentTileFlags.Bottom
                && (v & EAdjacentTileFlags.Right) == EAdjacentTileFlags.Right
                && (v & EAdjacentTileFlags.BottomLeft) != EAdjacentTileFlags.BottomLeft
                && (v & EAdjacentTileFlags.BottomRight) == EAdjacentTileFlags.BottomRight;
        }

        public static bool IsCenter(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Left) == EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Bottom) == EAdjacentTileFlags.Bottom
                && (v & EAdjacentTileFlags.Right) == EAdjacentTileFlags.Right
                && (v & EAdjacentTileFlags.BottomRight) == EAdjacentTileFlags.BottomRight
                && (v & EAdjacentTileFlags.BottomLeft) == EAdjacentTileFlags.BottomLeft;
        }

        public static bool IsSideRight(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Left) == EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Bottom) == EAdjacentTileFlags.Bottom
                && (v & EAdjacentTileFlags.Right) != EAdjacentTileFlags.Right
                && (v & EAdjacentTileFlags.BottomLeft) == EAdjacentTileFlags.BottomLeft
                && (v & EAdjacentTileFlags.BottomRight) != EAdjacentTileFlags.BottomRight;
        }

        public static bool IsInnerCornerTopRight(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Left) == EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Bottom) == EAdjacentTileFlags.Bottom
                && (v & EAdjacentTileFlags.Right) == EAdjacentTileFlags.Right
                && (v & EAdjacentTileFlags.BottomRight) == EAdjacentTileFlags.BottomRight
                && (v & EAdjacentTileFlags.BottomLeft) != EAdjacentTileFlags.BottomLeft;
        }

        public static bool IsInnerCornerTopLeft(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Left) == EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Bottom) == EAdjacentTileFlags.Bottom
                && (v & EAdjacentTileFlags.Right) == EAdjacentTileFlags.Right
                && (v & EAdjacentTileFlags.BottomRight) != EAdjacentTileFlags.BottomRight
                && (v & EAdjacentTileFlags.BottomLeft) == EAdjacentTileFlags.BottomLeft;
        }

        public static bool IsBottomEndCap(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Left) != EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Bottom) != EAdjacentTileFlags.Bottom
                && (v & EAdjacentTileFlags.Right) != EAdjacentTileFlags.Right;
        }

        public static bool IsTJoinBottom(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Left) == EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Bottom) == EAdjacentTileFlags.Bottom
                && (v & EAdjacentTileFlags.Right) == EAdjacentTileFlags.Right
                && (v & EAdjacentTileFlags.BottomRight) != EAdjacentTileFlags.BottomRight
                && (v & EAdjacentTileFlags.BottomLeft) != EAdjacentTileFlags.BottomLeft;
        }

        public static bool IsLeftInnerCornerTopLeft(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Left) != EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Bottom) == EAdjacentTileFlags.Bottom
                && (v & EAdjacentTileFlags.Right) == EAdjacentTileFlags.Right
                && (v & EAdjacentTileFlags.BottomRight) != EAdjacentTileFlags.BottomRight;
        }

        public static bool IsRightInnerCornerTopRight(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Left) == EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Bottom) == EAdjacentTileFlags.Bottom
                && (v & EAdjacentTileFlags.Right) != EAdjacentTileFlags.Right
                && (v & EAdjacentTileFlags.BottomLeft) != EAdjacentTileFlags.BottomLeft;
        }

        public static bool IsTopDownPipe(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Left) != EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Bottom) == EAdjacentTileFlags.Bottom
                && (v & EAdjacentTileFlags.Right) != EAdjacentTileFlags.Right
                && (v & EAdjacentTileFlags.BottomLeft) != EAdjacentTileFlags.BottomLeft
                && (v & EAdjacentTileFlags.BottomRight) != EAdjacentTileFlags.BottomRight;
        }

        public static bool IsBottomRightInner(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Left) != EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Bottom) == EAdjacentTileFlags.Bottom
                && (v & EAdjacentTileFlags.BottomLeft) == EAdjacentTileFlags.BottomLeft
                && (v & EAdjacentTileFlags.BottomRight) == EAdjacentTileFlags.BottomRight;
        }
        
        public static bool IsBottomLeftInner(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Right) != EAdjacentTileFlags.Right
                && (v & EAdjacentTileFlags.Bottom) == EAdjacentTileFlags.Bottom
                && (v & EAdjacentTileFlags.BottomLeft) == EAdjacentTileFlags.BottomLeft
                && (v & EAdjacentTileFlags.BottomRight) == EAdjacentTileFlags.BottomRight;
        }

        public static bool IsOverHangTopLeft(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Top) != EAdjacentTileFlags.Top
                && (v & EAdjacentTileFlags.TopRight) != EAdjacentTileFlags.TopRight
                && (v & EAdjacentTileFlags.Left) != EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Right) == EAdjacentTileFlags.Right;
        }

        public static bool IsOverHangTop(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Top) != EAdjacentTileFlags.Top
                && (v & EAdjacentTileFlags.Left) == EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Right) == EAdjacentTileFlags.Right;
        }

        public static bool IsOverHangTopRight(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Top) != EAdjacentTileFlags.Top
                && (v & EAdjacentTileFlags.TopLeft) != EAdjacentTileFlags.TopLeft
                && (v & EAdjacentTileFlags.Left) == EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Right) != EAdjacentTileFlags.Right;
        }

        public static bool IsOverHangEndCapTop(EAdjacentTileFlags v)
        {
            return (v & EAdjacentTileFlags.Top) != EAdjacentTileFlags.Top
                && (v & EAdjacentTileFlags.Left) != EAdjacentTileFlags.Left
                && (v & EAdjacentTileFlags.Right) != EAdjacentTileFlags.Right;
        }

        private SharpSprite _overhangSprite;

        public WallTileView(Tile tile) : base(tile) { }

        public override void SetSprite()
        {

            int spriteIndex = 9;
            int overhangSpriteIndex = -1;

            Tile topTile = _tile.floor.GetTile(_tile.coord + IntVector2.Up);
            Tile bottomTile = _tile.floor.GetTile(_tile.coord + IntVector2.Down);
            Tile leftTile = _tile.floor.GetTile(_tile.coord + IntVector2.Left);
            Tile rightTile = _tile.floor.GetTile(_tile.coord + IntVector2.Right);
            Tile topLeftTile = _tile.floor.GetTile(_tile.coord + IntVector2.Up + IntVector2.Left);
            Tile topRightTile = _tile.floor.GetTile(_tile.coord + IntVector2.Up + IntVector2.Right);
            Tile bottomLeftTile = _tile.floor.GetTile(_tile.coord + IntVector2.Down + IntVector2.Left);
            Tile bottomRightTile = _tile.floor.GetTile(_tile.coord + IntVector2.Down + IntVector2.Right);

            bool IsWall(Tile tile)
            {
                return tile == null || tile.Type == Tile.EType.Wall;
            }

            EAdjacentTileFlags mask = 0;
            // for the main tile, we only care about left, right, bottom, 
            if (IsWall(leftTile))
            {
                mask |= EAdjacentTileFlags.Left;
            }
            if (IsWall(rightTile))
            {
                mask |= EAdjacentTileFlags.Right;
            }
            if (IsWall(bottomLeftTile))
            {
                mask |= EAdjacentTileFlags.BottomLeft;
            }
            if (IsWall(bottomTile))
            {
                mask |= EAdjacentTileFlags.Bottom;
            }
            if (IsWall(bottomRightTile))
            {
                mask |= EAdjacentTileFlags.BottomRight;
            }
            if (IsWall(topLeftTile))
            {
                mask |= EAdjacentTileFlags.TopLeft;
            }
            if (IsWall(topTile))
            {
                mask |= EAdjacentTileFlags.Top;
            }
            if (IsWall(topRightTile))
            {
                mask |= EAdjacentTileFlags.TopRight;
            }

            if(leftTile != null && leftTile.Type == Tile.EType.Door)
            {
                Debug.Log($"mask: {(int)mask}, coord: {_tile.coord}");
            }

            // determine main sprite
            if (IsOuterCornerBottomLeft(mask))
            {
                spriteIndex = 5;
            }
            else if (IsSideBottom(mask))
            {
                spriteIndex = 6;
            }
            else if (IsOuterCornerBottomRight(mask))
            {
                spriteIndex = 7;
            }
            else if (IsSideLeft(mask))
            {
                spriteIndex = 8;
            }
            else if (IsSideRight(mask))
            {
                spriteIndex = 10;
            }
            else if (IsInnerCornerTopRight(mask))
            {
                spriteIndex = 11;
            }
            else if (IsInnerCornerTopLeft(mask))
            {
                spriteIndex = 12;
            }
            else if (IsBottomEndCap(mask))
            {
                spriteIndex = 13;
            }
            else if (IsTJoinBottom(mask))
            {
                spriteIndex = 14;
            }
            else if (IsLeftInnerCornerTopLeft(mask))
            {
                spriteIndex = 15;
            }
            else if (IsTopDownPipe(mask))
            {
                spriteIndex = 16;
            }
            else if (IsRightInnerCornerTopRight(mask))
            {
                spriteIndex = 17;
            }
            else if (IsBottomRightInner(mask))
            {
                spriteIndex = 18;
            }
            else if (IsBottomLeftInner(mask))
            {
                spriteIndex = 19;
            }
            else if (IsCenter(mask))
            {
                spriteIndex = 9;
            }
            Sprite = _sprites[spriteIndex];

            // determine overhang
            if (IsOverHangTopLeft(mask))
            {
                overhangSpriteIndex = 20;
            }
            if (IsOverHangTop(mask))
            {
                overhangSpriteIndex = 21;
            }
            if (IsOverHangTopRight(mask))
            {
                overhangSpriteIndex = 22;
            }
            if (IsOverHangEndCapTop(mask))
            {
                overhangSpriteIndex = 23;
            }

            if (_overhangSprite != null)
            {
                RemoveChild(_overhangSprite, true);
            }
            if (overhangSpriteIndex > 0)
            {
                _overhangSprite = new SharpSprite($"{Name}_overhang", _sprites[overhangSpriteIndex]);
                _overhangSprite.LocalPosition = new Vector3(0f, 1f, 0f);
                _overhangSprite.Layer = this.Layer + 1;
                AddChild(_overhangSprite);
            }
        }
    }
}