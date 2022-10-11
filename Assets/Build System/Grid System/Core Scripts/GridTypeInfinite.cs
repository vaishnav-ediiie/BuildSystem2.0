using UnityEngine;

namespace CustomGridSystem
{
    internal struct GridTypeInfinite : IGridType
    {
        public bool IsCellNumberValid(CellNumber number)
        {
            return (number.row >= 0) && (number.column >= 0);
        }

        public bool IsEdgeNumberValid(EdgeNumber number)
        {
            return IsCellNumberValid(number.CellAfter);
        }

        public CellNumber ValidateCellNumber(CellNumber number)
        {
            return new CellNumber(Mathf.Max(number.row, 0), Mathf.Max(number.column, 0));
        }

        CellNumber IGridType.LastCellNumber => CellNumber.Max;
    }
}