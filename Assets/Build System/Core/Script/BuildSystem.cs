using System;
using System.Collections.Generic;
using System.Diagnostics;
using CustomBuildSystem.PhotonIntegration;
using UnityEngine;
using CustomGridSystem;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;


namespace CustomBuildSystem
{
    [DefaultExecutionOrder(-123)]
    public class BuildSystem : MonoBehaviour
    {
        [Header("References"), Tooltip("We build from the perspective of this player")] [SerializeField]
        internal Transform player;

        [SerializeField, Tooltip("We raycast through this camera to find where player is looking")]
        internal Camera playerCamera;

        [Header("Settings")] [SerializeField, Tooltip("Everything that we build will be placed on this layer. (Must be a single layer)")]
        internal LayerMask ProbsLayer;

        [SerializeField, Tooltip("Cell number of LastCell in the plot. How many cells player has, to build the house")]
        private CellNumber lastCellNumber = CellNumber.One;

        [SerializeField, Tooltip("Size of a single cell in Unity units")]
        private Vector2 cellSize = Vector2.one;

        [SerializeField, Tooltip("The position of BottomLeft Corner of the plot. Click the green-frustum-button on the axis-navigator (the 3D preview of axes) in scene view)")]
        private Vector3 anchorPosition = Vector3.zero;

        [SerializeField, Tooltip("The differance in Y-coordinate of any two floors")]
        private float floorGap = 2f;

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


        private void Awake()
        {
            SwitchState<BSS_Idle>();
            allGrids = new Dictionary<int, DuoPlaceGrid<CellPlaceable, EdgePlaceable>>();
            Brain = new BuildSystemBrain();
        }

        private void Start()
        {
            if (!ProbsLayer.IsSingleLayer()) Debug.LogError("ProbsLayer LayerMask contains multiple layers, make sure there's only one selected.");
            if (Brain.AllPlaceableData == null || Brain.AllPlaceableData.Count == 0) Debug.LogError("AllPlaceableData in build system brain is empty, Save & Load wont work.");
            
            LoadFloor(0);
            Brain.Call_GridUpdate();
            
        }

        public void UseBrain<T>(T brainInstance, bool copyPlaceableDataFromLastBrain = false) where T : BuildSystemBrain
        {
            if (brainInstance == null)
            {
                Debug.LogError("Given BrainInstance is null");
                return;
            }

            if (copyPlaceableDataFromLastBrain)
            {
                if (Brain.AllPlaceableData == null || Brain.AllPlaceableData.Count == 0)
                {
                    Debug.LogError("AllPlaceableData in build system brain is empty, Save & Load wont work.");
                }
            }
            else if (brainInstance.AllPlaceableData == null || brainInstance.AllPlaceableData.Count == 0)
            {
                Debug.LogError("AllPlaceableData in build system brain is empty, Save & Load wont work.");
            }

            brainInstance.CopyFrom(this.Brain, copyPlaceableDataFromLastBrain);
            this.Brain = brainInstance;
            if (this.gridCurrent != null) this.Brain.Call_GridUpdate();
        }

        void Update()
        {
            int floor = (int)((player.position.y - anchorPosition.y) / floorGap);
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
                gridCurrent = new DuoPlaceGrid<CellPlaceable, EdgePlaceable>(lastCellNumber, cellSize, new Vector2(anchorPosition.x, anchorPosition.z), (floor * floorGap) + anchorPosition.y);
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

        public void StartBuild(EdgePlaceableSO edgePlaceableSo)
        {
            CancelBuild(false);
            if (edgePlaceableSo.isDecorator) SwitchState<BSS_PlacingEdgeDecorator>().Setup(edgePlaceableSo);
            else                             SwitchState<BSS_PlacingEdge>().Setup(edgePlaceableSo);
        }

        public void StartBuild(CellPlaceableSO cellPlaceableSo)
        {
            CancelBuild(false);
            if (cellPlaceableSo.isDecorator) SwitchState<BSS_PlacingCellDecorator>().Setup(cellPlaceableSo);
            else                             SwitchState<BSS_PlacingCell>().Setup(cellPlaceableSo);
        }

        public void ConfirmBuild()
        {
            Type activeType = currentState.GetType();
            if (activeType == typeof(BSS_PlacingCellDecorator)) ((BSS_PlacingCellDecorator)currentState).ConfirmPlacement();
            else if (activeType == typeof(BSS_PlacingEdgeDecorator)) ((BSS_PlacingEdgeDecorator)currentState).ConfirmPlacement();
            else if (activeType == typeof(BSS_PlacingCell)) ((BSS_PlacingCell)currentState).ConfirmPlacement();
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

        public string Serialize()
        {
            return gridCurrent.SerializeWithOccupants(
                cellOccupantSerializer: cellPlaceable => JsonConvert.SerializeObject(new CellPlaceable.Serializer(cellPlaceable)),
                edgeOccupantSerializer: edgePlaceable => JsonConvert.SerializeObject(new EdgePlaceable.Serializer(edgePlaceable))
            );
        }

        public void Deserialize(string data)
        {
            gridCurrent.DeserializeWithOccupants(data,
                cellOccupantDeserializer: cellData => CellPlaceable.Serializer.Deserialize(JsonConvert.DeserializeObject<CellPlaceable.Serializer>(cellData), this),
                edgeOccupantDeserializer: edgeData => EdgePlaceable.Serializer.Deserialize(JsonConvert.DeserializeObject<EdgePlaceable.Serializer>(edgeData), this)
            );
        }

       
        /// <summary> Create build system with given parameters (without grid visuals)</summary>
        /// <param name="player">We build from the perspective of this player</param>
        /// <param name="playerCamera">We raycast through this camera to find where player is looking</param>
        /// <param name="probLayer">Everything that we build will be placed on this layer. (Must be a single layer)</param>
        /// <param name="lastCellNumber">Cell number of LastCell in the plot. How many cells player has, to build the house</param>
        /// <param name="cellSize">Size of a single cell in Unity units</param>
        /// <param name="anchorPosition">The position of BottomLeft Corner of the plot. Click the green-frustum-button on the axis-navigator (the 3D preview of axes) in scene view)</param>
        /// <param name="floorGap">The differance in Y-coordinate of any two floors</param>
        /// <returns>BuildSystem that is created</returns>
        public static BuildSystem Setup(Transform player, Camera playerCamera, int probLayer, CellNumber lastCellNumber, Vector2 cellSize, Vector3 anchorPosition, float floorGap, bool createPhotonHandler = false)
        {
            BuildSystem buildSystem = new GameObject("Build System").AddComponent<BuildSystem>();
            buildSystem.player = player;
            buildSystem.playerCamera = playerCamera;
            buildSystem.ProbsLayer = probLayer;
            buildSystem.lastCellNumber = lastCellNumber;
            buildSystem.cellSize = cellSize;
            buildSystem.anchorPosition = anchorPosition;
            buildSystem.floorGap = floorGap;
            if (createPhotonHandler)
            {
                BuildSystemPhotonHandler photonHandler = new GameObject("BuildSystemPhotonHandler").AddComponent<BuildSystemPhotonHandler>();
                photonHandler.transform.parent = buildSystem.transform;
                photonHandler.buildSystem = buildSystem;
            }

            return buildSystem;
        }

        /// <summary> Create build system with given parameters (with grid visuals)</summary>
        /// <summary> Create build system with given parameters (without grid visuals)</summary>
        /// <param name="player">We build from the perspective of this player</param>
        /// <param name="playerCamera">We raycast through this camera to find where player is looking</param>
        /// <param name="probLayer">Everything that we build will be placed on this layer. (Must be a single layer)</param>
        /// <param name="lastCellNumber">Cell number of LastCell in the plot. How many cells player has, to build the house</param>
        /// <param name="cellSize">Size of a single cell in Unity units</param>
        /// <param name="anchorPosition">The position of BottomLeft Corner of the plot. Click the green-frustum-button on the axis-navigator (the 3D preview of axes) in scene view)</param>
        /// <param name="floorGap">The differance in Y-coordinate of any two floors</param>
        /// <param name="cellVisuals">Prefab which is used to display one cell</param>
        /// <param name="edgeVisuals">Prefab which is used to display one Edge (optional)</param>
        /// <returns>BuildSystem that is created</returns>
        /// <returns>BuildSystem that is created</returns>
        public static BuildSystem Setup(Transform player, Camera playerCamera, int probLayer, CellNumber lastCellNumber, Vector2 cellSize, Vector3 anchorPosition, float floorGap,
            CellVisuals cellVisuals,
            EdgeVisuals edgeVisuals = null,
            bool displayCellNumber = false,
            bool createPhotonHandler = false)
        {
            BuildSystem system = Setup(player, playerCamera, probLayer, lastCellNumber, cellSize, anchorPosition, floorGap, createPhotonHandler);
            BuildSystemVisuals visuals = new GameObject("BuildSystemVisuals").AddComponent<BuildSystemVisuals>();
            visuals.transform.parent = system.transform;
            visuals.Setup(system, cellVisuals, edgeVisuals, displayCellNumber);
            return system;
        }
    }
}