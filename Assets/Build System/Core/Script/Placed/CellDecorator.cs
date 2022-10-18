using System;
using System.Collections.Generic;
using CustomBuildSystem;
using CustomBuildSystem.Placing;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem.Placed
{
    public class CellDecorator : OccupantBaseMono
    {
        public CellPlaceable Placeable { get; private set; }
        public CellOccupantMono Parent { get; private set; }
        public int Rotation { get; private set; }

        public CellNumber Number => Parent.Number;

        public void Init(CellPlaceable placeable, CellOccupantMono parent, int rotation, LayerMask layer)
        {
            this.Placeable = placeable;
            this.Parent = parent;
            this.Rotation = rotation;
            this.gameObject.SetLayerRecursive(layer.GetLayer());
        }

        public override GameObject GetDeletePrefab() => Placeable.placingError;
        public override void UnOccupy(BuildSystem buildSystem) => Parent.RemoveDecorator(this);
        public override void Occupy(BuildSystem buildSystem) => Parent.AddDecorator(this);
        public override bool HasDecorator(PlaceableMonoBase placeable) => false;
        public override int PlaceableID => Placeable.ID;
        public override int FloorNumber => this.Parent.Floor;

        public override IEnumerable<OccupantBaseMono> Children
        {
            get { yield break; }
        }
    }
}