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
        [SerializeField] internal bool isDecorator = false;
        [SerializeField, DrawIfBool("isDecorator", true)] internal bool copyParentRotation = false;
    }
}


#if UNITY_EDITOR

#endif