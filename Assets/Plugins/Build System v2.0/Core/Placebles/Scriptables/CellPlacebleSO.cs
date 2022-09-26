using UnityEngine;

namespace CustomBuildSystem
{
    [CreateAssetMenu]
    public class CellPlacebleSO : PlacebleSOBase
    {
        [SerializeField] CellPlaceble placed;


        public override IPlaceble Place()
        {
            canPlace = false;
            return ReplaceActiveMovel(placed, nullCurrentSpawned: true);
        }
    }
}