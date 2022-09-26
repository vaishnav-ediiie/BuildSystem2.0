using CustomGridSystem;
using System;

namespace CustomGridSystem
{
    /// <summary>
    /// Representation of a Cell's Position in 2D grid.
    /// </summary>
    [Serializable]
    public struct CellNumber : IEquatable<CellNumber>, IFormattable, IGridNumber
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

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"[{row}, {column}]";
        }

        public override bool Equals(object obj) => obj is CellNumber other && Equals(other);
        public override int GetHashCode() => $"{row}{column}".GetHashCode();

        /// <summary> Shorthand for new CellNumber(0, 0) </summary>
        public static CellNumber Zero => new CellNumber();

        /// <summary> Shorthand for new CellNumber(int.MaxValue, int.MaxValue) </summary>
        public static CellNumber Max => new CellNumber(int.MaxValue, int.MaxValue);

        /// <summary> Shorthand for new CellNumber(int.MinValue, int.MinValue) </summary>
        public static CellNumber Min => new CellNumber(int.MinValue, int.MinValue);

        public static bool operator ==(CellNumber a, CellNumber b) => a.column == b.column && a.row == b.row;
        public static bool operator !=(CellNumber a, CellNumber b) => a.column != b.column || a.row != b.row;
        public static CellNumber operator +(CellNumber a, CellNumber b) => new CellNumber(a.row + b.row, a.column + b.column);
        public static CellNumber operator -(CellNumber a, CellNumber b) => new CellNumber(a.row - b.row, a.column - b.column);
    }
}