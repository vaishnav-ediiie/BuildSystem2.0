using System;
using System.Collections.Generic;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    [Serializable]
    public class CellPlaceable : IMonoPlaceable
    {
        [HideInInspector] public CellPlaceableSO Scriptable;
        [HideInInspector] public CellNumber Number;
        [HideInInspector] public int Rotation;
        [HideInInspector] public List<IPlaceable> Decorators;

        public void Init(CellPlaceableSO scriptable, CellNumber number, int rotation, LayerMask layer)
        {
            this.Scriptable = scriptable;
            this.Number = number;
            this.Rotation = rotation;
            this.Decorators = new List<IPlaceable>();
            this.gameObject.SetLayerRecursive(layer.GetLayer());
        }

        public override GameObject GetDeletePrefab() => Scriptable.placingError;
        
        public override void Occupy(BuildSystem buildSystem)
        {
            CellLayoutInfo layoutInfo = Scriptable.LayoutInfo(Number, Rotation);
            
            if (layoutInfo.IsSingleCelled)
            {
                buildSystem.gridCurrent.OccupyCell(Number, this);
                return;
            }

            foreach (CellNumber number in CellNumber.LoopCells(layoutInfo.BottomLeft, layoutInfo.TopRight+1))
            {
                buildSystem.gridCurrent.OccupyCell(number, this);
            }
        }
        
        public override void UnOccupy(BuildSystem buildSystem)
        {
            CellLayoutInfo layoutInfo = Scriptable.LayoutInfo(Number, Rotation);
            if (layoutInfo.IsSingleCelled)
            {
                buildSystem.gridCurrent.EmptyCell(Number);
                return;
            }
            
            foreach (CellNumber cellNumber in CellNumber.LoopCells(layoutInfo.BottomLeft, layoutInfo.TopRight+1))
            {
                buildSystem.gridCurrent.EmptyCell(cellNumber);
            }
        }

    }
}