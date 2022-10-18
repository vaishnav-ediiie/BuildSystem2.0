using CustomBuildSystem.Placed;
using CustomBuildSystem.Placing;
using CustomBuildSystem.Placing.Conditional;
using CustomGridSystem;

namespace CustomBuildSystem
{
    public class BSS_PlacingEdgeDecorator : BSS_PlacingEdge
    {
        internal override void Setup(EdgePlaceable placeable)
        {
            if (   !placeable.HasCondition(ConditionType.OccupiedByAny,      0, relativeFloor: true, invertCondition: false)
                && !placeable.HasCondition(ConditionType.OccupiedBySpecific, 0, relativeFloor: true, invertCondition: false))
            {
                placeable.AddCondition(new EdgeCondition(ConditionType.OccupiedByAny, floorNumber: 0, isFloorRelative: true, invertCondition: false));
            }
            base.Setup(placeable);
        }

        protected override void HandleRotation()
        {
            if (!Current.parentRelativeRotation)
            {
                base.HandleRotation();
            }
        }

        protected override void UpdateVisuals(EdgeNumber edgeNumber)
        {
            base.UpdateVisuals(edgeNumber);
            
            if (Current.parentRelativeRotation)
            {
                EdgeOccupantMono placed = BuildSystem.gridCurrent.GetEdgeOccupant(edgeNumber, null);
                if (placed != null) Rotation = placed.Rotation + Current.rotationOffset;
            }
            needRemark = true;
        }
    }
}