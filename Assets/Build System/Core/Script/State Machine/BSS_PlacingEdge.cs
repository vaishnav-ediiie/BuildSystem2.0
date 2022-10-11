using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public class BSS_PlacingEdge : BSS_Placing
    {
        public EdgePlaceableSO PlaceableSo { get; private set; }
        private Vector2 screenCenter;
        public EdgeNumber EdgeNumber { get; private set; }
        protected override PlaceableSOBase PlaceableSoBase => PlaceableSo;

        protected bool needRemark;

        private void Mark(bool cp, bool force = false)
        {
            Debug.Log($"Remarking to: {cp}, force: {force}");
            needRemark = false;
            if (cp == CanPlace && force == false) return;

            if (cp)
            {
                MarkOkay();
                BuildSystem.Brain.Call_EdgeStateChanged(this, PlacingState.PlacingOkay);
            }
            else
            {
                MarkError();
                BuildSystem.Brain.Call_EdgeStateChanged(this, PlacingState.PlacingError);
            }
        }

        internal void ConfirmPlacement()
        {
            if (PlaceableSo.isDecorator)
            {
                EdgePlaceable parent = BuildSystem.gridCurrent.GetEdgeOccupant(EdgeNumber, null);
                if (parent == null) return;

                EdgeDecorator placed = ReplaceActiveModel(PlaceableSo.placed, nullCurrentSpawned: true).AddComponent<EdgeDecorator>();
                placed.Init(PlaceableSo, parent, Rotation, BuildSystem.ProbsLayer);
                placed.Occupy(BuildSystem);
            }
            else
            {
                EdgePlaceable placed = ReplaceActiveModel(PlaceableSo.placed, nullCurrentSpawned: true).AddComponent<EdgePlaceable>();
                placed.Init(PlaceableSo, EdgeNumber, Rotation, BuildSystem.CurrentFloor, BuildSystem.ProbsLayer);
                placed.Occupy(BuildSystem);
            }

            BuildSystem.SwitchState<BSS_Idle>();
            BuildSystem.Brain.Call_EdgeStateChanged(this, PlacingState.Placed);
        }

        internal void CancelPlacement()
        {
            CanPlace = false;
            Object.Destroy(CurrentSpawned);
            BuildSystem.SwitchState<BSS_Idle>();
        }

        internal virtual void Setup(EdgePlaceableSO placeable)
        {
            screenCenter = new Vector2(Screen.width, Screen.height) / 2f;
            this.PlaceableSo = placeable;
            CurrentSpawned = Object.Instantiate(placeable.placingOkay);
            Rotation = 0;

            EdgeNumber = GetReferenceEdge(screenCenter);
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

        public override void OnUpdate()
        {
            EdgeNumber edgeNumber = GetReferenceEdge(screenCenter);
            if (edgeNumber != EdgeNumber) UpdateVisuals(edgeNumber);

            HandleRotation();
            if (needRemark) Mark(this.BuildSystem.Brain.ValidateEdgePlacement(this));
            if (BuildSystem.Brain.ShouldPlaceEdge(this)) ConfirmPlacement();
        }
    }
}