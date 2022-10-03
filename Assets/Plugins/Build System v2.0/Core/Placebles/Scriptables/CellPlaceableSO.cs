using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    [CreateAssetMenu]
    public class CellPlaceableSO : PlaceableSOBase
    {
        [SerializeField] CellPlaceable placed;
        [SerializeField] private CellNumber cellNeeded;
        [SerializeField] private CellNumber centerCell;

        public CellLayoutInfo LayoutInfo => new CellLayoutInfo(this.centerCell, cellNeeded);
        public bool IsSingleCelled => this.cellNeeded.row <= 1 && this.cellNeeded.column <= 0;
        
        public override IPlaceable Place()
        {
            canPlace = false;
            return ReplaceActiveMovel(placed, nullCurrentSpawned: true);
        }

    }
}