using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public class BuildSystemVisuals : MonoBehaviour
    {
        [SerializeField] private BuildSystem buildSystem;
        [SerializeField] private CellVisuals cellObject;
        [SerializeField] private EdgeVisuals edgeObject;
        [SerializeField] private bool displayEdges;
        [SerializeField] private bool displayCellNumbers;
        [SerializeField] private bool scaleVisualsByCellSize;


        internal void Setup(BuildSystem buildSystem, CellVisuals cellObject, EdgeVisuals edgeObject, bool displayCellNumbers, bool scaleVisualsByCellSize)
        {
            this.buildSystem = buildSystem;
            this.cellObject = cellObject;
            this.edgeObject = edgeObject;
            this.displayEdges = (edgeObject != null);
            this.displayCellNumbers = displayCellNumbers;
            this.scaleVisualsByCellSize = scaleVisualsByCellSize;
            buildSystem.Brain.OnGridUpdated += UpdateVisuals;
        }

        private void OnEnable()
        {
            if (buildSystem) buildSystem.Brain.OnGridUpdated += UpdateVisuals;
        }

        private void OnDisable()
        {
            if (buildSystem) buildSystem.Brain.OnGridUpdated -= UpdateVisuals;
        }

        internal void UpdateVisuals()
        {
            SimpleGrid simpleGrid = buildSystem.gridCurrent;

            foreach (CellNumber cellNumber in CellNumber.LoopCells(CellNumber.Zero, simpleGrid.LastCellNumber))
            {
                CellVisuals cellVisuals = Instantiate(cellObject, simpleGrid.CellNumberToPosition(cellNumber), Quaternion.identity, transform);
                if (scaleVisualsByCellSize) cellVisuals.Init(cellNumber, displayCellNumbers, simpleGrid.CellSize);
                else cellVisuals.Init(cellNumber, displayCellNumbers);
                if (displayEdges)
                {
                    Instantiate(edgeObject, simpleGrid.EdgeNumberToPosition(new EdgeNumber(cellNumber, EdgeType.Horizontal)), Quaternion.identity, transform).Init($"{cellNumber} Horizontal", true, displayCellNumbers);
                    Instantiate(edgeObject, simpleGrid.EdgeNumberToPosition(new EdgeNumber(cellNumber, EdgeType.Vertical)), Quaternion.identity, transform).Init($"{cellNumber} Vertical", false, displayCellNumbers);
                }
            }

        }

       
    }
}