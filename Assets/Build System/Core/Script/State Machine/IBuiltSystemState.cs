using CustomBuildSystem.Placed;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public abstract class BuiltSystemState
    {
        internal BuildSystem BuildSystem;
        public DuoPlaceGrid<CellOccupantMono, EdgeOccupantMono> GridCurrent => BuildSystem.gridCurrent;
        public Transform player => BuildSystem.player;
        public Camera playerCamera => BuildSystem.playerCamera;

        internal void Init(BuildSystem buildSystem)
        {
            this.BuildSystem = buildSystem;
        }

        public virtual void OnEnter()
        {
        }

        public virtual void OnUpdate()
        {
        }
        
        public virtual void OnExit()
        {
        }


        
    }
}