using System;
using UnityEngine;

namespace CustomGridSystem
{
    internal struct GridTypeInfinite : IGridType
    {
        public bool IsCellNumberValid(int row, int col)
        {
            return (row >= 0) && (col >= 0);
        }

        public CellNumber ValidateCellNumber(CellNumber number)
        {
            return new CellNumber(Mathf.Max(number.row, 0), Mathf.Max(number.column, 0));
        }

        CellNumber IGridType.LastCellNumber => CellNumber.Max;
    }
}