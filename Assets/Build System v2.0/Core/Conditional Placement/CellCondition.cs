using System;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    [Serializable]
    public class CellCondition : ISerializationCallbackReceiver
    {
        public ConditionType conditionType;
        public PlaceableSOBase occupant;

        public static CellCondition CenterCondition => new CellCondition() { conditionType = ConditionType.MustBeEmpty };
        
        /// <summary>Checks if the condition is violated on the given cells</summary>
        /// <returns>true is the condition is not met</returns>
        /// <exception cref="NotImplementedException">When the condition is not implemented</exception>
        [NonSerialized] public Func<DuoPlaceGrid<CellPlaceable, EdgePlaceable>, CellNumber, bool> HasViolatedCell;
        
        /// <summary>Checks if the condition is violated on the edge</summary>
        /// <returns>true is the condition is not met</returns>
        /// <exception cref="NotImplementedException">When the condition is not implemented</exception>
        [NonSerialized] public Func<DuoPlaceGrid<CellPlaceable, EdgePlaceable>, EdgeNumber, bool> HasViolatedEdge;
        
        /// <summary>Checks if the condition is violated on the edge that is on the cell and specified by direction</summary>
        /// <returns>true is the condition is not met</returns>
        /// <exception cref="NotImplementedException">When the condition is not implemented</exception>
        [NonSerialized] public Func<DuoPlaceGrid<CellPlaceable, EdgePlaceable>, CellNumber, Direction, bool> HasViolatedCD;

        private void Init()
        {
            switch (conditionType)
            {
                case ConditionType.DontCare:
                {
                    HasViolatedCell = (grid, cell) => !grid.IsCellNumberValid(cell);
                    HasViolatedEdge = (grid, edge) => !grid.IsEdgeNumberValid(edge);
                    HasViolatedCD = (grid, cell, dir) => !grid.IsEdgeNumberValid(cell.GetEdgeNumber(dir));
                    break;
                }
                case ConditionType.MustBeEmpty:
                {
                    HasViolatedCell = (grid, cell) => grid.IsCellOccupied(cell);
                    HasViolatedEdge = (grid, edge) => grid.IsEdgeOccupied(edge);
                    HasViolatedCD = (grid, cell, dir) => grid.IsEdgeOccupied(cell.GetEdgeNumber(dir));
                    break;
                }
                case ConditionType.OccupiedByAny:
                {
                    HasViolatedCell = (grid, cell) => !grid.IsCellOccupied(cell);
                    HasViolatedEdge = (grid, edge) => !grid.IsEdgeOccupied(edge);
                    HasViolatedCD = (grid, cell, dir) => !grid.IsEdgeOccupied(cell.GetEdgeNumber(dir));
                    break;
                }
                case ConditionType.OccupiedBySpecific:
                {
                    HasViolatedCell = OccupiedBySpecific;
                    HasViolatedEdge = OccupiedBySpecific;
                    HasViolatedCD = OccupiedBySpecific;
                    break;
                }
                case ConditionType.NotOccupiedBySpecific:
                {
                    HasViolatedCell = NotOccupiedBySpecific;
                    HasViolatedEdge = NotOccupiedBySpecific;
                    HasViolatedCD = NotOccupiedBySpecific;
                    break;
                }
                default: throw new NotImplementedException($"The Condition {conditionType} is not implemented");
            }
        }

        private bool OccupiedBySpecific(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, CellNumber cellNumber)
        {
            CellPlaceable placeable = grid.GetCellOccupant(cellNumber, null);
            return !(placeable != null && placeable.Scriptable == occupant);
        }
        
        private bool OccupiedBySpecific(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, EdgeNumber edgeNumber)
        {
            EdgePlaceable placeable = grid.GetEdgeOccupant(edgeNumber, null);
            return !(placeable != null && placeable.Scriptable == occupant);
        }
        
        private bool OccupiedBySpecific(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, CellNumber cell, Direction directions)
        {
            EdgePlaceable placeable = grid.GetEdgeOccupant(cell.GetEdgeNumber(directions), null);
            return !(placeable != null && placeable.Scriptable == occupant);
        }
        
        private bool NotOccupiedBySpecific(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, CellNumber cellNumber)
        {
            CellPlaceable placeable = grid.GetCellOccupant(cellNumber, null);
            return !(placeable == null || placeable.Scriptable != occupant);
        }
        
        private bool NotOccupiedBySpecific(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, EdgeNumber edgeNumber)
        {
            EdgePlaceable placeable = grid.GetEdgeOccupant(edgeNumber, null);
            return !(placeable == null || placeable.Scriptable != occupant);
        }
        
        private bool NotOccupiedBySpecific(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, CellNumber cell, Direction directions)
        {
            EdgePlaceable placeable = grid.GetEdgeOccupant(cell.GetEdgeNumber(directions), null);
            return !(placeable == null || placeable.Scriptable != occupant);
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