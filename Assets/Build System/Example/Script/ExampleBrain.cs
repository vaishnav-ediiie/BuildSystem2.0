using UnityEngine;

namespace CustomBuildSystem.Example
{
    public class ExampleBrain : BuildBrainBase
    {
        public override int ShouldRotateCell(BSS_PlacingCell placeableSo) => (int)(10f*Input.GetAxis("Mouse ScrollWheel"));
        public override bool ShouldRotateEdge(BSS_PlacingEdge placeableSo) => Input.GetAxis("Mouse ScrollWheel") != 0;
        public override bool ShouldPlaceCell(BSS_PlacingCell placeableSo) => placeableSo.CanPlace && Input.GetMouseButton(0);
        public override bool ShouldPlaceEdge(BSS_PlacingEdge placeableSo) => placeableSo.CanPlace && Input.GetMouseButton(0);
        public override Vector3 GetMousePosition => Input.mousePosition;
    }
}