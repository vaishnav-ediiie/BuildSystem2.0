using System;
using CustomBuildSystem.Placed;
using CustomBuildSystem.Placing;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    [Serializable]
    public class CellCondition : ISerializationCallbackReceiver
    {
        public ConditionType conditionType;
        public PlaceableMonoBase occupant;

        public static CellCondition CenterCondition => new CellCondition() { conditionType = ConditionType.MustBeEmpty };
        
        /// <summary>Checks if the condition is violated on the given cells</summary>
        /// <returns>true is the condition is not met</returns>
        /// <exception cref="NotImplementedException">When the condition is not implemented</exception>
        [NonSerialized] public Func<DuoPlaceGrid<CellOccupantMono, EdgeOccupantMono>, CellNumber, bool> HasViolatedCell;
        
        /// <summary>Checks if the condition is violated on the edge</summary>
        /// <returns>true is the condition is not met</returns>
        /// <exception cref="NotImplementedException">When the condition is not implemented</exception>
        [NonSerialized] public Func<DuoPlaceGrid<CellOccupantMono, EdgeOccupantMono>, EdgeNumber, bool> HasViolatedEdge;
        

        private void Init()
        {
            switch (conditionType)
            {
                case ConditionType.DontCare:
                {
                    HasViolatedCell = (grid, cell) => false;
                    HasViolatedEdge = (grid, edge) => false;
                    break;
                }
                case ConditionType.MustBeEmpty:
                {
                    HasViolatedCell = (grid, cell) => !grid.IsCellNumberValid(cell) || grid.IsCellOccupied(cell);
                    HasViolatedEdge = (grid, edge) => !grid.IsEdgeNumberValid(edge) || grid.IsEdgeOccupied(edge);
                    break;
                }
                case ConditionType.OccupiedByAny:
                {
                    HasViolatedCell = (grid, cell) => !grid.IsCellNumberValid(cell) && !grid.IsCellOccupied(cell);
                    HasViolatedEdge = (grid, edge) => !grid.IsEdgeNumberValid(edge) && !grid.IsEdgeOccupied(edge);
                    break;
                }
                case ConditionType.OccupiedBySpecific:
                {
                    HasViolatedCell = (grid, cell) =>
                    {
                        CellOccupantMono occupantMono = grid.GetCellOccupant(cell, null);
                        return !(occupantMono != null && occupantMono.Scriptable == occupant);
                    };
                    HasViolatedEdge = (grid, edge) =>
                    {
                        EdgeOccupantMono occupantMono = grid.GetEdgeOccupant(edge, null);
                        return !(occupantMono != null && occupantMono.Scriptable == occupant);
                    };
                    break;
                }
                case ConditionType.NotOccupiedBySpecific:
                {
                    HasViolatedCell = (grid, cell) =>
                    {
                        CellOccupantMono occupantMono = grid.GetCellOccupant(cell, null);
                        return !(occupantMono == null || occupantMono.Scriptable != occupant);
                    };
                    HasViolatedEdge = (grid, edge) =>
                    {
                        EdgeOccupantMono occupantMono = grid.GetEdgeOccupant(edge, null);
                        return !(occupantMono == null || occupantMono.Scriptable != occupant);
                    };
                    break;
                }
                default: throw new NotImplementedException($"The Condition {conditionType} is not implemented");
            }
        }
        
        public bool HasViolatedCD(DuoPlaceGrid<CellOccupantMono, EdgeOccupantMono> grid, CellNumber cell, Direction direction) => HasViolatedEdge(grid, cell.GetEdgeNumber(direction));

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            Init();
        }

        
    }
}