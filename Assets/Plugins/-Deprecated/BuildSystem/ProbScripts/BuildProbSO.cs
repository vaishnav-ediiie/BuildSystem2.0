using System;
using UnityEngine;
using KSRecs.Deprecated.GridSystemSpace;

namespace KSRecs.Deprecated.BuildSystemSpace
{
    [CreateAssetMenu(fileName = "BuildProb", menuName = "Scriptable Objects/BuildSystemProb", order = 1)]
    public class BuildProbSO : ScriptableObject
    {
        /// <summary>Called when user starts putting any prob on grid</summary>
        public static event Action<BuildProbSO> OnBuildInit;

        /// <summary>Called when user a confirms placement of prob</summary>
        public static event Action<BuildProbSO> OnBuildConfirm;

        /// <summary>Called when user a cancels placement of prob</summary>
        public static event Action<BuildProbSO> OnBuildCancel;

        /// <summary>Called when user a confirms or cancels placement of prob or </summary>
        public static event Action<BuildProbSO> OnBuildOver;

        enum PlacementType
        {
            Center,
            Edge
        }

        private static readonly CellNumber ONE = CellNumber.one;

        [SerializeField] private GameObject prefabPlacing;
        [SerializeField] private GameObject prefabPlaced;
        [SerializeField] private PlacementType placementType;
        [SerializeField] internal bool scaleWithCellSize;


        [SerializeField,
         // DvDrawIf(nameof(placementType), PlacementType.Center, DvDrawIfAttribute.DisablingType.ReadOnly),
         Tooltip("Number of cells this object occupies")]
        internal CellNumber cellsNeeded = CellNumber.one;

        [SerializeField,
         // DvDrawIf(nameof(placementType), PlacementType.Center, DvDrawIfAttribute.DisablingType.ReadOnly),
         Tooltip("Center offset ofr central cell")]
        internal CellNumber centerCell = CellNumber.zero;

        internal GameObject CurrentInstance;
        private ProbState probState;

        private void UpdateState()
        {
            if (placementType == PlacementType.Edge) probState = new ProbEdgePlacement();
            else if (cellsNeeded == ONE) probState = new ProbCenterPlaceSingleCell();
            else probState = new ProbCenterPlaceMultiCell();
            probState.Init(this);
        }

        public void InitNew()
        {
            if (CurrentInstance != null) CancelBuild();

            CurrentInstance = Instantiate(prefabPlacing);
            CurrentInstance.transform.rotation = Quaternion.identity;
            OnBuildInit?.Invoke(this);
            UpdateState();
        }

        public void MovePreviewTo(Vector3 curCellPos, Vector3 lookAtCellPos, PlayerCompForBuildSystem player)
        {
            Vector3 directionVector = lookAtCellPos - curCellPos;
            directionVector.Normalize();
            directionVector *= GridSystem.Instance.CellSize.sqrMagnitude;

            CellNumber ccn = GridSystem.Instance.GetCellNumber(curCellPos);
            CellNumber lcn = GridSystem.Instance.GetCellNumber(curCellPos + directionVector);

            lcn.column = ccn.column + Mathf.Clamp(lcn.column - ccn.column, -1, 1);
            lcn.row = ccn.row + Mathf.Clamp(lcn.row - ccn.row, -1, 1);
            probState.MovePreviewTo(ccn, lcn, player);
        }

        public void MovePreviewTo(CellNumber curCellNum, CellNumber lookAtCellNum, PlayerCompForBuildSystem player)
        {
            if (!CurrentInstance) return;
            probState.MovePreviewTo(curCellNum, lookAtCellNum, player);
        }

        public void ConfirmBuild(int floorNumber)
        {
            if (!CurrentInstance)
            {
                Debug.Log("ConfirmBuild Empty");
                return;
            }

            Transform ci = this.CurrentInstance.transform;
            
            this.CurrentInstance = Instantiate(prefabPlaced, ci.position, ci.rotation, BuildSystem.Instance.transform);
            this.CurrentInstance.transform.localScale = ci.localScale; 
            this.CurrentInstance.layer = LayerMask.NameToLayer(BuildSystem.Instance.probsLayerName);
            this.CurrentInstance.AddComponent<LiveProb>().ParentProb = this;
            probState.ConfirmBuild(floorNumber);

            Destroy(ci.gameObject);
            this.CurrentInstance = null;

            OnBuildConfirm?.Invoke(this);
            OnBuildOver?.Invoke(this);
        }

        public void CancelBuild()
        {
            if (!CurrentInstance) return;

            Destroy(CurrentInstance);
            CurrentInstance = null;
            OnBuildCancel?.Invoke(this);
            OnBuildOver?.Invoke(this);
        }
    }
}