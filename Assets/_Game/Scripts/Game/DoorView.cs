using Tofunaut.Core;
using UnityEngine;

namespace Tofunaut.Deeepr.Game
{
    //public class DoorView : TileView
    //{
    //    [Header("Door")]
    //    [SerializeField] private Animator _animator;
    //
    //    private void Update()
    //    {
    //        _animator.SetBool("open", !(Tile as DoorTile).IsClosed);
    //    }
    //
    //    public override void Initialize(Tile tile)
    //    {
    //        base.Initialize(tile);
    //
    //        Tile leftTile = tile.floor.GetTile(tile.coord + IntVector2.Left);
    //        Tile rightTile = tile.floor.GetTile(tile.coord + IntVector2.Right);
    //
    //        bool blockedLeft = leftTile == null || (leftTile.SolidLayer & Collision.ELayer.Actor) != 0;
    //        bool blockedRight = rightTile == null || (rightTile.SolidLayer & Collision.ELayer.Actor) != 0;
    //
    //        _animator.SetBool("up_down", blockedLeft && blockedRight);
    //        _animator.SetBool("open", !(Tile as DoorTile).IsClosed);
    //    }
    //}
}