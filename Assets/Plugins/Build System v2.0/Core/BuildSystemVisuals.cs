using System;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public class BuildSystemVisuals : MonoBehaviour
    {
        [SerializeField] private BuildSystem buildSystem;
        [SerializeField] private CellVisuals cellObject;
        // [SerializeField] private Transform DownParent;
        // [SerializeField] private Transform UpParent;
        // [SerializeField] private Transform LeftParent;
        // [SerializeField] private Transform RightParent;
        [SerializeField] private EdgeVisuals edgeObject;


        private void OnEnable()
        {
            buildSystem.OnGridUpdated += UpdateVisuals;
        }

        private void OnDisable()
        {
            buildSystem.OnGridUpdated -= UpdateVisuals;
        }

        void UpdateVisuals()
        {
            SimpleGrid simpleGrid = buildSystem.activeGrid;

            foreach (CellNumber cellNumber in CellNumber.LoopCells(CellNumber.Zero, simpleGrid.LastCellNumber))
            {
                Instantiate(cellObject, simpleGrid.CellNumberToPosition(cellNumber), Quaternion.identity, transform).Init(cellNumber);
                // Vector3 pos = simpleGrid.CellNumberToPosition(cellNumber);

                // EdgeNumber enDown   = simpleGrid.EdgePositionToNumber(pos, Direction.Down);
                // EdgeNumber enUp     = simpleGrid.EdgePositionToNumber(pos, Direction.Up);
                // EdgeNumber enLeft   = simpleGrid.EdgePositionToNumber(pos, Direction.Left);
                // EdgeNumber enRight  = simpleGrid.EdgePositionToNumber(pos, Direction.Right);
                
                // Instantiate(edgeObject, simpleGrid.EdgeNumberToPosition(enDown), Quaternion.identity, DownParent).Init($"{cellNumber} Down", false);
                // Instantiate(edgeObject, simpleGrid.EdgeNumberToPosition(enUp), Quaternion.identity, UpParent).Init($"{cellNumber} Up", false);
                // Instantiate(edgeObject, simpleGrid.EdgeNumberToPosition(enLeft), Quaternion.identity, LeftParent).Init($"{cellNumber} Left", true);
                // Instantiate(edgeObject, simpleGrid.EdgeNumberToPosition(enRight), Quaternion.identity, RightParent).Init($"{cellNumber} Right", true);
            }

            // int lastRow = simpleGrid.LastCellNumber.row;
            // MakeDownEdges(lastRow, -2, simpleGrid);
            // MakeDownEdges(lastRow, -1, simpleGrid);
            // MakeDownEdges(lastRow, simpleGrid.LastCellNumber.row+1, simpleGrid);
            // MakeDownEdges(lastRow, simpleGrid.LastCellNumber.row+2, simpleGrid);
        }

        /*void MakeDownEdges(int lastRow, int currentRow, SimpleGrid simpleGrid)
        {
            for (int col = -2; col < lastRow; col++)
            {
                CellNumber cellNumber = new CellNumber(currentRow, col);
                Debug.Log($"Extra: {cellNumber}");
                Vector3 pos = simpleGrid.CellNumberToPosition(cellNumber);
                EdgeNumber enUp     = simpleGrid.EdgePositionToNumber(pos, Direction.Down);
                Instantiate(edgeObject, simpleGrid.EdgeNumberToPosition(enUp), Quaternion.identity, DownParent).Init($"{cellNumber} Down", false);
            }
        }*/
    }
}