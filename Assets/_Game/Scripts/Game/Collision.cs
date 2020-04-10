using System;

namespace Tofunaut.Deeepr.Game
{
    public static class Collision
    {
        [Flags]
        public enum ELayer
        {
            None = 0,
            All = ~None,

            Actor = 1,
            Ground = 2,
            Airborn = 4,
        }
    }

    [Serializable]
    public class CollisionInfo
    {
        public Collision.ELayer solidLayer; // what am I?
        public Collision.ELayer collsionLayer; // what do I collide with?

        /// <summary>
        /// Returns true if this collision info will collide with some other collision info
        /// </summary>
        public bool DoesCollideWith(CollisionInfo other) => DoesCollideWith(other.solidLayer);
        public bool DoesCollideWith(Collision.ELayer solidLayer)
        {
            return (collsionLayer & solidLayer) != 0;
        }
    }

    public interface ICollider
    {
        CollisionInfo CollisionInfo { get; }
    }
}