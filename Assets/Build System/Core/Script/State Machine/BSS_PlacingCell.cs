using CustomBuildSystem.Placed;
using CustomBuildSystem.Placing;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public class BSS_PlacingCell : BSS_Placing<CellPlaceable>
    {
        public CellPlaceable Placeable { get; protected set; }
        public CellNumber CellNumber { get; private set; }

        protected bool needRemark;

        internal virtual void Setup(CellPlaceable placeable)
        {
            this.Placeable = placeable;
            Debug.Log($"placeable: {placeable}     BuildSystem: {BuildSystem}");
            Current = Object.Instantiate(placeable, BuildSystem.probesParent);
            if (placeable.scaleToCellSize)
            {
                Transform plaTran = placeable.transform;
                Vector2 cellSize = BuildSystem.gridCurrent.CellSize;
                plaTran.localScale = new Vector3(cellSize.x, plaTran.localScale.y, cellSize.y);
            }
            
            Rotation = 0;
            CellNumber = GetReferenceCell();
            UpdateVisuals(CellNumber);
            Mark(BuildSystem.Brain.ValidateCellPlacement(this), force: true);
        }
        
        public override void OnUpdate()
        {
            CellNumber cellNumber = GetReferenceCell();
            if (cellNumber != CellNumber) UpdateVisuals(cellNumber);
            
            HandleRotation();
            if (needRemark) Mark(BuildSystem.Brain.ValidateCellPlacement(this));
            if (BuildSystem.Brain.ShouldPlaceCell(this)) ConfirmPlacement();
        }

        public void Mark(bool cp, bool force = false)
        {
            needRemark = false;
            if (cp == CanPlace && force == false) return;

            if (cp)
            {
                MarkOkay();
                BuildEvents.Call_CellStateChanged(this, PlacingState.PlacingOkay);
            }
            else
            {
                MarkError();
                BuildEvents.Call_CellStateChanged(this, PlacingState.PlacingError);
            }
        }

        internal void ConfirmPlacement()
        {
            if (Current.isDecorator)
            {
                CellOccupantMono parent = BuildSystem.gridCurrent.GetCellOccupant(CellNumber, null);
                if (parent == null) return;
                
                CellDecorator placed = Place().AddComponent<CellDecorator>();
                placed.Init(Current, parent, Rotation, BuildSystem.ProbsLayer);
                placed.Occupy(BuildSystem);
                
            }
            else
            {
                CellOccupantMono placed = Place().AddComponent<CellOccupantMono>();
                placed.Init(Current, CellNumber, Rotation, BuildSystem.CurrentFloor, BuildSystem.ProbsLayer);
                placed.Occupy(BuildSystem);
            }
            BuildSystem.SwitchState<BSS_Idle>();
            BuildEvents.Call_CellStateChanged(this, PlacingState.Placed);
        }

        internal void CancelPlacement()
        {
            CanPlace = false;
            Object.Destroy(Current.gameObject);
            BuildSystem.SwitchState<BSS_Idle>();
        }

        protected virtual void HandleRotation()
        {
            int inp = BuildSystem.Brain.ShouldRotateCell(this);
            if (inp == 0) return;

            float rotateBy = -90f;
            if (inp > 0) rotateBy = 90f;

            RotateBy(0f, rotateBy, 0f);
            needRemark = true;
        }

        protected virtual void UpdateVisuals(CellNumber cellNumber)
        {
            CellNumber = cellNumber;
            MoveTo(GridCurrent.CellNumberToPosition(cellNumber), cellNumber);
            needRemark = true;
        }
    }
}