using CustomGridSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

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


        public EdgeNumber GetEdgeNumber(Direction direction)
        {
            if (direction == Direction.Down) return new EdgeNumber(this.row, this.column, EdgeType.Vertical);
            if (direction == Direction.Up)   return new EdgeNumber(this.row + 1, this.column, EdgeType.Vertical);
            if (direction == Direction.Left) return new EdgeNumber(this.row, this.column, EdgeType.Horizontal);
                                             return new EdgeNumber(this.row, this.column + 1, EdgeType.Horizontal);
        }


        public override bool Equals(object obj) => obj is CellNumber other && Equals(other);
        public override int GetHashCode() => $"{row}{column}".GetHashCode();
        public override string ToString() => $"[{row}, {column}]";


        /// <summary> Shorthand for new CellNumber(0, 0) </summary>
        public static CellNumber Zero => new CellNumber();

        /// <summary> Shorthand for new CellNumber(1, 1) </summary>
        public static CellNumber One => new CellNumber(1, 1);
        
        /// <summary> Shorthand for new CellNumber(int.MaxValue, int.MaxValue) </summary>
        public static CellNumber Max => new CellNumber(int.MaxValue, int.MaxValue);

        /// <summary> Shorthand for new CellNumber(int.MinValue, int.MinValue) </summary>
        public static CellNumber Min => new CellNumber(int.MinValue, int.MinValue);


        public static bool operator ==(CellNumber a, CellNumber b) => a.column == b.column && a.row == b.row;
        public static bool operator !=(CellNumber a, CellNumber b) => a.column != b.column || a.row != b.row;
        public static CellNumber operator +(CellNumber a, CellNumber b) => new CellNumber(a.row + b.row, a.column + b.column);
        public static CellNumber operator -(CellNumber a, CellNumber b) => new CellNumber(a.row - b.row, a.column - b.column);
        
        /// <summary>
        /// 
        /// </summary>
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