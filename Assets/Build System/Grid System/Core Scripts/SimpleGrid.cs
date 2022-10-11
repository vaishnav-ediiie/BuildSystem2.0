using System;
using Newtonsoft.Json;
using UnityEngine;


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

        public CellNumber CellPositionToNumberRaw(Vector3 position)
        {
            float col = (position.x - AnchorPosition.x) / CellSize.x;
            float row = (position.z - AnchorPosition.y) / CellSize.y;
            return new CellNumber(Mathf.RoundToInt(row), Mathf.RoundToInt(col));
        }

        public CellNumber AdjacentCellToRaw(CellNumber referenceCell, Direction direction)
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

        public CellNumber AdjacentCellTo(CellNumber referenceCell, Direction direction)
        {
            switch (direction)
            {
                case Direction.Down: return gridType.ValidateCellNumber(new CellNumber(referenceCell.row - 1, referenceCell.column));
                case Direction.Up: return gridType.ValidateCellNumber(new CellNumber(referenceCell.row + 1, referenceCell.column));
                case Direction.Left: return gridType.ValidateCellNumber(new CellNumber(referenceCell.row, referenceCell.column - 1));
                case Direction.Right: return gridType.ValidateCellNumber(new CellNumber(referenceCell.row, referenceCell.column + 1));
                default: return referenceCell;
            }
        }

        public EdgeNumber EdgePositionToNumber(Vector3 position, EdgeType edgeType)
        {
            CellNumber rawNumber = CellPositionToNumberRaw(position);
            CellNumber lastCell = gridType.LastCellNumber;
            int row, col;
            switch (edgeType)
            {
                case EdgeType.Vertical:
                    row = Mathf.Clamp(rawNumber.row, 0, lastCell.row);
                    col = Mathf.Clamp(rawNumber.column, 0, lastCell.column + 1);
                    break;
                case EdgeType.Horizontal:
                    row = Mathf.Clamp(rawNumber.row, 0, lastCell.row + 1);
                    col = Mathf.Clamp(rawNumber.column, 0, lastCell.column);
                    break;
                default:
                    throw new NotImplementedException($"Cannot understand direction: {edgeType}");
            }

            return new EdgeNumber(row, col, edgeType);
        }

        public EdgeNumber EdgePositionToNumber(Vector3 position, Direction direction)
        {
            CellNumber rawNumber = CellPositionToNumberRaw(position);
            CellNumber lastCell = gridType.LastCellNumber - new CellNumber(1, 1);
            int row, col;
            switch (direction)
            {
                case Direction.Up:
                    row = Mathf.Clamp(rawNumber.row, 0, lastCell.row) + 1;
                    col = Mathf.Clamp(rawNumber.column, 0, lastCell.column);
                    return new EdgeNumber(row, col, EdgeType.Vertical);
                case Direction.Down:
                    row = Mathf.Clamp(rawNumber.row, 0, lastCell.row);
                    col = Mathf.Clamp(rawNumber.column, 0, lastCell.column);
                    return new EdgeNumber(row, col, EdgeType.Vertical);
                case Direction.Right:
                    row = Mathf.Clamp(rawNumber.row, 0, lastCell.row);
                    col = Mathf.Clamp(rawNumber.column, 0, lastCell.column) + 1;
                    return new EdgeNumber(row, col, EdgeType.Horizontal);
                case Direction.Left:
                    row = Mathf.Clamp(rawNumber.row, 0, lastCell.row);
                    col = Mathf.Clamp(rawNumber.column, 0, lastCell.column);
                    return new EdgeNumber(row, col, EdgeType.Horizontal);
                default:
                    throw new NotImplementedException($"Cannot understand direction: {direction}");
            }
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

        public bool IsCellNumberValid(CellNumber cellNumber) => gridType.IsCellNumberValid(cellNumber);
        public bool IsEdgeNumberValid(EdgeNumber edgeNumber) => gridType.IsEdgeNumberValid(edgeNumber);
        public CellNumber ValidateCellNumber(CellNumber cellNumber) => gridType.ValidateCellNumber(cellNumber);
        public CellNumber LastCellNumber => gridType.LastCellNumber;

        // @formatter:off
        public CellNumber CellPositionToNumber(Vector3 position) => gridType.ValidateCellNumber(CellPositionToNumberRaw(position));
        public Vector3 CellNumberToPosition(CellNumber cellNumber) => new Vector3(cellNumber.column * CellSize.x + AnchorPosition.x, GridYPos, cellNumber.row * CellSize.y + AnchorPosition.y);
        public Vector3 EdgeNumberToPosition(EdgeNumber edgeNumber) => (CellNumberToPosition(edgeNumber.CellBefore) + CellNumberToPosition(edgeNumber.CellAfter)) / 2f;
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
            return JsonConvert.SerializeObject(
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
            SimpleGridData gridData = JsonConvert.DeserializeObject<SimpleGridData>(data);
            Vector2 oldCellSize = this.CellSize;
            Vector2 oldPosition = this.AnchorPosition;
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
    }
}