namespace CustomBuildSystem.Placing.Conditional
{
    public enum ConditionType
    {
        /// <summary>Number must be valid & place must be empty</summary>
        MustBeEmpty = 0,

        /// <summary>Number must be valid & place must be occupied</summary>
        OccupiedByAny = 1,

        /// <summary>Number must be valid & place must be occupied by either one of specified items</summary>
        OccupiedBySpecific = 2,
        
        /// <summary>Number must be valid & place must be not occupied by any one of specified items</summary>
        NotOccupiedBySpecific = 3
    }

    public enum CellConditionPlace
    {
        Centers,
        Sides,
        Corners,
        EdgeBoundaries,
        EdgeBetween
    }
}