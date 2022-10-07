using UnityEngine;

namespace CustomBuildSystem
{
    public abstract class PlaceableSOBase : ScriptableObject
    {
        [SerializeField] internal int placeableID;
        [SerializeField] internal GameObject placingOkay;
        [SerializeField] internal GameObject placingError;
        [SerializeField] internal GameObject placed;
        [SerializeField] internal Sprite Icon;
    }
}