using System;
using CustomBuildSystem;
using CustomGridSystem;
using UnityEngine;


public class EdgeDecorator : IMonoPlaceable
{
    [NonSerialized] public EdgePlaceableSO Scriptable;
    [NonSerialized] public EdgePlaceable Parent;
    [NonSerialized] public EdgeNumber Number;
    [NonSerialized] public int Rotation;

    public void Init(EdgePlaceableSO scriptable, EdgePlaceable parent, EdgeNumber number, int rotation, LayerMask layer)
    {
        this.Scriptable = scriptable;
        this.Parent = parent;
        this.Number = number;
        this.Rotation = rotation;
        this.gameObject.SetLayerRecursive(layer.GetLayer());
    }
    
    public override GameObject GetDeletePrefab() => Scriptable.placingError;
    public override void UnOccupy(BuildSystem buildSystem) => Parent.RemoveDecorator(this);
    public override void Occupy(BuildSystem buildSystem) => Parent.AddDecorator(this);
    public override bool HasDecorator(PlaceableSOBase scriptable) => false;
}
