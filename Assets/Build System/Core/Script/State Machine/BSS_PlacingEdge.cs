using CustomBuildSystem.Placed;
using CustomBuildSystem.Placing;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public class BSS_PlacingEdge : BSS_Placing<EdgePlaceable>
    {
        public EdgePlaceable Placeable { get; protected set; }
        public EdgeNumber EdgeNumber { get; private set; }

        protected bool needRemark;
        
        internal virtual void Setup(EdgePlaceable placeable)
        {
            this.Placeable = placeable;
            Current = Object.Instantiate(placeable, BuildSystem.probesParent);
            Rotation = 0;

            EdgeNumber = GetReferenceEdge();
            UpdateVisuals(EdgeNumber);
            Mark(BuildSystem.Brain.ValidateEdgePlacement(this), force: true);
        }

        protected virtual void UpdateVisuals(EdgeNumber edgeNumber)
        {
            float rotation = (edgeNumber.edgeType == EdgeType.Vertical) ? -90f : 0f;
            MoveTo(GridCurrent.EdgeNumberToPosition(edgeNumber), edgeNumber);
            RotateTo(0, rotation, 0);
            EdgeNumber = edgeNumber;
            needRemark = true;
        }

        protected virtual void HandleRotation()
        {
            if (BuildSystem.Brain.ShouldRotateEdge(this))
            {
                RotateBy(0f, 180f, 0f);
                needRemark = true;
            }
        }

        public void Mark(bool cp, bool force = false)
        {
            needRemark = false;
            if (cp == CanPlace && force == false) return;

            if (cp)
            {
                MarkOkay();
                BuildEvents.Call_EdgeStateChanged(this, PlacingState.PlacingOkay);
            }
            else
            {
                MarkError();
                BuildEvents.Call_EdgeStateChanged(this, PlacingState.PlacingError);
            }
        }

        internal void ConfirmPlacement()
        {
            if (Current.isDecorator)
            {
                EdgeOccupantMono parent = BuildSystem.gridCurrent.GetEdgeOccupant(EdgeNumber, null);
                if (parent == null) return;
                
                EdgeDecorator placed = Place().AddComponent<EdgeDecorator>();
                placed.Init(Current, parent, Rotation, BuildSystem.ProbsLayer);
                placed.Occupy(BuildSystem);
            }
            else
            {
                EdgeOccupantMono placed = Place().AddComponent<EdgeOccupantMono>();
                placed.Init(Current, EdgeNumber, Rotation, BuildSystem.CurrentFloor, BuildSystem.ProbsLayer);
                placed.Occupy(BuildSystem);
            }

            BuildSystem.SwitchState<BSS_Idle>();
            BuildEvents.Call_EdgeStateChanged(this, PlacingState.Placed);
        }

        internal void CancelPlacement()
        {
            Debug.Log($"Cancel Build: {Current}");
            CanPlace = false;
            Object.Destroy(Current.gameObject);
            BuildSystem.SwitchState<BSS_Idle>();
        }
        
        public override void OnUpdate()
        {
            EdgeNumber edgeNumber = GetReferenceEdge();
            if (edgeNumber != EdgeNumber) UpdateVisuals(edgeNumber);

            HandleRotation();
            if (needRemark) Mark(this.BuildSystem.Brain.ValidateEdgePlacement(this));
            if (BuildSystem.Brain.ShouldPlaceEdge(this)) ConfirmPlacement();
        }
    }
}