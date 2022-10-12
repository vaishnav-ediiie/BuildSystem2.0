using System;
using System.Collections.Generic;

namespace CustomGridSystem
{
    /// <summary>
    /// Representation of a Cell's Position in 2D grid.
    /// </summary>
    [Serializable]
    public struct CellNumber : IEquatable<CellNumber>, IGridNumber
    {
        public int row;
        public int column;

        public CellNumber(int row, int column)
        {
            this.row = row;
            this.column = column;
        }

        public bool Equals(CellNumber other)
        {
            return other.row == row && other.column == column;
        }
        
        /// <returns>Raw EdgeNumber for the edge of this cell and in specified direction.</returns>
        /// <remarks>Raw EdgeNumber meaning this edge number may or may not be valid for the grid being used</remarks>
        /// <exception cref="NotImplementedException">If the direction is other than Up, Down, Left, Right</exception>
        public EdgeNumber GetEdgeNumber(Direction direction)
        {
            return direction switch
            {
                Direction.Down  => new EdgeNumber(this.row, this.column, EdgeType.Vertical),
                Direction.Up    => new EdgeNumber(this.row + 1, this.column, EdgeType.Vertical),
                Direction.Left  => new EdgeNumber(this.row, this.column, EdgeType.Horizontal),
                Direction.Right => new EdgeNumber(this.row, this.column + 1, EdgeType.Horizontal),
                _ => throw new NotImplementedException($"Unknown direction {direction}")
            };
        }

        /// <summary> Rotates a CellNumber by given angle. Think of cell number as Vector2Int, so we can rotate it. </summary>
        /// <param name="angle">Clock-wise angle</param>
        /// <returns>Rotated CellNumber (Raw)</returns>
        /// <remarks>Raw CellNumber meaning this number may or may not be valid for the grid being used</remarks>
        /// <exception cref="NotImplementedException"></exception>
        public CellNumber Rotate(int angle)
        {
            float angleInner = angle % 360;
            if (angleInner < 0) angleInner += 360;
            
            return angleInner switch
            {
                0 =>   new CellNumber(+row, +column),
                90 =>  new CellNumber(-column, +row),
                180 => new CellNumber(-row, -column),
                270 => new CellNumber(+column, -row),
                _ => throw new NotImplementedException($"RotationFactor Not implementer for angle {angle}")
            };
        }

        public override bool Equals(object obj) => obj is CellNumber other && Equals(other);
        public override int GetHashCode() => ToString().GetHashCode();
        public override string ToString() => $"[{row}, {column}]";


        /// <summary> Shorthand for new CellNumber(0, 0) </summary>
        public static CellNumber Zero => new CellNumber();

        /// <summary> Shorthand for new CellNumber(1, 1) </summary>
        public static CellNumber One => new CellNumber(1, 1);
        
        /// <summary> Shorthand for new CellNumber(int.MaxValue, int.MaxValue) </summary>
        public static CellNumber Max => new CellNumber(int.MaxValue, int.MaxValue);

        /// <summary> Shorthand for new CellNumber(int.MinValue, int.MinValue) </summary>
        public static CellNumber Min => new CellNumber(int.MinValue, int.MinValue);

        /// <summary> Shorthand for new CellNumber(-1, 0) </summary>
        public static CellNumber Left => new CellNumber(-1, 0);
        
        /// <summary> Shorthand for new CellNumber(1, 0) </summary>
        public static CellNumber Right => new CellNumber(1, 0);
        
        /// <summary> Shorthand for new CellNumber(0, 1) </summary>
        public static CellNumber Front => new CellNumber(0, 1);
        
        /// <summary> Shorthand for new CellNumber(0, -1) </summary>
        public static CellNumber Back => new CellNumber(0, -1);
        
        
        public static bool operator ==(CellNumber a, CellNumber b) => a.column == b.column && a.row == b.row;
        public static bool operator !=(CellNumber a, CellNumber b) => a.column != b.column || a.row != b.row;
        public static CellNumber operator +(CellNumber a, CellNumber b) => new CellNumber(a.row + b.row, a.column + b.column);
        public static CellNumber operator -(CellNumber a, CellNumber b) => new CellNumber(a.row - b.row, a.column - b.column);
        public static CellNumber operator *(CellNumber a, CellNumber b) => new CellNumber(a.row * b.row, a.column * b.column);
        
        public static CellNumber operator +(CellNumber a, int b) => new CellNumber(a.row + b, a.column + b);
        public static CellNumber operator -(CellNumber a, int b) => new CellNumber(a.row - b, a.column - b);
        public static CellNumber operator *(CellNumber a, int b) => new CellNumber(a.row * b, a.column * b);
        
        public static implicit operator string(CellNumber cellNumber) => cellNumber.ToString();

        public static CellNumber operator -(CellNumber a) => new CellNumber(-a.row, -a.column);

        public static IEnumerable<CellNumber> LoopCells(CellNumber startCell, CellNumber endCell)
        {
            for (int row = startCell.row; row < endCell.row; row++)
            {
                for (int col = startCell.column; col < endCell.column; col++)
                {
                    yield return new CellNumber(row, col);
                }
            }

            yield break;
        }
    }
}