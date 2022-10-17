using System.Collections.Generic;
using CustomBuildSystem.Placing;
using UnityEngine;

namespace CustomBuildSystem.Placed
{
    public abstract class OccupantBaseMono : MonoBehaviour, IPlaceable
    {
        public abstract GameObject GetDeletePrefab();
        public abstract void UnOccupy(BuildSystem buildSystem);
        public abstract void Occupy(BuildSystem buildSystem);
        public abstract bool HasDecorator(PlaceableMonoBase scriptable);
        public abstract int ScriptableID { get; }
        public abstract int FloorNumber { get; }
        public abstract IEnumerable<OccupantBaseMono> Children { get; }
    }
}