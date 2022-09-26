namespace CustomBuildSystem
{
    public class BSS_PlacingEdge: BuiltSystemState
    {
        private EdgePlacebleSO placebleSO;

        public BSS_PlacingEdge(EdgePlacebleSO placebleSO)
        {
            this.placebleSO = placebleSO;
        }

        public override void OnUpdate()
        {
            
        }
    }
}