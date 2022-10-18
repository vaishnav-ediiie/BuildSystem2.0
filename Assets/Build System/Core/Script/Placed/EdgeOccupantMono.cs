using System;
using System.Collections.Generic;
using CustomBuildSystem.Placing;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem.Placed
{
    public class EdgeOccupantMono : OccupantBaseMono
    {
        public EdgePlaceable Placeable { get; private set; }
        public EdgeNumber Number { get; private set; }
        public int Rotation { get; private set; }
        public int Floor { get; private set; }
        [NonSerialized] public List<EdgeDecorator> Decorators;

        public void Init(EdgePlaceable placeable, EdgeNumber number, int rotation, int floorNumber, LayerMask layer)
        {
            this.Placeable = placeable;
            this.Number = number;
            this.Rotation = rotation;
            this.Floor = floorNumber;
            this.Decorators = new List<EdgeDecorator>();
            this.gameObject.SetLayerRecursive(layer.GetLayer());
        }

        public override GameObject GetDeletePrefab() => Placeable.placingError;

        public override void Occupy(BuildSystem buildSystem)
        {
            foreach (EdgeNumber edgeNumber in Placeable.LoopAllEdges(Number))
            {
                buildSystem.gridCurrent.OccupyEdge(edgeNumber, this);
            }
        }

        public override void UnOccupy(BuildSystem buildSystem)
        {
            foreach (EdgeNumber edgeNumber in Placeable.LoopAllEdges(Number))
            {
                buildSystem.gridCurrent.EmptyEdge(edgeNumber);
            }
        }

        public override int PlaceableID => Placeable.ID;
        public override int FloorNumber => this.Floor;

        public override bool HasDecorator(PlaceableMonoBase placeable)
        {
            foreach (EdgeDecorator decorator in Decorators)
            {
                if (decorator.Placeable == placeable) return true;
            }

            return false;
        }

        public override IEnumerable<OccupantBaseMono> Children
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
            public int placeableID;
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

            public Serializer(EdgeOccupantMono source)
            {
                this.placeableID = source.Placeable.ID;
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

            public static EdgeOccupantMono Deserialize(Serializer serializer, BuildSystem buildSystem)
            {
                EdgeNumber cellNumber = new EdgeNumber(serializer.row, serializer.column, serializer.EdgyType);
                Vector3 position = buildSystem.gridCurrent.EdgeNumberToPosition(cellNumber);
                Quaternion rotation = Quaternion.Euler(0, serializer.rotation, 0);
                EdgePlaceable placeable = BuildSystem.AllPlaceableData[serializer.placeableID] as EdgePlaceable;
                
                EdgeOccupantMono parent = Instantiate(placeable.placed, position, rotation).AddComponent<EdgeOccupantMono>();
                parent.Init(placeable, cellNumber, serializer.rotation, serializer.floorNumber, buildSystem.ProbsLayer);
                parent.Occupy(buildSystem);

                foreach (DecoSer decoSer in serializer.decorators)
                {
                    Quaternion decoRot = Quaternion.Euler(0, decoSer.rotation, 0);
                    EdgePlaceable deco = BuildSystem.AllPlaceableData[decoSer.placeableID] as EdgePlaceable;
                    EdgeDecorator decoPlaced = Instantiate(deco.placed, position, decoRot).AddComponent<EdgeDecorator>();
                    decoPlaced.Init(deco, parent, decoSer.rotation, buildSystem.ProbsLayer);
                    decoPlaced.Occupy(buildSystem);
                }

                return parent;
            }
        }

        [Serializable]
        public class DecoSer
        {
            public int placeableID;
            public int rotation;

            public DecoSer()
            {
            }

            public DecoSer(EdgeDecorator source)
            {
                placeableID = source.Placeable.ID;
                rotation = source.Rotation;
            }
        }
    }
}