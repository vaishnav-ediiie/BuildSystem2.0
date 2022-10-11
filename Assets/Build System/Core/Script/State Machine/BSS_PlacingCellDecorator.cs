using CustomGridSystem;

namespace CustomBuildSystem
{
    public class BSS_PlacingCellDecorator : BSS_PlacingCell
    {
        internal override void Setup(CellPlaceableSO placeable)
        {
            ConditionType current = placeable.placementCriteria.cellCenter.conditionType;
            if (current != ConditionType.OccupiedByAny && current != ConditionType.OccupiedBySpecific) 
                placeable.placementCriteria.cellCenter.conditionType = ConditionType.OccupiedByAny;
            base.Setup(placeable);
        }

        protected override void HandleRotation()
        {
            if (!PlaceableSo.parentRelativeRotation) 
                base.HandleRotation();
        }

        protected override void UpdateVisuals(CellNumber cellNumber)
        {
            base.UpdateVisuals(cellNumber);
            if (PlaceableSo.parentRelativeRotation)
            {
                CellPlaceable placed = BuildSystem.gridCurrent.GetCellOccupant(cellNumber, null);
                if (placed != null)
                {
                    Rotation = placed.Rotation + PlaceableSo.rotationOffset;
                }
            }

            needRemark = true;
        }
    }
}