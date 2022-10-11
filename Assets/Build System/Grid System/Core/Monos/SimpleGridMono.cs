using UnityEngine;

namespace CustomGridSystem.Examples
{
    public class SimpleGridMono : MonoBehaviour
    {
        [SerializeField] private bool isFinite;

        [SerializeField, DrawIfBool("isFinite", true)]
        private CellNumber lastCellNumber;

        [SerializeField] private Vector2 cellSize;

        [SerializeField] private bool showVisuals;

        [SerializeField, DrawIfBool("showVisuals", true)]
        private CellNumber visualsFirstCell;

        [SerializeField, DrawIfBool("showVisuals", true)]
        private CellNumber visualsLastCell;

        [SerializeField, DrawIfBool("showVisuals", true)]
        private CellVisuals cellVisuals;

        private SimpleGrid grid;

        void Awake()
        {
            Vector3 position = transform.position;
            if (isFinite) grid = new SimpleGrid(lastCellNumber, cellSize, new Vector2(position.x, position.z), position.y);
            else grid = new SimpleGrid(cellSize, new Vector2(position.x, position.z), position.y);

            if (showVisuals)
            {
                foreach (CellNumber cellNumber in CellNumber.LoopCells(visualsFirstCell, visualsLastCell))
                {
                    Instantiate(cellVisuals, grid.CellNumberToPosition(cellNumber), Quaternion.identity, transform);
                }
            }
        }

        // @formatter:off
        public CellNumber LastCellNumber => grid.LastCellNumber;
        
        public string SerializeGrid() => grid.SerializeGrid();
        
        public CellNumber CellPositionToNumberRaw(Vector3 position) => grid.CellPositionToNumberRaw(position);
        public void SetCellSize(Vector2 newSize)                    => grid.SetCellSize(newSize);
        public void SetLastCellNumber(CellNumber cellNumber)        => grid.SetLastCellNumber(cellNumber);
        public void SetYPosition(float yPosition)                   => grid.SetYPosition(yPosition);
        public void MoveBy(Vector2 delta)                           => grid.MoveBy(delta);
        public void MoveTo(Vector2 newPosition)                     => grid.MoveTo(newPosition);
        public bool IsCellNumberValid(CellNumber cellNumber)        => grid.IsCellNumberValid(cellNumber);
        public bool IsEdgeNumberValid(EdgeNumber edgeNumber)        => grid.IsEdgeNumberValid(edgeNumber);
        public CellNumber ValidateCellNumber(CellNumber cellNumber) => grid.ValidateCellNumber(cellNumber);
        public CellNumber CellPositionToNumber(Vector3 position)    => grid.CellPositionToNumber(position);
        public Vector3 CellNumberToPosition(CellNumber cellNumber)  => grid.CellNumberToPosition(cellNumber);
        public Vector3 EdgeNumberToPosition(EdgeNumber edgeNumber)  => grid.EdgeNumberToPosition(edgeNumber);
        public void DeserializeGrid(string data)                    => grid.DeserializeGrid(data);

        public CellNumber AdjacentCellToRaw(CellNumber referenceCell, Direction direction) => grid.AdjacentCellToRaw(referenceCell, direction);
        public CellNumber AdjacentCellTo(CellNumber referenceCell, Direction direction)    => grid.AdjacentCellTo(referenceCell, direction);
        public EdgeNumber EdgePositionToNumber(Vector3 position, EdgeType edgeType)        => grid.EdgePositionToNumber(position, edgeType);
        public EdgeNumber EdgePositionToNumber(Vector3 position, Direction direction)      => grid.EdgePositionToNumber(position, direction);
        
        public void UpdateInfo(CellNumber lastCellNumber, Vector2 newCellSize, Vector2 newAnchor, float newYPosition) => grid.UpdateInfo(lastCellNumber, newCellSize, newAnchor, newYPosition);
        // @formatter:on

    }
}