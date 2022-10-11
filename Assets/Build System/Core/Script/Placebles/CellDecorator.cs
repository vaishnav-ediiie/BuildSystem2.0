using System;
using System.Collections.Generic;
using CustomBuildSystem;
using CustomGridSystem;
using UnityEngine;


public class CellDecorator : IMonoPlaceable
{
    public CellPlaceableSO Scriptable { get; private set; }
    public CellPlaceable Parent { get; private set; }
    public int Rotation { get; private set; }

    public CellNumber Number => Parent.Number;

    public void Init(CellPlaceableSO scriptable, CellPlaceable parent, int rotation, LayerMask layer)
    {
        this.Scriptable = scriptable;
        this.Parent = parent;
        this.Rotation = rotation;
        this.gameObject.SetLayerRecursive(layer.GetLayer());
    }

    public override GameObject GetDeletePrefab() => Scriptable.placingError;
    public override void UnOccupy(BuildSystem buildSystem) => Parent.RemoveDecorator(this);
    public override void Occupy(BuildSystem buildSystem) => Parent.AddDecorator(this);
    public override bool HasDecorator(PlaceableSOBase scriptable) => false;
    public override int ScriptableID => Scriptable.ID;
    public override int FloorNumber => this.Parent.Floor;

    public override IEnumerable<IMonoPlaceable> Children
    {
        get { yield break; }
    }
}