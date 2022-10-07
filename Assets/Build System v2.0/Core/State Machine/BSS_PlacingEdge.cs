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
        
        internal void Setup(EdgePlaceableSO placeable)
        {
            screenCenter = new Vector2(Screen.width, Screen.height) / 2f;
            this.PlaceableSo = placeable;
            CurrentSpawned = Object.Instantiate(placeable.placingOkay); 
            Rotation = 0;
            EdgeNumber = GetReferenceEdge(screenCenter);
            UpdateVisuals(EdgeNumber);
        }
        
        public override void OnUpdate()
        {
            EdgeNumber edgeNumber = GetReferenceEdge(screenCenter);
            if (edgeNumber != EdgeNumber) UpdateVisuals(edgeNumber);
            if (BuildSystem.Brain.ShouldRotateEdge(this)) RotateBy(0f, 180f, 0f);
            if (BuildSystem.Brain.ShouldPlaceEdge(this)) ConfirmPlacement();
        }

        private void Mark(bool cp)
        {
            if (cp == CanPlace) return;

            if (cp)
            {
                MarkOkay();
                BuildSystem.Brain.Call_EdgeStateChanged(this, PlacingStage.PlacingOkay);
            }
            else
            {
                MarkError();
                BuildSystem.Brain.Call_EdgeStateChanged(this, PlacingStage.PlacingError);
            }
        }
        
        internal void ConfirmPlacement()
        {
            EdgePlaceable placed = ReplaceActiveModel(PlaceableSo.placed, nullCurrentSpawned: true).AddComponent<EdgePlaceable>();
            placed.Init(PlaceableSo, EdgeNumber, BuildSystem.ProbsLayer);
            placed.Occupy(BuildSystem);
            BuildSystem.SwitchState<BSS_Idle>();
            BuildSystem.Brain.Call_EdgeStateChanged(this, PlacingStage.Placed);
        }

        internal void CancelPlacement()
        {
            CanPlace = false;
            Object.Destroy(CurrentSpawned);
            BuildSystem.SwitchState<BSS_Idle>();

        }

        private void UpdateVisuals(EdgeNumber edgeNumber)
        {
            float rotation = (edgeNumber.edgeType == EdgeType.Horizontal) ? -90f : 0f;
            MoveTo(GridCurrent.EdgeNumberToPosition(edgeNumber), edgeNumber);
            RotateTo(0, rotation, 0);
            Mark(this.BuildSystem.Brain.ValidateEdgePlacement(this));
            EdgeNumber = edgeNumber;
        }
    }
}