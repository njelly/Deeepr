using UnityEngine;

namespace Tofunaut.Deeepr.Game
{
    public class TileView : MonoBehaviour
    {
        public bool Initialized { get; private set; }
        public Tile Tile { get; private set; }

        [SerializeField] protected SpriteRenderer _spriteRenderer;
        [SerializeField] protected ColorPallet.EColor _color;

        protected virtual void OnEnable()
        {
            if(Tile != null)
            {
                Initialize(Tile);
            }
        }

        public virtual void Initialize(Tile tile)
        {
            Tile = tile;
            _spriteRenderer.color = AppManager.ColorPallet.Get(_color, ColorPallet.EAlpha.Solid);
        }
    }
}