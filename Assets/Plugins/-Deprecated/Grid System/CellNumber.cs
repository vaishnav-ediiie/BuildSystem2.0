using System;
using System.Collections.Generic;


namespace KSRecs.Deprecated.GridSystemSpace
{
    [Serializable]
    public struct CellNumber
    {
        public int row;
        public int column;

        public CellNumber(int row, int column)
        {
            this.row = row;
            this.column = column;
        }

        public static CellNumber zero => new CellNumber();
        public static CellNumber one => new CellNumber(1, 1);


        public static IEnumerable<CellNumber> LoopOver(CellNumber start, CellNumber end)
        {
            CellNumber cn = new CellNumber();
            for (int c = start.column; c < end.column; c++)
            {
                for (int r = start.row; r < end.row; r++)
                {
                    cn.column = c;
                    cn.row = r;
                    yield return cn;
                }
            }
        }

        public static IEnumerable<CellNumber> LoopOverComplete(CellNumber start, CellNumber end)
        {
            CellNumber cn = new CellNumber();
            for (int c = start.column; c <= end.column; c++)
            {
                for (int r = start.row; r <= end.row; r++)
                {
                    cn.column = c;
                    cn.row = r;
                    yield return cn;
                }
            }
        }

        public override string ToString() => $"Cell({row}, {column})";

        public static bool operator ==(CellNumber a, CellNumber b) => a.column == b.column && a.row == b.row;
        public static bool operator !=(CellNumber a, CellNumber b) => a.column != b.column || a.row != b.row;
        public static CellNumber operator +(CellNumber a, CellNumber b) => new CellNumber(a.row + b.row, a.column + b.column);
        public static CellNumber operator -(CellNumber a, CellNumber b) => new CellNumber(a.row - b.row, a.column - b.column);
        public static CellNumber operator *(CellNumber a, CellNumber b) => new CellNumber(a.row * b.row, a.column * b.column);
        public static CellNumber operator /(CellNumber a, CellNumber b) => new CellNumber(a.row / b.row, a.column / b.column);
    }
}