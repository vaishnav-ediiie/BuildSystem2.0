using KSRecs.Deprecated.GridSystemSpace;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;


namespace CustomGridSystem
{
    public class SimpleGrid
    {
        /// <summary>
        /// Called when the grid is moved. Arg0 is the amount by which cell was moved. For new position, use "AnchorPosition" Property
        /// </summary>
        public event Action<Vector2> OnGridMoved;

        /// <summary>
        /// Called when the cell size is changed. Arg0 is the amount by which cellSize was changed. For new cellSize, use "CellSize" Property
        /// </summary>
        public event Action<Vector2> OnCellSizeChanged;

        /// <summary>
        /// Called when the grid is moved on Y axis. Arg0 is the amount by which grid was moved.
        /// </summary>
        public event Action<float> OnGridYPosChanged;

        public Vector2 CellSize { get; private set; }
        public Vector2 AnchorPosition { get; private set; }
        public float GridYPos { get; private set; }

        private IGridType gridType;

        // @formatter:off
        public SimpleGrid()                                                               : this(Vector2.one, Vector2.zero, 0f){}
        public SimpleGrid(Vector2 cellSize, float gridYPos = 0)                           : this(cellSize, Vector2.zero, gridYPos) {}
        public SimpleGrid(CellNumber lastCellNumber, float gridYPos = 0)                  : this(lastCellNumber, Vector2.one, Vector2.zero, gridYPos){}
        public SimpleGrid(CellNumber lastCellNumber, Vector2 cellSize, float gridYPos = 0): this(lastCellNumber, cellSize, Vector2.zero, gridYPos) {}
        // @formatter:on

        public SimpleGrid(Vector2 cellSize, Vector2 anchorPosition, float gridYPos = 0)
        {
            this.CellSize = cellSize;
            this.AnchorPosition = anchorPosition;
            this.GridYPos = gridYPos;
            this.gridType = new GridTypeInfinite();
        }

        public SimpleGrid(CellNumber lastCellNumber, Vector2 cellSize, Vector2 anchorPosition, float gridYPos = 0)
        {
            this.CellSize = cellSize;
            this.AnchorPosition = anchorPosition;
            this.GridYPos = gridYPos;
            this.gridType = new GridTypeFinite(lastCellNumber);
        }

        private CellNumber CellPositionToNumberRaw(Vector3 position)
        {
            Vector2 wp = new Vector2(position.x, position.z);
            float col = (wp.x - AnchorPosition.x) / CellSize.x;
            float row = (wp.y - AnchorPosition.y) / CellSize.y;
            return new CellNumber(Mathf.RoundToInt(row), Mathf.RoundToInt(col));
        }

        public CellNumber AdjacentCellTo(CellNumber referenceCell, Direction direction)
        {
            switch (direction)
            {
                case Direction.Down: return new CellNumber(referenceCell.row - 1, referenceCell.column);
                case Direction.Up: return new CellNumber(referenceCell.row + 1, referenceCell.column);
                case Direction.Left: return new CellNumber(referenceCell.row, referenceCell.column - 1);
                case Direction.Right: return new CellNumber(referenceCell.row, referenceCell.column + 1);
                default: return referenceCell;
            }
        }

        public EdgeNumber EdgePositionToNumber(Vector3 position, EdgeType edgeType)
        {
            var rawNumber = CellPositionToNumberRaw(position);
            var clampedNumber = gridType.ValidateCellNumber(rawNumber);
            if (edgeType == EdgeType.Horizontal)
            {
                if (clampedNumber.column == rawNumber.column || rawNumber.column < 0) return new EdgeNumber(CellPositionToNumber(position), edgeType);
                return new EdgeNumber(clampedNumber.row, clampedNumber.column + 1, edgeType);
            }

            if (clampedNumber.row == rawNumber.row || rawNumber.row < 0) return new EdgeNumber(CellPositionToNumber(position), edgeType);
            return new EdgeNumber(clampedNumber.row + 1, clampedNumber.column, edgeType);
        }

        public void SetCellSize(Vector2 newSize)
        {
            Vector2 delta = newSize - CellSize;
            this.CellSize = newSize;
            OnCellSizeChanged?.Invoke(delta);
        }

        public void SetLastCellNumber(CellNumber cellNumber)
        {
            if (this.gridType is GridTypeFinite)
            {
                GridTypeFinite finite = (GridTypeFinite)this.gridType;
                finite.lastCellNumber = cellNumber;
            }
            else
            {
                this.gridType = new GridTypeFinite(cellNumber);
            }
        }

        public void SetYPosition(float yPosition)
        {
            float delta = yPosition - this.GridYPos;
            this.GridYPos = yPosition;
            OnGridYPosChanged?.Invoke(delta);
        }

        public void UpdateInfo(CellNumber lastCellNumber, Vector2 newCellSize, Vector2 newAnchor, float newYPosition)
        {
            Vector2 deltaSize = newCellSize - CellSize;
            Vector2 deltaPos = newAnchor - AnchorPosition;
            float deltaY = newYPosition - this.GridYPos;
            

            if (this.gridType is GridTypeFinite)
            {
                GridTypeFinite finite = (GridTypeFinite)this.gridType;
                finite.lastCellNumber = lastCellNumber;
            }
            else
            {
                this.gridType = new GridTypeFinite(lastCellNumber);
            }

            this.CellSize = newCellSize;
            this.GridYPos = newYPosition;
            this.AnchorPosition = newAnchor;

            this.OnGridMoved?.Invoke(deltaPos);
            OnCellSizeChanged?.Invoke(deltaSize);
            OnGridYPosChanged?.Invoke(deltaY);
        }

        public void MoveBy(Vector2 delta)
        {
            this.AnchorPosition += delta;
            this.OnGridMoved?.Invoke(delta);
        }

        public void MoveTo(Vector2 newPosition)
        {
            Vector2 delta = newPosition - AnchorPosition;
            this.AnchorPosition = newPosition;
            this.OnGridMoved?.Invoke(delta);
        }

        public bool IsCellNumberValid(CellNumber cellNumber) => gridType.IsCellNumberValid(cellNumber.row, cellNumber.column);
        public CellNumber ValidateCellNumber(CellNumber cellNumber) => gridType.ValidateCellNumber(cellNumber);
        public CellNumber LastCellNumber => gridType.LastCellNumber;

        // @formatter:off
        private Vector3 CellNumberToPositionRaw(CellNumber cellNumber) => new Vector3(cellNumber.column * CellSize.x + AnchorPosition.x, GridYPos, cellNumber.row * CellSize.y + AnchorPosition.y);
        public CellNumber CellPositionToNumber(Vector3 position) => gridType.ValidateCellNumber(CellPositionToNumberRaw(position));
        public Vector3 CellNumberToPosition(CellNumber cellNumber) => CellNumberToPositionRaw(gridType.ValidateCellNumber(cellNumber));
        public Vector3 EdgeNumberToPosition(EdgeNumber edgeNumber) => (CellNumberToPositionRaw(edgeNumber.CellBefore) + CellNumberToPositionRaw(edgeNumber.CellAfter)) / 2f;
        // @formatter:on

        /// <summary>
        /// Do not use this method if you are using UniPlaceGrid or DuoPlaceGrid. Use SerializeWithOccupants instead.
        /// </summary>
        /// <returns>String data for this instance</returns>
        public string SerializeGrid()
        {
            CellNumber lastCell;
            bool isFinite = gridType is GridTypeFinite;
            if (isFinite) lastCell = gridType.LastCellNumber;
            else lastCell = CellNumber.Zero;
            return JsonUtility.ToJson(
                new SimpleGridData()
                {
                    cellSize = CellSize,
                    anchorPosition = AnchorPosition,
                    gridYPos = GridYPos,
                    isFinite = isFinite,
                    lastCellNumber = lastCell
                });
        }
        
        /// <summary>
        /// Do not use this method if you are using UniPlaceGrid or DuoPlaceGrid. Use DeserializeWithOccupants instead.
        /// </summary>
        /// <param name="data"></param>
        public void DeserializeGrid(string data)
        {
            SimpleGridData gridData = JsonUtility.FromJson<SimpleGridData>(data);
            Vector2 oldCellSize = this.CellSize;
            Vector2 oldPosition= this.AnchorPosition;
            float oldY = this.GridYPos;

            this.CellSize = gridData.cellSize;
            this.AnchorPosition = gridData.anchorPosition;
            this.GridYPos = gridData.gridYPos;
            
            if (gridData.isFinite) this.gridType = new GridTypeFinite(gridData.lastCellNumber);
            else this.gridType = new GridTypeInfinite();

            OnCellSizeChanged?.Invoke(this.CellSize - oldCellSize);
            OnGridMoved?.Invoke(this.AnchorPosition - oldPosition);
            OnGridYPosChanged?.Invoke(this.GridYPos - oldY);
        }
        
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