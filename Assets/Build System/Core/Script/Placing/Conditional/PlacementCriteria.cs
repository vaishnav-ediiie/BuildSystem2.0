using System;
using System.Collections;
using System.Collections.Generic;
using CustomGridSystem;
using UnityEngine;


namespace CustomBuildSystem.Placing.Conditional
{
    /*[Serializable]
    public class CellPlacementCriteria : IEnumerable<CellCondition> 
    {
        [SerializeField] private List<CellCondition> conditions;

        public bool AreSatisfied(BuildSystem buildSystem, CellLayoutInfo layoutInfo)
        {
            foreach (CellCondition condition in conditions)
            {
                condition.HasViolated(buildSystem, layoutInfo);
            }

            return true;
        }

        public void Add(CellCondition condition)
        {
            conditions.Add(condition);
        }

        public IEnumerator<CellCondition> GetEnumerator() => conditions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()        => conditions.GetEnumerator();
    }*/
    
    
    /*
    [Serializable]
    public class EdgePlacementCriteria : IEnumerable<EdgeCondition> 
    {

        public bool AreSatisfied(BuildSystem buildSystem, EdgeNumber current, EdgePlaceable layoutInfo)
        { 
            foreach (EdgeCondition condition in conditions)
            {
                foreach (EdgeNumber edge in layoutInfo.LoopAllEdges(current))
                {
                    if (condition.HasViolated(buildSystem, edge)) return false;
                }
            }

            return true;
        }

        public void Add(EdgeCondition condition)
        {
            conditions.Add(condition);
        }

        public IEnumerator<EdgeCondition> GetEnumerator() => conditions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()        => conditions.GetEnumerator();
    }*/
}