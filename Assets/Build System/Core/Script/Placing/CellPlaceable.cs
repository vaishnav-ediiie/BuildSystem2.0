using System.Collections.Generic;
using CustomBuildSystem.Placing.Conditional;
using UnityEngine;
using CellNumber = CustomGridSystem.CellNumber;

namespace CustomBuildSystem.Placing
{
    public class CellPlaceable : PlaceableMonoBase
    {
        [SerializeField] private CellNumber cellNeeded = CellNumber.One;
        [SerializeField] private CellNumber centerCell = CellNumber.Zero;
        [SerializeField] private CombineMode criteriaCombineMode;
        [SerializeField] private List<CellCondition> placementCriteria;
        
        public CellLayoutInfo LayoutInfo { get; private set; }

        public void Awake()
        {
            LayoutInfo = new CellLayoutInfo(this.centerCell, cellNeeded);
        }

        public bool AreBaseConditionsSatisfied(BSS_PlacingCell placingCell)
        {
            LayoutInfo.Refresh(placingCell.CellNumber, placingCell.Rotation);
            if (criteriaCombineMode == CombineMode.And)
            {
                foreach (CellCondition condition in placementCriteria)
                {
                    if (condition.HasViolated(placingCell.BuildSystem, LayoutInfo))
                        return false;
                }
                return true;
            }
            
            foreach (CellCondition condition in placementCriteria)
            {
                if (!condition.HasViolated(placingCell.BuildSystem, LayoutInfo))
                    return true;
            }
            return false;
        }

        public bool HasCondition(ConditionType type, CellConditionPlace place, int floorNumber, bool relativeFloor, bool invertCondition)
        {
            foreach (CellCondition criterion in placementCriteria)
            {
                if (criterion.type == type
                    && criterion.place == place
                    && criterion.floorNumber == floorNumber
                    && criterion.isFloorRelative == relativeFloor
                    && criterion.invertCondition == invertCondition
                   )
                {
                    return true;
                }
            }

            return false;
        }

        public void AddCondition(CellCondition condition)
        {
            placementCriteria.Add(condition);
        }

        private void Reset()
        {
            this.placementCriteria = new List<CellCondition>()
            {
                new CellCondition(ConditionType.MustBeEmpty, CellConditionPlace.Centers, floorNumber: 0, isFloorRelative: true, invertCondition: false)
            };
        }
        
    }
    
}