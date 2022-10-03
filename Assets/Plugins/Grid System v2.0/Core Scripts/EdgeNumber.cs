using System;

namespace CustomGridSystem
{
    /// <summary>
    /// Representation of a Edge's Position in 2D grid.
    /// </summary>
    [Serializable]
    public struct EdgeNumber : IEquatable<EdgeNumber>, IGridNumber
    {
       /// <summary>For horizontal cellType, returns Cell right of this edge; For vertical cellType, returns Cell below this edge. </summary>
        public CellNumber CellAfter { get; }

        /// <summary>For horizontal cellType, returns Cell left of this edge; For vertical cellType, returns Cell above this edge. </summary>
        public CellNumber CellBefore
        {
            get
            {
                if (edgeType == EdgeType.Vertical) return new CellNumber(CellAfter.row - 1, CellAfter.column);
                return new CellNumber(CellAfter.row, CellAfter.column - 1);
            }
        }
        
        public EdgeType edgeType;

        public EdgeNumber(int row, int column, EdgeType edgeType)
        {
            this.CellAfter = new CellNumber(row, column);
            this.edgeType = edgeType;
        }

        internal EdgeNumber(CellNumber cellAfterNumber, EdgeType edgeType)
        {
            this.CellAfter = cellAfterNumber;
            this.edgeType = edgeType;
        }

        public bool Equals(EdgeNumber other)
        {
            return (other.CellAfter == this.CellAfter) && (other.edgeType == this.edgeType);
        }

        
        public override bool Equals(object obj) => obj is EdgeNumber other && Equals(other);
        public override int GetHashCode() => $"{CellAfter.row}{CellAfter.column}{edgeType}".GetHashCode();
        public override string ToString() => $"[{CellAfter.row}, {CellAfter.column}, {edgeType}]";

        public static implicit operator string(EdgeNumber cellNumber) => $"[{cellNumber.CellAfter.row}, {cellNumber.CellAfter.column}, {cellNumber.edgeType}]";

        public static bool operator ==(EdgeNumber a, EdgeNumber b) => a.CellAfter == b.CellAfter && a.edgeType == b.edgeType;
        public static bool operator !=(EdgeNumber a, EdgeNumber b) => a.CellAfter != b.CellAfter || a.edgeType != b.edgeType;

        /// <summary>If using finite grid then you should use "Grid.ValidateEdgeNumber" to make sure you get a valid cell</summary>
        public static EdgeNumber operator +(EdgeNumber edgeNumber, CellNumber numberOfCells) => new EdgeNumber(edgeNumber.CellAfter + numberOfCells, edgeNumber.edgeType);

        /// <summary>If using finite grid then you should use "Grid.ValidateEdgeNumber" to make sure you get a valid cell</summary>
        public static EdgeNumber operator +(CellNumber numberOfCells, EdgeNumber edgeNumber) => new EdgeNumber(edgeNumber.CellAfter + numberOfCells, edgeNumber.edgeType);

        /// <summary>If using finite grid then you should use "Grid.ValidateEdgeNumber" to make sure you get a valid cell</summary>
        public static EdgeNumber operator -(EdgeNumber edgeNumber, CellNumber numberOfCells) => new EdgeNumber(edgeNumber.CellAfter - numberOfCells, edgeNumber.edgeType);

        /// <summary>If using finite grid then you should use "Grid.ValidateEdgeNumber" to make sure you get a valid cell</summary>
        public static EdgeNumber operator -(CellNumber numberOfCells, EdgeNumber edgeNumber) => new EdgeNumber(edgeNumber.CellAfter - numberOfCells, edgeNumber.edgeType);
    }
}