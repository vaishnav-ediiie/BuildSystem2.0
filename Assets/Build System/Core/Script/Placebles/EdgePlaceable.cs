using System;
using System.Collections.Generic;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    public class EdgePlaceable : IMonoPlaceable
    {
        public EdgePlaceableSO Scriptable { get; private set; }
        public EdgeNumber Number { get; private set; }
        public int Rotation { get; private set; }
        public int Floor { get; private set; }
        [NonSerialized] public List<EdgeDecorator> Decorators;

        public void Init(EdgePlaceableSO scriptable, EdgeNumber number, int rotation, int floorNumber, LayerMask layer)
        {
            this.Scriptable = scriptable;
            this.Number = number;
            this.Rotation = rotation;
            this.Floor = floorNumber;
            this.Decorators = new List<EdgeDecorator>();
            this.gameObject.SetLayerRecursive(layer.GetLayer());
        }

        public override GameObject GetDeletePrefab() => Scriptable.placingError;

        public override void Occupy(BuildSystem buildSystem)
        {
            foreach (EdgeNumber edgeNumber in Scriptable.LoopAllEdges(Number))
            {
                buildSystem.gridCurrent.OccupyEdge(edgeNumber, this);
            }
        }

        public override void UnOccupy(BuildSystem buildSystem)
        {
            foreach (EdgeNumber edgeNumber in Scriptable.LoopAllEdges(Number))
            {
                buildSystem.gridCurrent.EmptyEdge(edgeNumber);
            }
        }

        public override int ScriptableID => Scriptable.ID;
        public override int FloorNumber => this.Floor;

        public override bool HasDecorator(PlaceableSOBase scriptable)
        {
            foreach (EdgeDecorator decorator in Decorators)
            {
                if (decorator.Scriptable == scriptable) return true;
            }

            return false;
        }

        public override IEnumerable<IMonoPlaceable> Children
        {
            get
            {
                foreach (EdgeDecorator decorator in Decorators)
                {
                    yield return decorator;
                }
            }
        }

        public void RemoveDecorator(EdgeDecorator deco)
        {
            if (Decorators.Contains(deco))
                this.Decorators.Remove(deco);
        }

        public void AddDecorator(EdgeDecorator deco)
        {
            if (!Decorators.Contains(deco))
                this.Decorators.Add(deco);
        }


        [Serializable]
        public class Serializer
        {
            public int scriptableID;
            public int row;
            public int column;
            public int edgyType;
            public int rotation;
            public int floorNumber;
            public DecoSer[] decorators;

            public EdgeType EdgyType => (EdgeType)edgyType;

            public Serializer()
            {
            }

            public Serializer(EdgePlaceable source)
            {
                this.scriptableID = source.Scriptable.ID;
                this.row = source.Number.CellAfter.row;
                this.column = source.Number.CellAfter.column;
                this.edgyType = (int)source.Number.edgeType;
                this.rotation = source.Rotation;
                this.floorNumber = source.Floor;
                decorators = new DecoSer[source.Decorators.Count];
                int i = 0;
                foreach (EdgeDecorator deco in source.Decorators)
                {
                    decorators[i] = new DecoSer(deco);
                    i++;
                }
            }

            public static EdgePlaceable Deserialize(Serializer serializer, BuildSystem buildSystem)
            {
                EdgeNumber cellNumber = new EdgeNumber(serializer.row, serializer.column, serializer.EdgyType);
                Vector3 position = buildSystem.gridCurrent.EdgeNumberToPosition(cellNumber);
                Quaternion rotation = Quaternion.Euler(0, serializer.rotation, 0);
                EdgePlaceableSO placeableSo = buildSystem.Brain.AllPlaceableData[serializer.scriptableID] as EdgePlaceableSO;
                
                EdgePlaceable parent = Instantiate(placeableSo.placed, position, rotation).AddComponent<EdgePlaceable>();
                parent.Init(placeableSo, cellNumber, serializer.rotation, serializer.floorNumber, buildSystem.ProbsLayer);
                parent.Occupy(buildSystem);

                foreach (DecoSer decoSer in serializer.decorators)
                {
                    Quaternion decoRot = Quaternion.Euler(0, decoSer.rotation, 0);
                    EdgePlaceableSO decoSO = buildSystem.Brain.AllPlaceableData[decoSer.scriptableID] as EdgePlaceableSO;
                    EdgeDecorator decoPlaced = Instantiate(decoSO.placed, position, decoRot).AddComponent<EdgeDecorator>();
                    decoPlaced.Init(decoSO, parent, decoSer.rotation, buildSystem.ProbsLayer);
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

            public DecoSer(EdgeDecorator source)
            {
                scriptableID = source.Scriptable.ID;
                rotation = source.Rotation;
            }
        }
    }
}