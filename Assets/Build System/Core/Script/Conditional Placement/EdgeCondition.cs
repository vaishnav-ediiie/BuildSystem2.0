using System;
using CustomGridSystem;
using UnityEngine;

namespace CustomBuildSystem
{
    [Serializable]
    public class EdgeCondition : ISerializationCallbackReceiver
    {
        public ConditionType conditionType;
        public PlaceableSOBase occupant;

        public static EdgeCondition CenterCondition => new EdgeCondition() { conditionType = ConditionType.MustBeEmpty };
        
        /// <summary>Checks if the condition is violated on the given edges</summary>
        /// <returns>true is the condition is not met</returns>
        /// <exception cref="NotImplementedException">When the condition is not implemented</exception>
        [NonSerialized] public Func<DuoPlaceGrid<CellPlaceable, EdgePlaceable>, EdgeNumber, bool> HasViolated;
        
        private void Init()
        {
            switch (conditionType)
            {
                case ConditionType.DontCare:
                    HasViolated = (grid, edge) => !grid.IsEdgeNumberValid(edge);
                    break;
                case ConditionType.MustBeEmpty:
                    HasViolated = (grid, edge) => grid.IsEdgeOccupied(edge);
                    break;
                case ConditionType.OccupiedByAny:
                    HasViolated = (grid, edge) => !grid.IsEdgeOccupied(edge);
                    break;
                case ConditionType.OccupiedBySpecific:
                    HasViolated = OccupiedBySpecific;
                    break;
                case ConditionType.NotOccupiedBySpecific:
                    HasViolated = NotOccupiedBySpecific;
                    break;
                default: throw new NotImplementedException($"The Condition {conditionType} is not implemented");
            }
        }

        private bool OccupiedBySpecific(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, EdgeNumber edgeNumber)
        {
            EdgePlaceable placeable = grid.GetEdgeOccupant(edgeNumber, null);
            return !(placeable != null && placeable.Scriptable == occupant);
        }

        private bool NotOccupiedBySpecific(DuoPlaceGrid<CellPlaceable, EdgePlaceable> grid, EdgeNumber edgeNumber)
        {
            EdgePlaceable placeable = grid.GetEdgeOccupant(edgeNumber, null);
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