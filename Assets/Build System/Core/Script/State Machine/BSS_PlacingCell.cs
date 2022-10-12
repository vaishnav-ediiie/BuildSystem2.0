using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public class BSS_PlacingCell : BSS_Placing
    {
        public CellPlaceableSO Scriptable { get; private set; }
        public CellNumber CellNumber { get; private set; }
        protected Vector2 screenCenter;
        
        protected override PlaceableSOBase PlaceableSoBase => Scriptable;
        protected bool needRemark;

        internal virtual void Setup(CellPlaceableSO placeable)
        {
            screenCenter = new Vector2(Screen.width, Screen.height) / 2f;
            this.Scriptable = placeable;
            CurrentSpawned = Object.Instantiate(placeable.placingOkay, BuildSystem.transform); 
            Rotation = 0;
            CellNumber = GetReferenceCell(screenCenter);
            UpdateVisuals(CellNumber);
            Mark(BuildSystem.Brain.ValidateCellPlacement(this), force: true);
        }
        
        public override void OnUpdate()
        {
            CellNumber cellNumber = GetReferenceCell(screenCenter);
            if (cellNumber != CellNumber) UpdateVisuals(cellNumber);
            
            HandleRotation();
            if (needRemark) Mark(BuildSystem.Brain.ValidateCellPlacement(this));
            if (BuildSystem.Brain.ShouldPlaceCell(this)) ConfirmPlacement();
        }

        private void Mark(bool cp, bool force = false)
        {
            needRemark = false;
            if (cp == CanPlace && force == false) return;

            if (cp)
            {
                MarkOkay();
                BuildSystem.Brain.Call_CellStateChanged(this, PlacingState.PlacingOkay);
            }
            else
            {
                MarkError();
                BuildSystem.Brain.Call_CellStateChanged(this, PlacingState.PlacingError);
            }
        }

        internal void ConfirmPlacement()
        {
            if (Scriptable.isDecorator)
            {
                CellPlaceable parent = BuildSystem.gridCurrent.GetCellOccupant(CellNumber, null);
                if (parent == null) return;
                
                CellDecorator placed = ReplaceActiveModel(Scriptable.placed, nullCurrentSpawned: true).AddComponent<CellDecorator>();
                placed.Init(Scriptable, parent, Rotation, BuildSystem.ProbsLayer);
                placed.Occupy(BuildSystem);
            }
            else
            {
                CellPlaceable placed = ReplaceActiveModel(Scriptable.placed, nullCurrentSpawned: true).AddComponent<CellPlaceable>();
                placed.Init(Scriptable, CellNumber, Rotation, BuildSystem.CurrentFloor, BuildSystem.ProbsLayer);
                placed.Occupy(BuildSystem);
            }
            BuildSystem.SwitchState<BSS_Idle>();
            BuildSystem.Brain.Call_CellStateChanged(this, PlacingState.Placed);
        }

        internal void CancelPlacement()
        {
            CanPlace = false;
            Object.Destroy(CurrentSpawned);
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