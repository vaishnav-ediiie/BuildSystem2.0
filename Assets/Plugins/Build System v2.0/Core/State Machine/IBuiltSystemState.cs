using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public abstract class BuiltSystemState
    {
        protected BuildSystem buildSystem;
        public DuoPlaceGrid<CellPlaceable, EdgePlaceable> ActiveGrid => buildSystem.activeGrid;
        public Transform player => buildSystem.player;
        public Camera playerCamera => buildSystem.playerCamera;

        internal void Init(BuildSystem buildSystem)
        {
            Debug.Log("Init(BuildSystem buildSystem)");
            this.buildSystem = buildSystem;
        }

        protected CellNumber GetReferenceCell(Vector3 mousePos)
        {
            Vector3 playerPos = buildSystem.player.transform.position;
            Vector3 directionVector = ScreenToFloorPoint(mousePos) - playerPos;
            CellNumber playerCell = buildSystem.activeGrid.CellPositionToNumber(playerPos);
            return buildSystem.activeGrid.AdjacentCellTo(playerCell, directionVector.GetEdgeDirection());
        }
        
        protected EdgeNumber GetReferenceEdge(Vector3 mousePos)
        {
            Vector3 playerPos = buildSystem.player.transform.position;
            Direction direction = (ScreenToFloorPoint(mousePos) - playerPos).GetEdgeDirection();
            return buildSystem.activeGrid.EdgePositionToNumber(playerPos, direction);
        }
        
        private Vector3 ScreenToFloorPoint(Vector3 mousePos)
        {
            Ray ray = buildSystem.playerCamera.ScreenPointToRay(mousePos);
            float h = buildSystem.activeGrid.GridYPos;
            Vector3 orig = ray.origin;
            Vector3 dire = orig + ray.direction;
            float fact = (h - dire.y) / (dire.y - orig.y);
            float x = fact * (dire.x - orig.x);
            float z = fact * (dire.z - orig.z);
            return new Vector3(x + dire.x, h, z + dire.z);
        }
        
        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
    }
}