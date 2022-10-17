using System;
using System.Collections.Generic;
using CustomBuildSystem.Placing;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem.Placed
{
    public class CellOccupantMono : OccupantBaseMono
    {
        public CellPlaceable Scriptable { get; private set; }
        public CellNumber Number { get; private set; }
        public int Rotation { get; private set; }
        public int Floor { get; private set; }
        [NonSerialized] public List<CellDecorator> Decorators;

        public void Init(CellPlaceable scriptable, CellNumber number, int rotation, int floorNumber, LayerMask layer)
        {
            this.Scriptable = scriptable;
            this.Number = number;
            this.Rotation = rotation;
            this.Decorators = new List<CellDecorator>();
            this.Floor = floorNumber;
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

            foreach (CellNumber number in CellNumber.LoopCells(layoutInfo.BottomLeft, layoutInfo.TopRight + 1))
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

            foreach (CellDecorator decorator in Decorators)
            {
                Destroy(decorator.gameObject);
            }

            foreach (CellNumber cellNumber in CellNumber.LoopCells(layoutInfo.BottomLeft, layoutInfo.TopRight + 1))
            {
                buildSystem.gridCurrent.EmptyCell(cellNumber);
            }
        }

        public override bool HasDecorator(PlaceableMonoBase scriptable)
        {
            foreach (CellDecorator decorator in Decorators)
            {
                if (decorator.Placeable == scriptable) return true;
            }

            return false;
        }

        public override int ScriptableID => Scriptable.ID;
        public override int FloorNumber => Floor;

        public void RemoveDecorator(CellDecorator deco)
        {
            if (Decorators.Contains(deco))
            {
                this.Decorators.Remove(deco);
            }
        }

        public void AddDecorator(CellDecorator deco)
        {
            if (!Decorators.Contains(deco))
                this.Decorators.Add(deco);
        }


        public override IEnumerable<OccupantBaseMono> Children
        {
            get
            {
                foreach (CellDecorator decorator in Decorators)
                {
                    yield return decorator;
                }
            }
        }

        [Serializable]
        public class Serializer
        {
            public int scriptableID;
            public int row;
            public int column;
            public int rotation;
            public int floorNumber;
            public DecoSer[] decorators;

            public Serializer()
            {
            }

            public Serializer(CellOccupantMono source)
            {
                this.scriptableID = source.Scriptable.ID;
                this.row = source.Number.row;
                this.column = source.Number.column;
                this.rotation = source.Rotation;
                this.floorNumber = source.Floor;
                decorators = new DecoSer[source.Decorators.Count];
                int i = 0;
                foreach (CellDecorator deco in source.Decorators)
                {
                    decorators[i] = new DecoSer(deco);
                    i++;
                }
            }

            public static CellOccupantMono Deserialize(Serializer serializer, BuildSystem buildSystem)
            {
                CellPlaceable placeable = BuildSystem.AllPlaceableData[serializer.scriptableID] as CellPlaceable;
                if (placeable == null)
                {
                    foreach (KeyValuePair<int, PlaceableMonoBase> placeableSoBase in BuildSystem.AllPlaceableData)
                    {
                        Debug.Log($" We Have: {placeableSoBase.Key} as {placeableSoBase.Value.GetType()}");
                    }

                    Debug.Log($"Not found with id: {serializer.scriptableID} as CellPlaceableSO");
                }

                CellNumber cellNumber = new CellNumber(serializer.row, serializer.column);
                Vector3 position = buildSystem.gridCurrent.CellNumberToPosition(cellNumber);
                Quaternion rotation = Quaternion.Euler(0, serializer.rotation, 0);
                CellOccupantMono parent = Instantiate(placeable.placed, position, rotation).AddComponent<CellOccupantMono>();
                parent.Init(placeable, cellNumber, serializer.rotation, serializer.floorNumber, buildSystem.ProbsLayer);
                parent.Occupy(buildSystem);

                foreach (DecoSer decoSer in serializer.decorators)
                {
                    CellPlaceable deco = BuildSystem.AllPlaceableData[decoSer.scriptableID] as CellPlaceable;
                    Quaternion decoRot = Quaternion.Euler(0, decoSer.rotation, 0);
                    CellDecorator decoPlaced = Instantiate(deco.placed, position, decoRot).AddComponent<CellDecorator>();
                    decoPlaced.Init(deco, parent, decoSer.rotation, buildSystem.ProbsLayer);
                    decoPlaced.Occupy(buildSystem);
                }

                return parent;
            }
        }

        [Serializable]
        public class DecoSer
        {
            public int scriptableID;
            public int rotation;

            public DecoSer()
            {
            }

            public DecoSer(CellDecorator source)
            {
                scriptableID = source.Placeable.ID;
                rotation = source.Rotation;
            }
        }
    }
}