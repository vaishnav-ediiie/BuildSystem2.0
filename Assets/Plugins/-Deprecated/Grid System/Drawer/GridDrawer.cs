using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace KSRecs.Deprecated.GridSystemSpace.Drawer
{
    public class GridDrawer : MonoBehaviour
    {
        [SerializeField] private GridSquareVisual squareVisual;
        [SerializeField] private CellNumber startCellNumber = CellNumber.zero;
        [SerializeField] private CellNumber endCellNumber = new CellNumber(50, 50);

        private Dictionary<string, GridSquareVisual> gridVisuals;
        private GridSystem grid;
        private GridSquareVisual prevAt;
        private GridSquareVisual prevAdj;

        private void Start()
        {
            if (GridSystem.Instance == null || !GridSystem.Instance.gameObject.activeInHierarchy)
            {
                Debug.LogWarning("No active grids found. Destroying this drawer");
                Destroy(gameObject);
                return;
            }

            grid = GridSystem.Instance;
            gridVisuals = new Dictionary<string, GridSquareVisual>();
            InitVisuals();
        }

        private void InitVisuals()
        {
            Vector3 cellScale = GridSystem.Instance.CellSize;
            cellScale.z = cellScale.y;
            cellScale.y = 1;

            startCellNumber = GridSystem.ValidateCellNumber(startCellNumber);
            endCellNumber = GridSystem.ValidateCellNumber(endCellNumber);
            StringBuilder poses = new StringBuilder();
            foreach (CellNumber number in CellNumber.LoopOver(startCellNumber, endCellNumber))
            {
                Vector3 position = grid.GetCellPosition(number);
                poses.AppendLine(position.ToString());
                GridSquareVisual vis = Instantiate(squareVisual, position, Quaternion.identity, transform);
                vis.Init(number, cellScale);
                gridVisuals.Add(GetID(number), vis);
            }
            Debug.Log($"Done Visuals: {poses.ToString()}");
        }

        private void DestroyAllVisuals()
        {
            foreach (KeyValuePair<string, GridSquareVisual> visual in gridVisuals)
            {
                Destroy(visual.Value.gameObject);
            }

            gridVisuals.Clear();
        }

        private string GetID(CellNumber number) => $"{number.row}-{number.column}";

        public void SetupCall(GridSquareVisual gridSquareVisual, CellNumber startCell, CellNumber endCell)
        {
            this.squareVisual = gridSquareVisual;
            this.startCellNumber = GridSystem.ValidateCellNumber(startCell);
            this.endCellNumber = GridSystem.ValidateCellNumber(endCell);

            DestroyAllVisuals();
            InitVisuals();
        }
    }
}