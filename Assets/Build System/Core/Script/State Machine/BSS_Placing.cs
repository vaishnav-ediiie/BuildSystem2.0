using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public abstract class BSS_Placing : BuiltSystemState
    {
        protected GameObject CurrentSpawned;
        public bool CanPlace { get; protected set; }
        protected abstract PlaceableSOBase PlaceableSoBase { get; }
        private int rotation;

        public int Rotation
        {
            get => rotation;
            set
            {
                Vector3 rot = CurrentSpawned.transform.rotation.eulerAngles;
                CurrentSpawned.transform.rotation = Quaternion.Euler(rot.x, value, rot.z);
                rotation = value;
            }
        }

        protected CellNumber GetReferenceCell(Vector3 mousePos)
        {
            Vector3 playerPos = BuildSystem.player.transform.position;
            Vector3 lookPoint = ScreenToFloorPoint(mousePos);
            CellNumber playerCell = BuildSystem.gridCurrent.CellPositionToNumber(playerPos);
            CellNumber lookAtCell = BuildSystem.gridCurrent.CellPositionToNumber(lookPoint);
            if (playerCell == lookAtCell) return playerCell;
            
            int rowDiff = lookAtCell.row - playerCell.row;
            int colDiff = lookAtCell.column - playerCell.column;
            lookAtCell.column = playerCell.column + Mathf.Clamp(colDiff, -BuildSystem.playerFOV, BuildSystem.playerFOV);
            lookAtCell.row = playerCell.row + Mathf.Clamp(rowDiff, -BuildSystem.playerFOV, BuildSystem.playerFOV);
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
            Vector3 point = BuildSystem.gridCurrent.RayCastToPoint(ray);
            return point;
        }

        protected void ScaleToCellSize(Transform target)
        {
            Vector2 cellSize = BuildSystem.gridCurrent.CellSize;
            target.localScale = new Vector3(cellSize.x, target.localScale.y, cellSize.y);
        }
        
        protected virtual GameObject ReplaceActiveModel(GameObject toPlace, bool nullCurrentSpawned)
        {
            Transform cpTrans = CurrentSpawned.transform;
            GameObject spawned = Object.Instantiate(toPlace, cpTrans.position, cpTrans.rotation, cpTrans.parent);
            spawned.transform.localScale = cpTrans.localScale;
            if (CurrentSpawned) Object.Destroy(CurrentSpawned);
            if (!nullCurrentSpawned) CurrentSpawned = spawned.gameObject;
            if (PlaceableSoBase.scaleToCellSize) ScaleToCellSize(spawned.transform);
            return spawned;
        }

        protected virtual void MarkOkay()
        {
            ReplaceActiveModel(PlaceableSoBase.placingOkay, nullCurrentSpawned: false);
            CanPlace = true;
        }

        protected virtual void MarkError()
        {
            ReplaceActiveModel(PlaceableSoBase.placingError, nullCurrentSpawned: false);
            CanPlace = false;
        }

        public void MoveTo(Vector3 position, IGridNumber gridNumber)
        {
            CurrentSpawned.transform.position = position;
        }

        public void RotateTo(float xAngle, float yAngle, float zAngle)
        {
            rotation = (int)yAngle;
            CurrentSpawned.transform.rotation = Quaternion.Euler(xAngle, yAngle, zAngle);
        }

        public void RotateBy(float xAngle, float yAngle, float zAngle)
        {
            rotation += (int)yAngle;
            if (rotation < 0) rotation = 360 + rotation;
            else if (rotation >= 360) rotation -= 360;
            CurrentSpawned.transform.Rotate(xAngle, yAngle, zAngle);
        }
    }
}