using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    [CreateAssetMenu]
    public class EdgePlaceableSO : PlaceableSOBase
    {
        [SerializeField] EdgePlaceable placed;
        public int cellsCount = 1;
        public int centerCellIndex = 0;

        public override IPlaceable Place()
        {
            canPlace = false;
            return ReplaceActiveMovel(placed, nullCurrentSpawned: true);
        }
    }
}