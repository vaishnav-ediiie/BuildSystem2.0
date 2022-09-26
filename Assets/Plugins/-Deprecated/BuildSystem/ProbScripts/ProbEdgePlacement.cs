using UnityEngine;
using KSRecs.Deprecated.GridSystemSpace;


namespace KSRecs.Deprecated.BuildSystemSpace
{
    public class ProbEdgePlacement : ProbState
    {
        private Vector3 placementScale;

        internal override void Init(BuildProbSO parent)
        {
            base.Init(parent);
            Vector2 cellSize = GridSystem.Instance.CellSize;
            placementScale = new Vector3(cellSize.x, 1, cellSize.y);
        }

        internal override void MovePreviewTo(CellNumber curCellNum, CellNumber lookAtCellNum, PlayerCompForBuildSystem player)
        {
            lookAtCellNum.column = curCellNum.column + Mathf.Clamp(lookAtCellNum.column - curCellNum.column, -1, 1);
            lookAtCellNum.row = curCellNum.row + Mathf.Clamp(lookAtCellNum.row - curCellNum.row, -1, 1);

            if (curCellNum.column != lookAtCellNum.column && curCellNum.row != lookAtCellNum.row) return;
            if (!GridSystem.IsCellNumberValid(curCellNum) && !GridSystem.IsCellNumberValid(lookAtCellNum)) return;

            Vector3 ccp = GridSystem.Instance.GetCellPosition(curCellNum);
            Vector3 lcp = GridSystem.Instance.GetCellPosition(lookAtCellNum);
            Vector3 position = (ccp + lcp) / 2f;

            GridCell.Place ep = GridCell.GetEdgePlace(position, ccp);
            int fn = player.GetFloorNumber();
            if (GridSystem.IsPlaceOccupied(curCellNum, fn, ep)) return;

            position.y = player.GetFloorHeight();
            Vector3 diff = ccp - lcp;
            float angle = Mathf.Atan2(diff.z, diff.x) * Mathf.Rad2Deg;
            parent.CurrentInstance.transform.rotation = Quaternion.Euler(0, angle, 0);
            parent.CurrentInstance.transform.position = position;
            if (parent.scaleWithCellSize) parent.CurrentInstance.transform.localScale = placementScale;
        }

        internal override void ConfirmBuild(int floorNumber)
        {
            Vector3 instancePos = parent.CurrentInstance.transform.position;
            Vector3 cellPos = GridSystem.SnapToClosestCellCenter(instancePos);

            CellNumber cellNumber = GridSystem.Instance.GetCellNumber(instancePos);
            GridCell.Place edge = GridCell.GetEdgePlace(instancePos, cellPos);
            GridSystem.AddOccupant(cellNumber, floorNumber, edge, parent.CurrentInstance);
        }
    }
}