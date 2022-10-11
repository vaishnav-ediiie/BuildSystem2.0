using UnityEngine;

namespace CustomBuildSystem.Example
{
    public class ExampleBrain : BuildSystemBrain
    {
        public override int ShouldRotateCell(BSS_PlacingCell placeableSo) => (int)Input.GetAxis("Mouse ScrollWheel");
        public override bool ShouldRotateEdge(BSS_PlacingEdge placeableSo) => Input.GetAxis("Mouse ScrollWheel") != 0;
        public override bool ShouldPlaceCell(BSS_PlacingCell placeableSo) => placeableSo.CanPlace && Input.GetMouseButtonDown(0);
        public override bool ShouldPlaceEdge(BSS_PlacingEdge placeableSo) => placeableSo.CanPlace && Input.GetMouseButtonDown(0);
    }
}