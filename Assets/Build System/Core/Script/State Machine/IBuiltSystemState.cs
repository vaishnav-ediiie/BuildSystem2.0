using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public abstract class BuiltSystemState
    {
        internal BuildSystem BuildSystem;
        public DuoPlaceGrid<CellPlaceable, EdgePlaceable> GridCurrent => BuildSystem.gridCurrent;
        public Transform player => BuildSystem.player;
        public Camera playerCamera => BuildSystem.playerCamera;

        internal void Init(BuildSystem buildSystem)
        {
            this.BuildSystem = buildSystem;
        }

        protected CellNumber GetReferenceCell(Vector3 mousePos)
        {
            Vector3 playerPos = BuildSystem.player.transform.position;
            Vector3 lookPoint = ScreenToFloorPoint(mousePos);
            CellNumber playerCell = BuildSystem.gridCurrent.CellPositionToNumber(playerPos);
            CellNumber lookAtCell = BuildSystem.gridCurrent.CellPositionToNumber(lookPoint);
            if (playerCell == lookAtCell) return playerCell;
            lookAtCell.column = playerCell.column + Mathf.Clamp(lookAtCell.column - playerCell.column, -1, 1);
            lookAtCell.row = playerCell.row + Mathf.Clamp(lookAtCell.row - playerCell.row, -1, 1);
            return lookAtCell;
        }

        protected EdgeNumber GetReferenceEdge(Vector3 mousePos)
        {
            Vector3 playerPos = BuildSystem.player.transform.position;
            Direction direction = (ScreenToFloorPoint(mousePos) - playerPos).GetDirection();
            return BuildSystem.gridCurrent.EdgePositionToNumber(playerPos, direction);
        }

        private Vector3 ScreenToFloorPoint(Vector3 mousePos)
        {
            Ray ray = BuildSystem.playerCamera.ScreenPointToRay(mousePos);
            float h = BuildSystem.gridCurrent.GridYPos;
            Vector3 orig = ray.origin;
            Vector3 dire = orig + ray.direction;
            float fact = (h - dire.y) / (dire.y - orig.y);
            float x = fact * (dire.x - orig.x);
            float z = fact * (dire.z - orig.z);
            return new Vector3(x + dire.x, h, z + dire.z);
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnUpdate()
        {
        }
        
        public virtual void OnExit()
        {
        }


        
    }
}