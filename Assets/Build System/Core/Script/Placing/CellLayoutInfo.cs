using System.Collections.Generic;
using CustomGridSystem;
using DebugToScreen;
using UnityEngine;

namespace CustomBuildSystem.Placing
{
    public class CellLayoutInfo
    {
        public CellNumber LocalCenter { get; }
        public CellNumber GlobalCenter { get; private set; }
        public CellNumber TotalSize { get; }
        public CellNumber BottomLeft { get; private set; }
        public CellNumber TopRight { get; private set; }
        public int Rotation { get; private set; }
        public bool IsSingleCelled => this.TotalSize.row <= 1 && this.TotalSize.column <= 0;

        public CellNumber TopLeft => new CellNumber(TopRight.row, BottomLeft.column);
        public CellNumber BottomRight => new CellNumber(BottomLeft.row, TopRight.column);

        public CellNumber TotalSizeRotated
        {
            get
            {
                if (Rotation % 180 == 0) return new CellNumber(TotalSize.row, TotalSize.column);
                return new CellNumber(TotalSize.column, TotalSize.row);
            }
        }

        public CellNumber Left => CellNumber.Left.Rotate(Rotation);
        public CellNumber Front => CellNumber.Front.Rotate(Rotation);

        public CellLayoutInfo(CellNumber localCenter, CellNumber totalSize)
        {
            LocalCenter = localCenter;
            TotalSize = totalSize;
        }

        public CellLayoutInfo Refresh(CellNumber current, int rotation)
        {
            GlobalCenter = current;

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

            return this;
        }


        #region Loops
        public IEnumerable<CellNumber> LoopCenterCells()
        {
            if (IsSingleCelled)
            {
                yield return GlobalCenter;
                yield break;
            }

            foreach (CellNumber cellNumber in CellNumber.LoopCells(BottomLeft, TopRight + 1))
            {
                yield return cellNumber;
            }
        }

        public IEnumerable<CellNumber> LoopOuterSideCell()
        {
            if (IsSingleCelled)
            {
                CellNumber current = new CellNumber(GlobalCenter.row, GlobalCenter.column);
                current.row++;
                yield return current;
                current.row -= 2;
                yield return current;
                current.row++;
                current.column++;
                yield return current;
                current.column -= 2;
                yield return current;
                yield break;
            }

            CellNumber currentOne = new CellNumber(0, BottomLeft.column - 1);
            CellNumber currentTwo = new CellNumber(0, TopRight.column + 1);
            for (int i = BottomLeft.row; i <= TopRight.row; i++)
            {
                currentOne.row = i;
                currentTwo.row = i;
                yield return currentOne;
                yield return currentTwo;
            }

            currentOne = new CellNumber(BottomLeft.row - 1, 0);
            currentTwo = new CellNumber(TopRight.row + 1, 0);
            for (int i = BottomLeft.column; i <= TopRight.column; i++)
            {
                currentOne.column = i;
                currentTwo.column = i;
                yield return currentOne;
                yield return currentTwo;
            }
        }

        public IEnumerable<CellNumber> LoopOuterCornerCells()
        {
            yield return this.BottomLeft - 1;
            yield return new CellNumber(BottomLeft.row - 1, TopRight.column + 1);
            yield return this.TopRight + 1;
            yield return new CellNumber(TopRight.row + 1, BottomLeft.column - 1);
        }

        public IEnumerable<CellNumber> LoopInnerSideCell()
        {
            CellNumber tsr = TotalSizeRotated;

            if (tsr.row <= 1 && tsr.column <= 1) // 1x1 block, not used IsSingleCelled for simplicity
            {
                yield return GlobalCenter;
                yield break;
            }

            CellNumber currentOne = BottomLeft;
            if (tsr.row <= 1) // 1xm block
            {
                for (int c = BottomLeft.column; c <= TopRight.column; c++)
                {
                    currentOne.column = c;
                    yield return currentOne;
                }

                yield break;
            }

            if (tsr.column <= 1) // nx1 block
            {
                for (int r = BottomLeft.row; r <= TopRight.row; r++)
                {
                    currentOne.row = r;
                    yield return currentOne;
                }

                yield break;
            }

            // Note that in any possible scenario, only one of above 3 conditions can be true at once
            // nxm block    where n & m are at-least 2
            currentOne = new CellNumber(BottomLeft.row, BottomLeft.column);
            CellNumber currentTwo = new CellNumber(BottomLeft.row, TopRight.column);
            for (int r = BottomLeft.row; r < TopRight.row; r++)
            {
                currentOne.row = r;
                currentTwo.row = r;
                yield return currentOne;
                yield return currentTwo;
            }

            if (tsr.column <= 2) yield break;

            // nxm block where n is at-least 2 and m is at-least 3
            currentOne = new CellNumber(BottomLeft.row, BottomLeft.column + 1);
            currentTwo = new CellNumber(TopRight.row, BottomLeft.column + 1);
            for (int c = BottomLeft.column; c < TopRight.column; c++)
            {
                currentOne.column = c;
                currentTwo.column = c;
                yield return currentOne;
                yield return currentTwo;
            }
        }

        public IEnumerable<CellNumber> LoopInnerCornerCells()
        {
            yield return this.BottomLeft;
            CellNumber tsr = TotalSizeRotated;
            if (tsr.column > 1) yield return this.BottomRight;
            if (tsr.row > 1) yield return this.TopRight;
            if (tsr.column > 1) yield return this.TopLeft;
        }

        public IEnumerable<EdgeNumber> LoopInnerEdges()
        {
            bool horizontal = true;
            bool vertical = false;
            for (int row = this.BottomLeft.row; row <= this.TopRight.row; row++)
            {
                horizontal = false;
                for (int col = this.BottomLeft.column; col <= this.TopRight.column; col++)
                {
                    if (vertical) yield return new EdgeNumber(row, col, EdgeType.Horizontal);
                    if (horizontal) yield return new EdgeNumber(row, col, EdgeType.Vertical);
                    horizontal = true;
                }

                vertical = true;
            }
        }

        public IEnumerable<EdgeNumber> LoopBorderEdges()
        {
            int r1 = BottomLeft.row;
            int r2 = TopRight.row + 1;
            for (int c = BottomLeft.column; c <= TopRight.column; c++)
            {
                yield return new EdgeNumber(r1, c, EdgeType.Horizontal);
                yield return new EdgeNumber(r2, c, EdgeType.Horizontal);
            }
            
            int c1 = BottomLeft.column;
            int c2 = TopRight.column + 1;
            for (int r = BottomLeft.row; r <= TopRight.row; r++)
            {
                yield return new EdgeNumber(r, c1, EdgeType.Vertical);
                yield return new EdgeNumber(r, c2, EdgeType.Vertical);
            }
            
        }
        #endregion
    }
}