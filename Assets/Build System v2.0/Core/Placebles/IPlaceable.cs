using UnityEngine;

namespace CustomBuildSystem
{
    public interface IPlaceable
    {

    }

    public abstract class IMonoPlaceable : MonoBehaviour, IPlaceable
    {
        public abstract GameObject GetDeletePrefab();
        public abstract void UnOccupy(BuildSystem buildSystem);
        public abstract void Occupy(BuildSystem buildSystem);
        public abstract bool HasDecorator(PlaceableSOBase scriptable);
        public abstract int GetScriptableID();
    }
}