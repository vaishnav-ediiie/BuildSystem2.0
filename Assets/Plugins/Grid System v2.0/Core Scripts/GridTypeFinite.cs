using System;
using UnityEngine;

namespace CustomGridSystem
{
    internal struct GridTypeFinite : IGridType
    {
        internal CellNumber lastCellNumber;

        public GridTypeFinite(CellNumber lastCellNumber)
        {
            this.lastCellNumber = lastCellNumber;
        }

        public bool IsCellNumberValid(CellNumber number)
        {
            return (number.row >= 0) && (number.column >= 0) && (number.row < lastCellNumber.row) && (number.column < lastCellNumber.column);
        }

        public bool IsEdgeNumberValid(EdgeNumber number)
        {
            if (number.CellAfter.row < 0 || number.CellAfter.column < 0) return false;
            
            switch (number.edgeType)
            {
                case EdgeType.Horizontal:
                    return number.CellAfter.row < lastCellNumber.row && number.CellAfter.column < lastCellNumber.column + 1;  
                case EdgeType.Vertical:
                    return number.CellAfter.row < lastCellNumber.row + 1 && number.CellAfter.column < lastCellNumber.column;  
                default:
                    throw new NotImplementedException($"Cannot understand direction: {number.edgeType}");
            }
        }

        public CellNumber ValidateCellNumber(CellNumber number)
        {
            return new CellNumber(Mathf.Clamp(number.row, 0, lastCellNumber.row-1), Mathf.Clamp(number.column, 0, lastCellNumber.column-1));
        }

        public CellNumber LastCellNumber => lastCellNumber;
    }
}