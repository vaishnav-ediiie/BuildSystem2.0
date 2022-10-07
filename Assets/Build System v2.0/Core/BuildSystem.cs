using System;
using System.Collections.Generic;
using UnityEngine;
using CustomGridSystem;
using DebugToScreen;


namespace CustomBuildSystem
{
    [DefaultExecutionOrder(-123)]
    public class BuildSystem : MonoBehaviour
    {
        // TODO: Remove This. BuildSystem should not and is not a Singleton. If you find this, means I forgot to remove it.
        public static BuildSystem Instance;
        
        [Header("References")] [SerializeField]
        internal Transform player;

        [SerializeField] internal Camera playerCamera;

        [Header("Settings")] [SerializeField] internal LayerMask ProbsLayer;
        [SerializeField] private CellNumber lastCellNumber;
        [SerializeField] private Vector2 cellSize;
        [SerializeField] private Vector3 anchorPosition;
        [SerializeField] private float groundFloorHeight;
        [SerializeField] private float floorGap;

        private BuiltSystemState currentState;
        private Dictionary<int, DuoPlaceGrid<CellPlaceable, EdgePlaceable>> allGrids;
        internal DuoPlaceGrid<CellPlaceable, EdgePlaceable> gridCurrent;
        internal DuoPlaceGrid<CellPlaceable, EdgePlaceable> gridBelow;
        internal DuoPlaceGrid<CellPlaceable, EdgePlaceable> gridAbove;
       
        public BuildSystemBrain Brain { get; private set; }

        public DuoPlaceGrid<CellPlaceable, EdgePlaceable> GridCurrent => gridCurrent;
        public Transform Player => player;
        public Camera PlayerCamera => playerCamera;
        public int CurrentFloor { get; private set; }
        public Transform Cursor;


        private void Awake()
        {
            Instance = this;
            SwitchState<BSS_Idle>();
            allGrids = new Dictionary<int, DuoPlaceGrid<CellPlaceable, EdgePlaceable>>();
            Brain = new BuildSystemBrain();
            if (!ProbsLayer.IsSingleLayer())
            {
                Debug.LogError("ProbsLayer LayerMask contains multiple layers, make sure there's only one selected.");
            }
            LoadFloor(0);
        }

        private void Start()
        {
            GameDebug.StartMyInfo(Instance, "Build System", true);
            Brain.Call_GridUpdate();
        }

        public void UseBrain<T>(T brainInstance) where T : BuildSystemBrain
        {
            if (brainInstance == null)
            {
                Debug.LogError("Given BrainInstance is null");
                return;
            }

            brainInstance.CopyEvents(this.Brain);
            this.Brain = brainInstance;
            this.Brain.Call_GridUpdate();
        }

        void Update()
        {
            int floor = (int)((player.position.y - groundFloorHeight) / floorGap);
            if (floor != CurrentFloor)
            {
                LoadFloor(floor);
            }

            if (Brain.ShouldEnterDeleteMove(currentState))
            {
                CancelBuild(true);
                SwitchState<BSS_Deleting>();
                return;
            }
            
            currentState.OnUpdate();
        }

        private void LoadFloor(int floor)
        {
            if (allGrids.ContainsKey(floor))
            {
                gridCurrent = allGrids[floor];
            }
            else
            {
                gridCurrent = new DuoPlaceGrid<CellPlaceable, EdgePlaceable>(lastCellNumber, cellSize, new Vector2(anchorPosition.x, anchorPosition.z), (floor * floorGap) + groundFloorHeight);
                allGrids.Add(floor, gridCurrent);
            }
            
            gridBelow = allGrids.ContainsKey(floor - 1) ? allGrids[floor - 1] : null;
            gridAbove = allGrids.ContainsKey(floor + 1) ? allGrids[floor + 1] : null;
            CurrentFloor = floor;
        }

        internal T SwitchState<T>() where T : BuiltSystemState, new()
        {
            if (currentState != null) currentState.OnExit();
            
            currentState = new T();
            currentState.Init(this);
            currentState.OnEnter();
            
            return currentState as T;
        }

        public void Setup(CellNumber lastCellNumber, Vector2 cellSize, Vector3 anchorPosition)
        {
            gridCurrent = new DuoPlaceGrid<CellPlaceable, EdgePlaceable>(lastCellNumber, cellSize, new Vector2(anchorPosition.x, anchorPosition.z), anchorPosition.y);
            SwitchState<BSS_Idle>();
            this.Brain.Call_GridUpdate();
        }

        public void StartBuild(EdgePlaceableSO edgePlaceableSo)
        {
            CancelBuild(false);
            SwitchState<BSS_PlacingEdge>().Setup(edgePlaceableSo);
        }

        public void StartBuild(CellPlaceableSO cellPlaceableSo)
        {
            CancelBuild(false);
            SwitchState<BSS_PlacingCell>().Setup(cellPlaceableSo);
        }

        public void ConfirmBuild()
        {
            Type activeType = currentState.GetType();
            if (activeType == typeof(BSS_PlacingCell)) ((BSS_PlacingCell)currentState).ConfirmPlacement();
            else if (activeType == typeof(BSS_PlacingEdge)) ((BSS_PlacingEdge)currentState).ConfirmPlacement();
        }
        
        
        /// <summary>
        /// Cancel the Placement of currently active Placeable.
        /// </summary>
        /// <param name="switchToIdle">If true and no placeable is being placed, then switch to Idle State</param>
        public void CancelBuild(bool switchToIdle)
        {
            Type activeType = currentState.GetType();
            if (activeType == typeof(BSS_PlacingCell)) ((BSS_PlacingCell)currentState).CancelPlacement();
            else if (activeType == typeof(BSS_PlacingEdge)) ((BSS_PlacingEdge)currentState).CancelPlacement();
            else if (switchToIdle && activeType != typeof(BSS_Idle))
            {
                SwitchState<BSS_Idle>();
            }
        }
        
    }
}