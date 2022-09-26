using UnityEngine;

namespace CustomBuildSystem
{
    [CreateAssetMenu]
    public class EdgePlacebleSO : PlacebleSOBase
    {
        [SerializeField] EdgePlaceble placed;

        public override IPlaceble Place()
        {
            canPlace = false;
            return ReplaceActiveMovel(placed, nullCurrentSpawned: true);
        }
    }
}