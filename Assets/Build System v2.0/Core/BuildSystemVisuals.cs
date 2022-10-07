using System;
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


        private void OnEnable()
        {
            buildSystem.Brain.OnGridUpdated += UpdateVisuals;
        }

        private void OnDisable()
        {
            buildSystem.Brain.OnGridUpdated -= UpdateVisuals;
        }

        void UpdateVisuals()
        {
            SimpleGrid simpleGrid = buildSystem.gridCurrent;

            foreach (CellNumber cellNumber in CellNumber.LoopCells(CellNumber.Zero, simpleGrid.LastCellNumber))
            {
                Instantiate(cellObject, simpleGrid.CellNumberToPosition(cellNumber), Quaternion.identity, transform).Init(cellNumber, displayCellNumbers);
                if (displayEdges)
                {
                    Instantiate(edgeObject, simpleGrid.EdgeNumberToPosition(new EdgeNumber(cellNumber, EdgeType.Horizontal)), Quaternion.identity, transform).Init($"{cellNumber} Horizontal", true, displayCellNumbers);
                    Instantiate(edgeObject, simpleGrid.EdgeNumberToPosition(new EdgeNumber(cellNumber, EdgeType.Vertical)), Quaternion.identity, transform).Init($"{cellNumber} Vertical", false, displayCellNumbers);
                }
            }

        }

       
    }
}