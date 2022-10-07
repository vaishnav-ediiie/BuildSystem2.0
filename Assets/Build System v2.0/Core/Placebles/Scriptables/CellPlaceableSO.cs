using UnityEngine;
using CellNumber = CustomGridSystem.CellNumber;

namespace CustomBuildSystem
{
    [CreateAssetMenu]
    public class CellPlaceableSO : PlaceableSOBase
    {
        [SerializeField] private CellNumber cellNeeded = CellNumber.One;
        [SerializeField] private CellNumber centerCell = CellNumber.Zero;
        [SerializeField] public CellPlacementCriteria placementCriteria;

        public CellLayoutInfo LayoutInfo(CellNumber cell, int rotation) => new CellLayoutInfo(this.centerCell, cellNeeded, cell, rotation);

        public void AddPlacementCriteria()
        {
            
        }
        
        public bool AreBaseConditionsSatisfied(BSS_PlacingCell placingCell) => this.placementCriteria.AreSatisfied(placingCell.BuildSystem, LayoutInfo(placingCell.CellNumber, placingCell.Rotation));
    }
}