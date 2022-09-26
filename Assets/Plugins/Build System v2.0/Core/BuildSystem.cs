using UnityEngine;
using CustomGridSystem;
using System;
using System.Collections.Generic;

namespace CustomBuildSystem
{
    [DefaultExecutionOrder(-123)]
    public class BuildSystem : MonoBehaviour
    {
        private BuiltSystemState activeState;
        private DuoPlaceGrid<CellPlaceble, EdgePlaceble> activeGrid;

        public DuoPlaceGrid<CellPlaceble, EdgePlaceble> ActiveGrid => activeGrid;


        private void Awake()
        {
            activeState = new BSS_Idle();
            activeState.Init(this);
            activeState.OnStart();
        }

        void Update()
        {
            activeState.OnUpdate();
        }

        public void EnterBuildMode(CellNumber lastCellNumber, Vector2 cellSize, Vector3 anchorPosition)
        {
            activeGrid = new DuoPlaceGrid<CellPlaceble, EdgePlaceble>(lastCellNumber, cellSize, new Vector2(anchorPosition.x, anchorPosition.z), anchorPosition.y);
            activeState = new BSS_BuildingIdle();
            activeState.Init(this);
        }

        public void ExitBuildMode(){
            activeState = new BSS_Idle();
            activeState.Init(this);
        }


    }
}
