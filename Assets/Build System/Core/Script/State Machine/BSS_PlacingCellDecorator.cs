using CustomBuildSystem.Placed;
using CustomBuildSystem.Placing;
using CustomGridSystem;

namespace CustomBuildSystem
{
    public class BSS_PlacingCellDecorator : BSS_PlacingCell
    {
        internal override void Setup(CellPlaceable placeable)
        {
            ConditionType current = placeable.placementCriteria.cellCenter.conditionType;
            if (current != ConditionType.OccupiedByAny && current != ConditionType.OccupiedBySpecific) 
                placeable.placementCriteria.cellCenter.conditionType = ConditionType.OccupiedByAny;
            base.Setup(placeable);
        }

        protected override void HandleRotation()
        {
            if (!Placeable.parentRelativeRotation) 
                base.HandleRotation();
        }

        protected override void UpdateVisuals(CellNumber cellNumber)
        {
            base.UpdateVisuals(cellNumber);
            if (Placeable.parentRelativeRotation)
            {
                CellOccupantMono placed = BuildSystem.gridCurrent.GetCellOccupant(cellNumber, null);
                if (placed != null)
                {
                    Rotation = placed.Rotation + Placeable.rotationOffset;
                }
            }

            needRemark = true;
        }
    }
}