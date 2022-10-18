using CustomBuildSystem.Placed;
using CustomBuildSystem.Placing;
using CustomBuildSystem.Placing.Conditional;
using CustomGridSystem;

namespace CustomBuildSystem
{
    public class BSS_PlacingCellDecorator : BSS_PlacingCell
    {
        internal override void Setup(CellPlaceable placeable)
        {
            if (   !placeable.HasCondition(ConditionType.OccupiedByAny, CellConditionPlace.Centers,         0, relativeFloor: true, invertCondition: false)
                   && !placeable.HasCondition(ConditionType.OccupiedBySpecific, CellConditionPlace.Centers, 0, relativeFloor: true, invertCondition: false))
            {
                placeable.AddCondition(new CellCondition(ConditionType.OccupiedByAny, CellConditionPlace.Centers, floorNumber: 0, isFloorRelative: true, invertCondition: false));
            }
            base.Setup(placeable);
        }

        protected override void HandleRotation()
        {
            if (!Current.parentRelativeRotation) 
                base.HandleRotation();
        }

        protected override void UpdateVisuals(CellNumber cellNumber)
        {
            base.UpdateVisuals(cellNumber);
            if (Current.parentRelativeRotation)
            {
                CellOccupantMono placed = BuildSystem.gridCurrent.GetCellOccupant(cellNumber, null);
                if (placed != null)
                {
                    Rotation = placed.Rotation + Current.rotationOffset;
                }
            }

            needRemark = true;
        }
    }
}