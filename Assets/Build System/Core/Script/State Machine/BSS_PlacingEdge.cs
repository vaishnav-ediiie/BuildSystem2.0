using CustomBuildSystem.Placed;
using CustomBuildSystem.Placing;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public class BSS_PlacingEdge : BSS_Placing
    {
        public EdgePlaceable Placeable { get; private set; }
        public EdgeNumber EdgeNumber { get; private set; }
        protected override PlaceableMonoBase PlaceableSoBase => Placeable;

        protected bool needRemark;
        
        internal virtual void Setup(EdgePlaceable placeable)
        {
            this.Placeable = placeable;
            CurrentSpawned = Object.Instantiate(placeable, BuildSystem.probesParent);
            Rotation = 0;

            EdgeNumber = GetReferenceEdge();
            UpdateVisuals(EdgeNumber);
            Mark(BuildSystem.Brain.ValidateEdgePlacement(this), force: true);
        }

        protected virtual void UpdateVisuals(EdgeNumber edgeNumber)
        {
            float rotation = (edgeNumber.edgeType == EdgeType.Horizontal) ? -90f : 0f;
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
            if (Placeable.isDecorator)
            {
                EdgeOccupantMono parent = BuildSystem.gridCurrent.GetEdgeOccupant(EdgeNumber, null);
                if (parent == null) return;
                
                EdgeDecorator placed = Place().AddComponent<EdgeDecorator>();
                placed.Init(Placeable, parent, Rotation, BuildSystem.ProbsLayer);
                placed.Occupy(BuildSystem);
            }
            else
            {
                EdgeOccupantMono placed = Place().AddComponent<EdgeOccupantMono>();
                placed.Init(Placeable, EdgeNumber, Rotation, BuildSystem.CurrentFloor, BuildSystem.ProbsLayer);
                placed.Occupy(BuildSystem);
            }

            BuildSystem.SwitchState<BSS_Idle>();
            BuildEvents.Call_EdgeStateChanged(this, PlacingState.Placed);
        }

        internal void CancelPlacement()
        {
            Debug.Log($"Cancel Build: {CurrentSpawned}");
            CanPlace = false;
            Object.Destroy(CurrentSpawned.gameObject);
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