using System;
using UnityEngine;

namespace CustomGridSystem
{
    /// <summary>
    /// Do not change the order or index of these directions
    /// <see cref="GridSystemExtension.RotateBy"/> 
    /// </summary>
    public enum Direction    
    {
        Left = 0,
        Up = 1,
        Right = 2,
        Down = 3
    }

    public static class GridSystemExtension
    {
        public static Direction GetDirection(this Vector3 direction) => GetDirection(new Vector2(direction.x, direction.z));
        
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

        public static Direction RotateBy(this Direction direction, int angle)
        {
            int dirOff = angle / 90;
            int newDir = ((int)direction + dirOff) % 4;
            if (newDir < 0) newDir += 4;
            return (Direction)newDir;
        }
    }
}