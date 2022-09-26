using UnityEngine;
using KSRecs.Deprecated.GridSystemSpace;

namespace KSRecs.Deprecated.BuildSystemSpace
{
    public abstract class ProbState
    {
        protected BuildProbSO parent;

        internal virtual void Init(BuildProbSO parent) => this.parent = parent;
        internal abstract void MovePreviewTo(CellNumber curCellNum, CellNumber lookAtCellNum, PlayerCompForBuildSystem player);
        internal abstract void ConfirmBuild(int floorNumber);
    }
}