using System;
using UnityEngine;
using KSRecs.Deprecated.BuildSystemSpace;

namespace KSRecs.Deprecated.BuildSystemSpace
{
    public class PlayerCompForBuildSystem : MonoBehaviour
    {
        [SerializeField] private float groundFloorHeight = 0f;
        [SerializeField] private float singleFloorHeight = 2f;

        public int GetFloorNumber() => (int)((transform.position.y - groundFloorHeight) / singleFloorHeight);
        public float GetFloorHeight() => GetFloorNumber() * singleFloorHeight + groundFloorHeight;
        
    }
}