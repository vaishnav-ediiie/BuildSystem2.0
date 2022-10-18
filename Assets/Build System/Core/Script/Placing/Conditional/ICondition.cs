using CustomBuildSystem.Placed;
using CustomGridSystem;

namespace CustomBuildSystem.Placing.Conditional
{
    public abstract class ICondition
    {
        public string name;
        public int floorNumber;
        public bool isFloorRelative;
        public bool invertCondition;
        public bool outputWhenFloorDontExist;

        protected bool GetGrid(BuildSystem buildSystem, out DuoPlaceGrid<CellOccupantMono, EdgeOccupantMono> grid)
        {
            if (isFloorRelative) grid = buildSystem.GetGrid(buildSystem.CurrentFloor + floorNumber);
            else grid = buildSystem.GetGrid(floorNumber);
            return grid == null;
        }
    }
}