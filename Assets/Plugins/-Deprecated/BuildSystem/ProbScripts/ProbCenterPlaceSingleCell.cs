using UnityEngine;
using KSRecs.Deprecated.GridSystemSpace;

namespace KSRecs.Deprecated.BuildSystemSpace
{
    public class ProbCenterPlaceSingleCell: ProbState
    {
        private float currentRotation;
        private Vector3 placementScale;

        internal override void Init(BuildProbSO parent)
        {
            base.Init(parent);
            currentRotation = parent.CurrentInstance.transform.rotation.eulerAngles.y;
            Vector2 cellSize = GridSystem.Instance.CellSize;
            placementScale = new Vector3(cellSize.x, 1, cellSize.y);
        }

        internal override void MovePreviewTo(CellNumber curCellNum, CellNumber lookAtCellNum, PlayerCompForBuildSystem player)
        {
            lookAtCellNum = GridSystem.ValidateCellNumberForCenterPlacement(lookAtCellNum);
            if (Input.GetKeyDown(KeyCode.R)) currentRotation = (currentRotation + 90) % 360;
            if (GridSystem.IsPlaceOccupied(lookAtCellNum, player.GetFloorNumber(), GridCell.Place.Center)) return;
        
            Vector3 pos = GridSystem.Instance.GetCellPosition(lookAtCellNum);
            pos.y = player.GetFloorHeight();
            parent.CurrentInstance.transform.position = pos;
            parent.CurrentInstance.transform.rotation = Quaternion.Euler(0f, currentRotation, 0f);
            
            if (parent.scaleWithCellSize) parent.CurrentInstance.transform.localScale = placementScale;
        }

        internal override void ConfirmBuild(int floorNumber)
        {
            CellNumber cellNumber = GridSystem.Instance.GetCellNumber(parent.CurrentInstance.transform.position); 
            GridSystem.AddOccupant(cellNumber, floorNumber, GridCell.Place.Center, parent.CurrentInstance);
            Debug.Log($"Confirm Build: {cellNumber}");
        }
    }
}