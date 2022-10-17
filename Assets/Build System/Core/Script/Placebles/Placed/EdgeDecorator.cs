using System;
using System.Collections.Generic;
using CustomBuildSystem;
using CustomBuildSystem.Placing;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem.Placed
{
    public class EdgeDecorator : OccupantBaseMono
    {
        public EdgePlaceable Scriptable { get; private set; }
        public EdgeOccupantMono Parent { get; private set; }
        public int Rotation { get; private set; }
        public EdgeNumber Number => Parent.Number;

        public void Init(EdgePlaceable scriptable, EdgeOccupantMono parent, int rotation, LayerMask layer)
        {
            this.Scriptable = scriptable;
            this.Parent = parent;
            this.Rotation = rotation;
            this.gameObject.SetLayerRecursive(layer.GetLayer());
        }

        public override GameObject GetDeletePrefab() => Scriptable.placingError;
        public override void UnOccupy(BuildSystem buildSystem) => Parent.RemoveDecorator(this);
        public override void Occupy(BuildSystem buildSystem) => Parent.AddDecorator(this);
        public override bool HasDecorator(PlaceableMonoBase scriptable) => false;
        public override int ScriptableID => Scriptable.ID;
        public override int FloorNumber => this.Parent.Floor;


        public override IEnumerable<OccupantBaseMono> Children
        {
            get { yield break; }
        }
    }
}