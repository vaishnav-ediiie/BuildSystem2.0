using UnityEngine;

namespace CustomGridSystem
{
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    public static class GridSystemExtension
    {
        public static Direction GetEdgeDirection(this Vector3 direction) => GetEdgeDirection(new Vector2(direction.x, direction.z));

        static bool IsIn(this float number, float bond1, float bond2)
        {
            if (bond1 < bond2) return bond1 < number && number < bond2;
            return bond2 < number && number < bond1;
        }
        
        public static Direction GetEdgeDirection(this Vector2 direction)
        {
            float angle = Vector2.SignedAngle(direction, Vector2.left);
            if (angle.IsIn(  0f,  45f) || angle.IsIn( -45f,    0f)) return Direction.Left;
            if (angle.IsIn(135f, 180f) || angle.IsIn(-135f, -180f)) return Direction.Right;
            if (angle.IsIn( 45f, 135f)) return Direction.Up;
            return Direction.Down;
        }
    }
}