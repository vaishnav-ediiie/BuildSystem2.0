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

        public bool IsCellNumberValid(int row, int col)
        {
            return (row >= 0) && (col >= 0) && (row < lastCellNumber.row) && (col < lastCellNumber.column);
        }

        public CellNumber ValidateCellNumber(CellNumber number)
        {
            return new CellNumber(Mathf.Clamp(number.row, 0, lastCellNumber.row-1), Mathf.Clamp(number.column, 0, lastCellNumber.column-1));
        }

        public CellNumber LastCellNumber => lastCellNumber;
    }
}