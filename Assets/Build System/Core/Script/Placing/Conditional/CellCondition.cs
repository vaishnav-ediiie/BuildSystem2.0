using System;
using System.Collections.Generic;
using CustomBuildSystem.Placed;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem.Placing.Conditional
{
    [Serializable]
    public class CellCondition : ICondition, ISerializationCallbackReceiver
    {
        public ConditionType type;
        public CellConditionPlace place;
        public PlaceableMonoBase[] occupants;

        /// <summary>Checks if the condition is violated on the given cells</summary>
        /// <returns>true is the condition is not met</returns>
        /// <exception cref="NotImplementedException">When the condition is not implemented</exception>
        private Func<BuildSystem, CellNumber, bool> HasViolatedCell;

        /// <summary>Checks if the condition is violated on the edge</summary>
        /// <returns>true is the condition is not met</returns>
        /// <exception cref="NotImplementedException">When the condition is not implemented</exception>
        private Func<BuildSystem, EdgeNumber, bool> HasViolatedEdge;

        private Func<CellLayoutInfo, IEnumerable<CellNumber>> LoopCells;
        private Func<CellLayoutInfo, IEnumerable<EdgeNumber>> LoopEdges;
        private bool isCellType;

        private HashSet<int> occupantsIds;

        public CellCondition(ConditionType type, CellConditionPlace place, int floorNumber, bool isFloorRelative = true, bool invertCondition = false)
        {
            if (type == ConditionType.OccupiedBySpecific)
            {
                throw new Exception("Use other overload of initializer for specifying occupants");
            }

            this.type = type;
            this.place = place;
            this.floorNumber = floorNumber;
            this.isFloorRelative = isFloorRelative;
            this.invertCondition = invertCondition;
            Init();
        }

        public CellCondition(CellConditionPlace place, PlaceableMonoBase[] occupants, int floorNumber, bool isFloorRelative = true, bool invertCondition = false)
        {
            this.type = ConditionType.OccupiedBySpecific;
            this.place = place;
            this.occupants = occupants;
            this.floorNumber = floorNumber;
            this.isFloorRelative = isFloorRelative;
            this.invertCondition = invertCondition;
            Init();
        }

        private void Init()
        {
            switch (place)
            {
                case CellConditionPlace.Centers:
                    LoopCells = (ly => ly.LoopCenterCells());
                    isCellType = true;
                    break;
                case CellConditionPlace.Sides:
                    LoopCells = (ly => ly.LoopOuterSideCell());
                    isCellType = true;
                    break;
                case CellConditionPlace.Corners:
                    LoopCells = (ly => ly.LoopOuterCornerCells());
                    isCellType = true;
                    break;
                case CellConditionPlace.EdgeBoundaries:
                    LoopEdges = (ly => ly.LoopBorderEdges());
                    isCellType = false;
                    break;
                case CellConditionPlace.EdgeBetween:
                    LoopEdges = (ly => ly.LoopInnerEdges());
                    isCellType = false;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            switch (type)
            {
                case ConditionType.MustBeEmpty:
                {
                    if (isCellType)
                    {
                        HasViolatedCell = (buildSystem, cell) =>
                        {
                            if (GetGrid(buildSystem, out var grid)) return !outputWhenFloorDontExist;
                            return !grid.IsCellNumberValid(cell) || grid.IsCellOccupied(cell);
                        };
                    }
                    else
                    {
                        HasViolatedEdge = (buildSystem, edge) =>
                        {
                            if (GetGrid(buildSystem, out var grid)) return !outputWhenFloorDontExist;
                            return !grid.IsEdgeNumberValid(edge) || grid.IsEdgeOccupied(edge);
                        };
                    }

                    break;
                }
                case ConditionType.OccupiedByAny:
                {
                    if (isCellType)
                    {
                        HasViolatedCell = (buildSystem, cell) =>
                        {
                            if (GetGrid(buildSystem, out var grid)) return !outputWhenFloorDontExist;
                            return !grid.IsCellNumberValid(cell) || !grid.IsCellOccupied(cell);
                        };
                    }
                    else
                    {
                        HasViolatedEdge = (buildSystem, edge) =>
                        {
                            if (GetGrid(buildSystem, out var grid)) return !outputWhenFloorDontExist;
                            return !grid.IsEdgeNumberValid(edge) || !grid.IsEdgeOccupied(edge);
                        };
                    }

                    break;
                }
                case ConditionType.OccupiedBySpecific:
                {
                    AssignOccupantsIds();
                    if (isCellType)
                    {
                        HasViolatedCell = (buildSystem, cell) =>
                        {
                            if (GetGrid(buildSystem, out var grid)) return !outputWhenFloorDontExist;
                            CellOccupantMono occupantMono = grid.GetCellOccupant(cell, null);
                            return !grid.IsCellNumberValid(cell) || (occupantMono == null) || !occupantsIds.Contains(occupantMono.Placeable.ID);
                        };
                    }
                    else
                    {
                        HasViolatedEdge = (buildSystem, edge) =>
                        {
                            if (GetGrid(buildSystem, out var grid)) return !outputWhenFloorDontExist;
                            EdgeOccupantMono occupantMono = grid.GetEdgeOccupant(edge, null);
                            return !grid.IsEdgeNumberValid(edge) || (occupantMono == null) || !occupantsIds.Contains(occupantMono.Placeable.ID);
                        };
                    }

                    break;
                }
                case ConditionType.NotOccupiedBySpecific:
                {
                    AssignOccupantsIds();
                    if (isCellType)
                    {
                        HasViolatedCell = (buildSystem, cell) =>
                        {
                            if (GetGrid(buildSystem, out var grid)) return !outputWhenFloorDontExist;
                            CellOccupantMono occupantMono = grid.GetCellOccupant(cell, null);
                            return !grid.IsCellNumberValid(cell) || !((occupantMono == null) || !occupantsIds.Contains(occupantMono.Placeable.ID));
                        };
                    }
                    else
                    {
                        HasViolatedEdge = (buildSystem, edge) =>
                        {
                            if (GetGrid(buildSystem, out var grid)) return !outputWhenFloorDontExist;
                            EdgeOccupantMono occupantMono = grid.GetEdgeOccupant(edge, null);
                            return !grid.IsEdgeNumberValid(edge) || !(occupantMono == null || !occupantsIds.Contains(occupantMono.Placeable.ID));
                        };
                    }

                    break;
                }
                default: throw new NotImplementedException($"The Condition {type} is not implemented");
            }
        }

        private void AssignOccupantsIds()
        {
            occupantsIds = new HashSet<int>();
            foreach (PlaceableMonoBase occupant in occupants)
            {
                occupantsIds.Add(occupant.ID); // We don't need to check if multiple placeable(s) are having same ID as its already checked when build system starts. 
            }
        }

        public bool HasViolatedCD(BuildSystem buildSystem, CellNumber cell, Direction direction) => HasViolatedEdge(buildSystem, cell.GetEdgeNumber(direction));

        public bool HasViolated(BuildSystem buildSystem, CellLayoutInfo layoutInfo)
        {
            if (isCellType)
            {
                foreach (CellNumber cellNumber in LoopCells(layoutInfo))
                {
                    if (HasViolatedCell(buildSystem, cellNumber)) 
                        return !invertCondition; // If invert condition is false then we would return true here (as in condition has been violated)
                }
                return invertCondition;
            }


            foreach (EdgeNumber edgeNumber in LoopEdges(layoutInfo))
            {
                if (HasViolatedEdge(buildSystem, edgeNumber)) 
                    return !invertCondition; // If invert condition is false then we would return true here (as in condition has been violated)
            }

            return invertCondition;
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            Init();
        }
    }
}