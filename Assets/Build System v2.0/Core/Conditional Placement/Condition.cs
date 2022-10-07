using System;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    [Serializable]
    public class Condition : ISerializationCallbackReceiver
    {
        public ConditionType conditionType;
        public PlaceableSOBase occupant;

        public static Condition CenterCellCondition => new Condition() { conditionType = ConditionType.MustBeEmpty };
        
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
                default: throw new NotImplementedException($"The Condition {conditionType} is not implemented");
            }
        }

        private bool OccupiedBySpecific(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, CellNumber cellNumber)
        {
            CellPlaceable placeable = grid.GetCellOccupant(cellNumber, null);
            return placeable != null && placeable.Scriptable != occupant;
        }
        
        private bool OccupiedBySpecific(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, EdgeNumber edgeNumber)
        {
            EdgePlaceable placeable = grid.GetEdgeOccupant(edgeNumber, null);
            return placeable != null && placeable.Scriptable != occupant;
        }
        
        
        private bool OccupiedBySpecific(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, CellNumber cell, Direction directions)
        {
            EdgePlaceable placeable = grid.GetEdgeOccupant(cell.GetEdgeNumber(directions), null);
            return placeable != null && placeable.Scriptable != occupant;
        }
        
        /*
        public bool IsViolated(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, CellNumber cellNumber)
        {
            if (!grid.IsCellNumberValid(cellNumber)) return true;
            switch (conditionType)
            {
                case ConditionType.DontCare: return false;
                case ConditionType.MustBeEmpty: return grid.IsCellOccupied(cellNumber);
                case ConditionType.OccupiedByAny: return !grid.IsCellOccupied(cellNumber);
                case ConditionType.OccupiedBySpecific:
                {
                    CellPlaceable placeable = grid.GetCellOccupant(cellNumber, null);
                    return placeable != null && placeable.Scriptable != occupant;
                }
                default: throw new NotImplementedException($"The Condition {conditionType} is not implemented");
            }
        }
        
        
        public bool IsViolated(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, EdgeNumber edgeNumber)
        {
            if (!grid.IsEdgeNumberValid(edgeNumber)) return true;
            switch (conditionType)
            {
                case ConditionType.DontCare: return false;
                case ConditionType.MustBeEmpty: return grid.IsEdgeOccupied(edgeNumber);
                case ConditionType.OccupiedByAny: return !grid.IsEdgeOccupied(edgeNumber);
                case ConditionType.OccupiedBySpecific:
                {
                    EdgePlaceable placeable = grid.GetEdgeOccupant(edgeNumber, null);
                    return placeable != null && placeable.Scriptable != occupant;
                }
                default: throw new NotImplementedException($"The Condition {conditionType} is not implemented");
            }
        }

      
        public bool IsViolated(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, CellNumber cell, Direction directions)
        {
            EdgeNumber edgeNumber = cell.GetEdgeNumber(directions);
            if (!grid.IsEdgeNumberValid(edgeNumber)) return true;

            switch (conditionType)
            {
                case ConditionType.DontCare: return false;
                case ConditionType.MustBeEmpty: return grid.IsEdgeOccupied(edgeNumber);
                case ConditionType.OccupiedByAny: return !grid.IsEdgeOccupied(edgeNumber);
                case ConditionType.OccupiedBySpecific:
                {
                    EdgePlaceable placeable = grid.GetEdgeOccupant(edgeNumber, null);
                    return placeable != null && placeable.Scriptable != occupant;
                }
                default: throw new NotImplementedException($"The Condition {conditionType} is not implemented");
            }
        }*/

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            Init();
        }

    }
}