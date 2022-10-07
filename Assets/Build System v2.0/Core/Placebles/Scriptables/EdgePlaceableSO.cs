using UnityEngine;

namespace CustomBuildSystem
{
    [CreateAssetMenu]
    public class EdgePlaceableSO : PlaceableSOBase
    {
        public int cellsCount = 1;
        public int centerCellIndex = 0;
    }
}