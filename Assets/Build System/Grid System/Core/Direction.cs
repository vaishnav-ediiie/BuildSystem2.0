using UnityEngine;

namespace CustomGridSystem
{
    /// <summary>
    /// Do not change the order or index of these directions
    /// </summary>
    /// <example>
    /// Some awesome properties of these indices:
    ///     (Up + 1) % 4   =   (1 + 1) % 4 = 2   =   Right       (Up rotated by 90°  (1 * 90°) is Right)
    ///     (Up + 2) % 4   =   (1 + 2) % 4 = 3   =   Down        (Up rotated by 180° (2 * 90°) is Down)
    ///     (Up + 3) % 4   =   (1 + 3) % 4 = 0   =   Left        (Up rotated by 270° (3 * 90°) is Left)
    ///     (Up + 4) % 4   =   (1 + 4) % 4 = 1   =   Up          (Up rotated by 360° (4 * 90°) is Up)
    /// Same logic works for all the direction and all rotation (Even for rotations beyond 0 to 360°)
    /// </example>
    /// <see cref="GridSystemExtension.RotateBy"/> 
    public enum Direction    
    {
        Left = 0,
        Up = 1,
        Right = 2,
        Down = 3
    }

    public static class GridSystemExtension
    {
        /// <returns> Direction in which given Vector is inclined the most (Only x & z components are used) </returns>
        public static Direction GetDirection(this Vector3 direction) => GetDirection(new Vector2(direction.x, direction.z));
        
        /// <returns> Direction in which given Vector is inclined the most</returns>
        public static Direction GetDirection(this Vector2 direction)
        {
            direction = direction.normalized;
            float x = Mathf.Abs(direction.x);
            float y = Mathf.Abs(direction.y);
            if (x > y)
            {
                if (direction.x > 0) return Direction.Right;
                return Direction.Left;
            } 
            if (direction.y > 0) return Direction.Up;
            return Direction.Down;
        }

        /// <summary> To understand the logic <see cref="Direction"/> </summary>
        /// <param name="direction">Direction to rotate</param>
        /// <param name="angle">Angle by which to rotate</param>
        /// <returns>Direction rotated by given angle</returns>
        public static Direction RotateBy(this Direction direction, int angle)
        {
            
            int dirOff = angle / 90;
            int newDir = ((int)direction + dirOff) % 4; 
            if (newDir < 0) newDir += 4;
            return (Direction)newDir;
        }
    }
}