using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public class BSS_PlacingCell : BSS_Placing
    {
        public CellPlaceableSO PlaceableSo { get; private set; }
        public CellNumber CellNumber { get; private set; }
        private Vector2 screenCenter;
        
        protected override PlaceableSOBase PlaceableSoBase => PlaceableSo;

        internal void Setup(CellPlaceableSO placeable)
        {
            screenCenter = new Vector2(Screen.width, Screen.height) / 2f;
            this.PlaceableSo = placeable;
            CurrentSpawned = Object.Instantiate(placeable.placingOkay); 
            Rotation = 0;

            if (placeable.isDecorator)
            {
                ConditionType current = placeable.placementCriteria.cellCenter.conditionType;
                if (current != ConditionType.OccupiedByAny && current != ConditionType.OccupiedBySpecific) 
                    placeable.placementCriteria.cellCenter.conditionType = ConditionType.OccupiedByAny;
            }
            
            CellNumber = GetReferenceCell(screenCenter);
            UpdateVisuals(CellNumber, forceMark: true);
        }
        
        public override void OnUpdate()
        {
            CellNumber cellNumber = GetReferenceCell(screenCenter);
            if (cellNumber != CellNumber) UpdateVisuals(cellNumber);
            HandleRotate();
            if (BuildSystem.Brain.ShouldPlaceCell(this)) ConfirmPlacement();
        }

        private void Mark(bool cp, bool force = false)
        {
            if (cp == CanPlace && force == false)
            {
                return;
            }

            if (cp)
            {
                MarkOkay();
                BuildSystem.Brain.Call_CellStateChanged(this, PlacingStage.PlacingOkay);
            }
            else
            {
                MarkError();
                BuildSystem.Brain.Call_CellStateChanged(this, PlacingStage.PlacingError);
            }
        }

        internal void ConfirmPlacement()
        {
            if (PlaceableSo.isDecorator)
            {
                CellPlaceable parent = BuildSystem.gridCurrent.GetCellOccupant(CellNumber, null);
                if (parent == null) return;
                
                CellDecorator placed = ReplaceActiveModel(PlaceableSo.placed, nullCurrentSpawned: true).AddComponent<CellDecorator>();
                placed.Init(PlaceableSo, parent, CellNumber, Rotation, BuildSystem.ProbsLayer);
                placed.Occupy(BuildSystem);
            }
            else
            {
                CellPlaceable placed = ReplaceActiveModel(PlaceableSo.placed, nullCurrentSpawned: true).AddComponent<CellPlaceable>();
                placed.Init(PlaceableSo, CellNumber, Rotation, BuildSystem.ProbsLayer);
                placed.Occupy(BuildSystem);
            }
            BuildSystem.SwitchState<BSS_Idle>();
            BuildSystem.Brain.Call_CellStateChanged(this, PlacingStage.Placed);
        }

        internal void CancelPlacement()
        {
            CanPlace = false;
            Object.Destroy(CurrentSpawned);
            BuildSystem.SwitchState<BSS_Idle>();
        }

        private void HandleRotate()
        {
            int inp = BuildSystem.Brain.ShouldRotateCell(this);
            if (inp == 0) return;

            float rotateBy = -90f;
            if (inp > 0) rotateBy = 90f;

            RotateBy(0f, rotateBy, 0f);
            Mark(BuildSystem.Brain.ValidateCellPlacement(this));
        }

        private void UpdateVisuals(CellNumber cellNumber, bool forceMark = false)
        {
            CellNumber = cellNumber;
            MoveTo(GridCurrent.CellNumberToPosition(cellNumber), cellNumber);
            Mark(BuildSystem.Brain.ValidateCellPlacement(this), forceMark);
        }
    }
}