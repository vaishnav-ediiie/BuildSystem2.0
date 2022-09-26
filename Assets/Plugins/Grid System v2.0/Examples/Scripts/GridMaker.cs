using UnityEngine;


namespace CustomGridSystem.Examples
{
    public class GridMaker : MonoBehaviour
    {
        [SerializeField] private GameObject cellObject;
        [SerializeField] private Vector2 position;
        [SerializeField] private Vector2 cellSize;
        [SerializeField] private float yPosition;
        [SerializeField] private CellNumber lastCellNumber;

        public SimpleGrid SimpleGrid;
        
        void Start()
        {
            SimpleGrid = new SimpleGrid(lastCellNumber, cellSize, position, yPosition);
            
            foreach (CellNumber cellNumber in SimpleGrid.LoopCells(CellNumber.Zero, lastCellNumber))
            {
                Instantiate(cellObject, SimpleGrid.CellNumberToPosition(cellNumber), Quaternion.identity, transform)
                    .name = $"Cell {cellNumber}";
            }

            SimpleGrid.SerializeGrid();
        }

        void Update()
        {

        }
    }
}