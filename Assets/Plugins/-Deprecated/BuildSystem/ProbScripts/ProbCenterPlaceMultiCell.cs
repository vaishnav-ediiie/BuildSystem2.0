using System;
using UnityEngine;
using KSRecs.Deprecated.GridSystemSpace;

namespace KSRecs.Deprecated.BuildSystemSpace
{
    public class ProbCenterPlaceMultiCell : ProbState
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
            if (Input.GetKeyDown(KeyCode.R) && !IncrementRotation(lookAtCellNum, player.GetFloorNumber()))
                return;

            if (!IsPlacementValid(lookAtCellNum, (int)currentRotation, player.GetFloorNumber()))
                return;

            Vector3 pos = GridSystem.Instance.GetCellPosition(lookAtCellNum);
            pos.y = player.GetFloorHeight();
            parent.CurrentInstance.transform.position = pos;
            parent.CurrentInstance.transform.rotation = Quaternion.Euler(0f, currentRotation, 0f);
            if (parent.scaleWithCellSize) parent.CurrentInstance.transform.localScale = placementScale;
                
        }

        internal override void ConfirmBuild(int floorNumber)
        {
            if (parent.CurrentInstance == null) return;
            CellNumber centerCellNumber = GridSystem.Instance.GetCellNumber(parent.CurrentInstance.transform.position);

            GetBoundsForLoop(centerCellNumber, (int)currentRotation, out CellNumber b1, out CellNumber b2);
            foreach (CellNumber number in CellNumber.LoopOverComplete(b1, b2))
            {
                GridSystem.AddOccupant(number, floorNumber, GridCell.Place.Center, parent.CurrentInstance);
            }
        }

        private bool IsPlacementValid(CellNumber pos, int angle, int floorNumber)
        {
            GetAllBonds(pos, (int)angle, out CellNumber b1, out CellNumber b2, out CellNumber b3, out CellNumber b4);


            if (!GridSystem.AreAllCellsValid(b1, b2, b3, b4))
                return false;


            GetBoundsForLoop(pos, (int)angle, out b1, out b2);
            foreach (CellNumber number in CellNumber.LoopOverComplete(b1, b2))
            {
                if (!GridSystem.IsCompleteEmpty(number, floorNumber))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Rotates currentInstance on Y-axis to next valid angle (angle for which object leaves plot boundaries is invalid)
        /// </summary>
        private bool IncrementRotation(CellNumber pos, int floorNumber)
        {
            int cr = (int)currentRotation;

            if (IsPlacementValid(pos, cr + 90, floorNumber))
            {
                currentRotation = (currentRotation + 90) % 360f;
                return true;
            }

            if (IsPlacementValid(pos, cr + 180, floorNumber))
            {
                currentRotation = (currentRotation + 180) % 360f;
                return true;
            }

            if (IsPlacementValid(pos, cr + 270, floorNumber))
            {
                currentRotation = (currentRotation + 270) % 360f;
                return true;
            }

            return IsPlacementValid(pos, cr, floorNumber);
        }

        private static CellNumber Flip(CellNumber vec) => new CellNumber(vec.row, vec.column);


        private void GetBounds(CellNumber cgp, int angle, out CellNumber bond1, out CellNumber bond2)
        {
            bond1 = cgp - RotatePoint(parent.centerCell, angle);
            bond2 = bond1 + RotatePoint(parent.cellsNeeded, angle) - RotatePoint(CellNumber.one, angle);
        }

        private void GetBoundsForLoop(CellNumber cgp, int angle, out CellNumber bond1, out CellNumber bond2)
        {
            CellNumber v1 = cgp - RotatePoint(parent.centerCell, angle);
            CellNumber v2 = v1 + RotatePoint(parent.cellsNeeded, angle) - RotatePoint(CellNumber.one, angle);

            bond1 = new CellNumber();
            bond2 = new CellNumber();

            bond1.column = Mathf.Min(v1.column, v2.column);
            bond1.row = Mathf.Min(v1.row, v2.row);

            bond2.column = Mathf.Max(v1.column, v2.column);
            bond2.row = Mathf.Max(v1.row, v2.row);
        }

        private void GetAllBonds(CellNumber cgp, int angle, out CellNumber bond1, out CellNumber bond2, out CellNumber bond3, out CellNumber bond4)
        {
            GetBounds(cgp, angle, out bond1, out bond2);
            bond3 = new CellNumber(bond1.row, bond2.column);
            bond4 = new CellNumber(bond2.row, bond1.column);
        }

        private CellNumber RotatePoint(CellNumber point, int angle)
        {
            if (angle == 0) return new CellNumber(+point.row, +point.column);
            if (angle == 90) return new CellNumber(+point.column, -point.row);
            if (angle == 180) return new CellNumber(-point.row, -point.column);
            if (angle == 270) return new CellNumber(-point.column, +point.row);
            return point;
        }
    }
}