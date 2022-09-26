using System;
using System.Text;
using UnityEngine;

namespace KSRecs.Deprecated.GridSystemSpace
{
    public class GridSystem : MonoBehaviour
    {
        public static GridSystem Instance { get; private set; }
        public static event Action<CellNumber, int> OnCellOccupiedEvent;
        public static event Action<CellNumber, int> OnCellEmptiedEvent;

        [SerializeField] private CellNumber gridSize;
        [SerializeField] private Vector2 cellSize;
        [SerializeField] private Vector2 startPoint;
        [SerializeField] public float GridYPos;
        private GridCell[,] allCells;

        /// <summary>
        /// Returns cell specified by cellNumber, null if the cell number is invalid (Valid cell numbers are 0 <= cellNumber < gridSize)
        /// </summary>
        public GridCell this[CellNumber cellNumber]
        {
            get
            {
                if (IsCellNumberValid(cellNumber))
                    return this.allCells[cellNumber.row, cellNumber.column];
                return null;
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Debug.Log($"Instance not Exist: {this.GetInstanceID()}");
                Instance = this;
                Init();
            }
            else
            {
                Debug.Log($"Instance3 Exist: {this.GetInstanceID()}");
                Destroy(gameObject);
            }
        }

        private void Init()
        {
            allCells = new GridCell[gridSize.row, gridSize.column];
            for (int i = 0; i < gridSize.row; i++)
                for (int j = 0; j < gridSize.column; j++)
                    allCells[i, j] = new GridCell().Init();
            GridCell.SetAngleFunction();
        }

        private void OnDestroy()
        {
            Debug.Log($"Instance Destroying: {this.GetInstanceID()}");
            if (Instance.GetInstanceID() == this.GetInstanceID())
            {
                Instance = null;
            }
        }
        
        private void ResetAllCells()
        {
            for (int i = 0; i < gridSize.row; i++)
                for (int j = 0; j < gridSize.column; j++)
                    allCells[i, j].ClearSelf();
            GridCell.SetAngleFunction();
        }
        
        public static bool AreAllCellsValid(params CellNumber[] numbers)
        {
            foreach (CellNumber cn in numbers)
                if (!IsCellNumberValid(cn))
                    return false;

            return true;
        }

        public static GameObject GetOccupant(CellNumber cellNumber, int floorNumber, GridCell.Place place)
        {
            var cell = GridSystem.Instance[cellNumber];
            if (cell == null) return null;

            GameObject go = cell.GetOccupant(place, floorNumber);
            if (place == GridCell.Place.Center || go != null) return go;

            var cellAdj = GridSystem.GetAdjacentCell(cellNumber, place);
            if (cellAdj == null) return null;
            return cellAdj.GetOccupant(GridCell.OppositeTo(place), floorNumber);
        }

        public static bool IsPlaceOccupied(CellNumber cellNumber, int floorNumber, GridCell.Place place) => GridSystem.GetOccupant(cellNumber, floorNumber, place);

        public static bool IsPlaceEmpty(CellNumber cellNumber, int floorNumber, GridCell.Place place) => !GridSystem.GetOccupant(cellNumber, floorNumber, place);

        public static bool IsCompleteEmpty(CellNumber cellNumber, int floorNumber) =>
            !GridSystem.GetOccupant(cellNumber, floorNumber, GridCell.Place.Left) &&
            !GridSystem.GetOccupant(cellNumber, floorNumber, GridCell.Place.Right) &&
            !GridSystem.GetOccupant(cellNumber, floorNumber, GridCell.Place.Up) &&
            !GridSystem.GetOccupant(cellNumber, floorNumber, GridCell.Place.Down) &&
            !GridSystem.GetOccupant(cellNumber, floorNumber, GridCell.Place.Center);

        public static bool IsCompleteOccupied(CellNumber cellNumber, int floorNumber) =>
            GridSystem.GetOccupant(cellNumber, floorNumber, GridCell.Place.Left) &&
            GridSystem.GetOccupant(cellNumber, floorNumber, GridCell.Place.Right) &&
            GridSystem.GetOccupant(cellNumber, floorNumber, GridCell.Place.Up) &&
            GridSystem.GetOccupant(cellNumber, floorNumber, GridCell.Place.Down) &&
            GridSystem.GetOccupant(cellNumber, floorNumber, GridCell.Place.Center);

        public static void AddOccupant(CellNumber cellNumber, int floorNumber, GridCell.Place place, GameObject occupant)
        {
            var cell = GridSystem.Instance[cellNumber];
            if (cell == null)
            {
                if (place == GridCell.Place.Center) return;
                cell = GetAdjacentCell(cellNumber, place);
                if (cell == null) return;
                place = GridCell.OppositeTo(place);
            }

            cell.AddOccupant(place, floorNumber, occupant);
            OnCellOccupiedEvent?.Invoke(cellNumber, floorNumber);
        }

        public static GameObject RemoveOccupant(CellNumber cellNumber, int floorNumber, GridCell.Place place)
        {
            var cell = GridSystem.Instance[cellNumber];
            if (cell == null) return null;

            GameObject go = cell.RemoveOccupant(place, floorNumber);
            OnCellEmptiedEvent?.Invoke(cellNumber, floorNumber);
            return go;
        }

        public static CellNumber GetAdjacentCellNumber(CellNumber cellNumber, GridCell.Place direction)
        {
            if (direction == GridCell.Place.Down) return new CellNumber(cellNumber.row - 1, cellNumber.column);
            if (direction == GridCell.Place.Up) return new CellNumber(cellNumber.row + 1, cellNumber.column);
            if (direction == GridCell.Place.Left) return new CellNumber(cellNumber.row, cellNumber.column - 1);
            if (direction == GridCell.Place.Right) return new CellNumber(cellNumber.row, cellNumber.column + 1);
            return cellNumber;
        }

        public static GridCell GetAdjacentCell(CellNumber cellNumber, GridCell.Place direction) => Instance[GetAdjacentCellNumber(cellNumber, direction)];

        
        public static CellNumber ValidateCellNumber(CellNumber cellNumber) => new CellNumber(Mathf.Clamp(cellNumber.row, 0, Instance.gridSize.row), Mathf.Clamp(cellNumber.column, 0, Instance.gridSize.column));
        
        internal static CellNumber ValidateCellNumberForCenterPlacement(CellNumber cellNumber) => new CellNumber(Mathf.Clamp(cellNumber.row, 0, Instance.gridSize.row-1), Mathf.Clamp(cellNumber.column, 0, Instance.gridSize.column-1));

        public static bool IsCellNumberValid(CellNumber cellNumber) => (0 <= cellNumber.column && cellNumber.column < Instance.gridSize.column) && (0 <= cellNumber.row && cellNumber.row < Instance.gridSize.row);

        public static Vector3 SnapToClosestCellCenter(Vector3 point) => Instance.GetCellPosition(Instance.GetCellNumber(point));

        public CellNumber GetCellNumber(Vector3 worldPos)
        {
            Vector2 wp = new Vector2(worldPos.x, worldPos.z);
            Vector2 exp = (wp - startPoint) / cellSize;
            return new CellNumber(Mathf.RoundToInt(exp.y), Mathf.RoundToInt(exp.x));
        }

        public Vector3 GetCellPosition(CellNumber cellNumber) => new Vector3(cellNumber.column * cellSize.x + startPoint.x, GridYPos, cellNumber.row * cellSize.y + startPoint.y);

        public Vector2 CellSize => cellSize;
        
        public string GetStats()
        {
            StringBuilder builder = new StringBuilder();
            foreach (CellNumber number in CellNumber.LoopOver(CellNumber.zero, gridSize))
            {
                if (Instance[number].GetStats(out string str)) builder.Append($"Cell {number}:\n{str}\n==============================\n\n");
            }

            return builder.ToString();
        }
        
        public void SetupCall(CellNumber theGridSize, Vector2 theCellSize, Vector2 theStartPoint)
        {
            this.gridSize = theGridSize;
            this.cellSize = theCellSize;
            this.startPoint = theStartPoint;
            GridSystem.Instance = this;
            ResetAllCells();
        }
    }
}