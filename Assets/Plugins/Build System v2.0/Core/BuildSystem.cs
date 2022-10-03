using System;
using UnityEngine;
using CustomGridSystem;


namespace CustomBuildSystem
{
    [DefaultExecutionOrder(-123)]
    public class BuildSystem : MonoBehaviour
    {
        public event Action OnGridUpdated;
        public event Action<EdgePlaceable> OnEdgePlaced;
        public event Action<CellPlaceable> OnCellPlaced;
        [SerializeField] internal Transform player;
        [SerializeField] internal Camera playerCamera;
        [SerializeField] private CellNumber lastCellNumber;
        [SerializeField] Vector2 cellSize;
        [SerializeField] private Vector3 anchorPosition;

        private BuiltSystemState currentState;
        internal DuoPlaceGrid<CellPlaceable, EdgePlaceable> activeGrid;

        public DuoPlaceGrid<CellPlaceable, EdgePlaceable> ActiveGrid => activeGrid;
        public Transform Player => player;
        public Camera PlayerCamera => playerCamera;

        void Start()
        {
            activeGrid = new DuoPlaceGrid<CellPlaceable, EdgePlaceable>(lastCellNumber, cellSize, new Vector2(anchorPosition.x, anchorPosition.z), anchorPosition.y);
            SwitchState<BSS_Idle>();
            currentState.OnStart();
            OnGridUpdated?.Invoke();
        }

        void Update()
        {
            currentState.OnUpdate();
        }
        
        T SwitchState<T>() where T : BuiltSystemState, new()
        {
            currentState = new T();
            currentState.Init(this);
            return currentState as T;
        }

        internal void ConfirmBuildInner(EdgePlaceable placed)
        {
            OnEdgePlaced?.Invoke(placed);
            SwitchState<BSS_Idle>();
        }

        internal void ConfirmBuildInner(CellPlaceable placed)
        {
            OnCellPlaced?.Invoke(placed);
            SwitchState<BSS_Idle>();
        }
        
        public void Setup(CellNumber lastCellNumber, Vector2 cellSize, Vector3 anchorPosition)
        {
            activeGrid = new DuoPlaceGrid<CellPlaceable, EdgePlaceable>(lastCellNumber, cellSize, new Vector2(anchorPosition.x, anchorPosition.z), anchorPosition.y);
            SwitchState<BSS_Idle>();
            OnGridUpdated?.Invoke();
        }

        public void StartBuild(EdgePlaceableSO edgePlaceableSo)
        {
            Type activeType = currentState.GetType();
            if (activeType == typeof(BSS_PlacingEdge) || activeType == typeof(BSS_PlacingCell))
            {
                CancelBuild();
            }

            SwitchState<BSS_PlacingEdge>().Setup(edgePlaceableSo);
        }

        public void StartBuild(CellPlaceableSO cellPlaceableSo)
        {
            SwitchState<BSS_PlacingCell>().Setup(cellPlaceableSo);
        }

        public void ConfirmBuild()
        {
            Type activeType = currentState.GetType();
            if (activeType == typeof(BSS_PlacingCell)) ((BSS_PlacingCell)currentState).ConfirmPlacement();
            else if (activeType == typeof(BSS_PlacingEdge)) ((BSS_PlacingEdge)currentState).ConfirmPlacement();
            SwitchState<BSS_Idle>();
        }
        
        public void CancelBuild()
        {
            Type activeType = currentState.GetType();
            if (activeType == typeof(BSS_PlacingCell)) ((BSS_PlacingCell)currentState).CancelPlacement();
            else if (activeType == typeof(BSS_PlacingEdge)) ((BSS_PlacingEdge)currentState).CancelPlacement();
            SwitchState<BSS_Idle>();
        }
    }
}