using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public class CellLayoutInfo
    {
        public CellNumber LocalCenter { get; }
        public CellNumber TotalSize { get; }
        public CellNumber BottomLeft { get; }
        public CellNumber TopRight { get; }
        public int Rotation { get; }
        public bool IsSingleCelled => this.TotalSize.row <= 1 && this.TotalSize.column <= 0;

        public CellNumber TopLeft => new CellNumber(TopRight.row, BottomLeft.column);
        public CellNumber BottomRight => new CellNumber(BottomLeft.row, TopRight.column);

        public CellNumber Left => CellNumber.Left.Rotate(Rotation);
        public CellNumber Front => CellNumber.Front.Rotate(Rotation);
        
        public CellLayoutInfo(CellNumber localCenter, CellNumber totalSize, CellNumber current, int rotation)
        {
            LocalCenter = localCenter;
            TotalSize = totalSize;


            if (IsSingleCelled)
            {
                BottomLeft = current;
                TopRight = current;
            }
            else
            {
                Rotation = rotation;
                CellNumber v1 = current - LocalCenter.Rotate(rotation);
                CellNumber v2 = v1 + (TotalSize - 1).Rotate(rotation);
                BottomLeft = new CellNumber(Mathf.Min(v1.row, v2.row), Mathf.Min(v1.column, v2.column));
                TopRight = new CellNumber(Mathf.Max(v1.row, v2.row), Mathf.Max(v1.column, v2.column));
            }
        }
    }
}