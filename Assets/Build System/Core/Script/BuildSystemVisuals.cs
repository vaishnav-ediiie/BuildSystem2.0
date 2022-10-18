using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public class BuildSystemVisuals : MonoBehaviour
    {
        [SerializeField] private CellVisuals cellObject;
        [SerializeField] private EdgeVisuals edgeObject;
        [SerializeField] private bool displayEdges;
        [SerializeField] private bool displayCellNumbers;
        [SerializeField] private bool scaleVisualsByCellSize;
        private BuildSystem buildSystem;
        
        
        

        internal void Setup(CellVisuals cellObject, EdgeVisuals edgeObject, bool displayCellNumbers, bool scaleVisualsByCellSize)
        {
            this.cellObject = cellObject;
            this.edgeObject = edgeObject;
            this.displayEdges = (edgeObject != null);
            this.displayCellNumbers = displayCellNumbers;
            this.scaleVisualsByCellSize = scaleVisualsByCellSize;
        }

        private void OnEnable()
        {
            BuildEvents.OnGridUpdated += UpdateVisuals;
            BuildEvents.OnBuildSystemCreated += AssignBuildSystem;
        }

        private void OnDisable()
        {
            BuildEvents.OnGridUpdated -= UpdateVisuals;
            BuildEvents.OnBuildSystemCreated -= AssignBuildSystem;
        }

        private void AssignBuildSystem(BuildSystem obj)
        {
            buildSystem = obj;
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
                    EdgeNumber edgeHor = new EdgeNumber(cellNumber, EdgeType.Horizontal);
                    Instantiate(edgeObject, simpleGrid.EdgeNumberToPosition(edgeHor), Quaternion.identity, transform).Init(edgeHor, false, displayCellNumbers);
                    
                    EdgeNumber edgeVer = new EdgeNumber(cellNumber, EdgeType.Vertical);
                    Instantiate(edgeObject, simpleGrid.EdgeNumberToPosition(edgeVer), Quaternion.identity, transform).Init(edgeVer, true, displayCellNumbers);
                }
            }

        }

       
    }
}