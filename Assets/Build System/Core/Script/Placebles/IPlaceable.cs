using System.Collections.Generic;
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
        public abstract int ScriptableID { get; }
        public abstract int FloorNumber { get; }
        public abstract IEnumerable<IMonoPlaceable> Children { get; }
        
    }
}