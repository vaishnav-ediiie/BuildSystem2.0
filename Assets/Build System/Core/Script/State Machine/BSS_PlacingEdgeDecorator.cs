using CustomGridSystem;

namespace CustomBuildSystem
{
    public class BSS_PlacingEdgeDecorator : BSS_PlacingEdge
    {
        internal override void Setup(EdgePlaceableSO placeable)
        {
            ConditionType current = placeable.placementCriteria.edgeCenter.conditionType;
            if (current != ConditionType.OccupiedByAny && current != ConditionType.OccupiedBySpecific)
                placeable.placementCriteria.edgeCenter.conditionType = ConditionType.OccupiedByAny;
            base.Setup(placeable);
        }

        protected override void HandleRotation()
        {
            if (!Scriptable.parentRelativeRotation)
            {
                base.HandleRotation();
            }
        }

        protected override void UpdateVisuals(EdgeNumber edgeNumber)
        {
            base.UpdateVisuals(edgeNumber);
            
            if (Scriptable.parentRelativeRotation)
            {
                EdgePlaceable placed = BuildSystem.gridCurrent.GetEdgeOccupant(edgeNumber, null);
                if (placed != null) Rotation = placed.Rotation + Scriptable.rotationOffset;
            }
            needRemark = true;
        }
    }
}