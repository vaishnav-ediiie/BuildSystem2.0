using System;
using CustomBuildSystem;
using CustomGridSystem;
using UnityEngine;


public class CellDecorator : IMonoPlaceable
{
    [NonSerialized] public CellPlaceableSO Scriptable;
    [NonSerialized] public CellPlaceable Parent;
    [NonSerialized] public int Rotation;
    
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
    public override int GetScriptableID() => Scriptable.ID;
}
