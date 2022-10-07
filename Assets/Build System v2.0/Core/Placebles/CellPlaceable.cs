using System;
using System.Collections.Generic;
using System.Data;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public class CellPlaceable : IMonoPlaceable
    {
        [NonSerialized] public CellPlaceableSO Scriptable;
        [NonSerialized] public CellNumber Number;
        [NonSerialized] public int Rotation;
        [NonSerialized] public List<CellDecorator> Decorators;

        public void Init(CellPlaceableSO scriptable, CellNumber number, int rotation, LayerMask layer)
        {
            this.Scriptable = scriptable;
            this.Number = number;
            this.Rotation = rotation;
            this.Decorators = new List<CellDecorator>();
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

        public override bool HasDecorator(PlaceableSOBase scriptable)
        {
            foreach (CellDecorator decorator in Decorators)
            {
                if (decorator.Scriptable == scriptable) return true;
            }
            return false;
        }
        
        public void RemoveDecorator(CellDecorator deco)
        {
            if (Decorators.Contains(deco))
                this.Decorators.Remove(deco);
        }

        public void AddDecorator(CellDecorator deco)
        {
            if (!Decorators.Contains(deco))
                this.Decorators.Add(deco);
        }
    }
}