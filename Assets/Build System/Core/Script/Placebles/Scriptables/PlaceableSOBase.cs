using UnityEngine;


namespace CustomBuildSystem
{
    public abstract class PlaceableSOBase : ScriptableObject
    {
        [SerializeField] internal int ID;
        [SerializeField] internal GameObject placingOkay;
        [SerializeField] internal GameObject placingError;
        [SerializeField] internal GameObject placed;
        [SerializeField] internal Sprite Icon;
        [SerializeField] internal bool scaleToCellSize;
        [SerializeField] internal bool isDecorator = false;
        [SerializeField, DrawIfBool("isDecorator", true)] internal bool parentRelativeRotation = false;
        [SerializeField, DrawIfBool("isDecorator", true)] internal int rotationOffset = 0;
    }
}
