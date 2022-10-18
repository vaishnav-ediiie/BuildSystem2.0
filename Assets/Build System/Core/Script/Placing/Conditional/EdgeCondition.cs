using System;
using System.Collections.Generic;
using System.Linq;
using CustomBuildSystem.Placed;
using CustomGridSystem;
using DebugToScreen;
using UnityEngine;

namespace CustomBuildSystem.Placing.Conditional
{
    [Serializable]
    public class EdgeCondition : ICondition, ISerializationCallbackReceiver
    {
        public ConditionType type;
        public PlaceableMonoBase[] occupants;
        public bool invertCondition;
        

        
        private HashSet<int> occupantsIds;


        /// <summary>Checks if the condition is violated on the given edges</summary>
        /// <returns>true is the condition is not met</returns>
        /// <exception cref="NotImplementedException">When the condition is not implemented</exception>
        [NonSerialized] public Func<BuildSystem, EdgeNumber, bool> HasViolated;


        public EdgeCondition(ConditionType type, int floorNumber, bool isFloorRelative = true, bool invertCondition = false)
        {
            if (type == ConditionType.OccupiedBySpecific)
            {
                throw new Exception("Use other overload of initializer for specifying occupants");
            }
            
            this.type = type;
            this.floorNumber = floorNumber;
            this.isFloorRelative = isFloorRelative;
            this.invertCondition = invertCondition;
            Init();
        }

        public EdgeCondition(PlaceableMonoBase[] occupants, int floorNumber, bool isFloorRelative = true, bool invertCondition = false)
        {
            this.type = ConditionType.OccupiedBySpecific;
            this.occupants = occupants;
            this.floorNumber = floorNumber;
            this.isFloorRelative = isFloorRelative;
            this.invertCondition = invertCondition;
            
            Init();
        }


        private void Init()
        {
            switch (type)
            {
                case ConditionType.MustBeEmpty: 
                {
                    HasViolated = (buildSystem, edge) =>
                    {
                        if (GetGrid(buildSystem, out var grid)) return true;
                        return !grid.IsEdgeNumberValid(edge) || grid.IsEdgeOccupied(edge);
                    };
                    break;
                }
                case ConditionType.OccupiedByAny:
                {
                    HasViolated = (buildSystem, edge) =>
                    {
                        if (GetGrid(buildSystem, out var grid)) return true;
                        return !grid.IsEdgeNumberValid(edge) || !grid.IsEdgeOccupied(edge);
                    };

                    break;
                }
                case ConditionType.OccupiedBySpecific:
                {
                    HasViolated = (buildSystem, edge) =>
                    {
                        if (GetGrid(buildSystem, out var grid)) return true;
                        EdgeOccupantMono occupantMono = grid.GetEdgeOccupant(edge, null);
                        return !grid.IsEdgeNumberValid(edge) || (occupantMono == null) || !occupantsIds.Contains(occupantMono.Placeable.ID);
                    };
                    break;
                }
                case ConditionType.NotOccupiedBySpecific:
                {
                    HasViolated = (buildSystem, edge) =>
                    {
                        if (GetGrid(buildSystem, out var grid)) return true;
                        EdgeOccupantMono occupantMono = grid.GetEdgeOccupant(edge, null);
                        return !grid.IsEdgeNumberValid(edge) || !(occupantMono == null || !occupantsIds.Contains(occupantMono.Placeable.ID));
                    };
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
                occupantsIds.Add(occupant.ID);      // We don't need to check if multiple placeable(s) are having same ID as its already checked when build system starts. 
            }
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