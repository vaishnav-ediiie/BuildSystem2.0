using System;
using System.Collections.Generic;
using System.Diagnostics;
using CustomBuildSystem.Example;
using CustomBuildSystem.PhotonIntegration;
using CustomBuildSystem.Placed;
using CustomBuildSystem.Placing;
using UnityEngine;
using CustomGridSystem;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;


namespace CustomBuildSystem
{
    [DefaultExecutionOrder(-123)]
    public class BuildSystem : MonoBehaviour
    {
        public static Dictionary<int, PlaceableMonoBase> AllPlaceableData;
        
        [Header("References"), SerializeField, Tooltip("We build from the perspective of this player")] 
        internal Transform player;

        [SerializeField, Tooltip("We raycast through this camera to find where player is looking")]
        internal Camera playerCamera;

        [Header("Settings"), SerializeField, Tooltip("Everything that we build will be placed on this layer. (Must be a single layer)")]
        internal LayerMask ProbsLayer;

        [SerializeField, Tooltip("Cell number of LastCell in the plot. How many cells player has, to build the house")]
        private CellNumber lastCellNumber = CellNumber.One;

        [SerializeField, Tooltip("Size of a single cell in Unity units")]
        private Vector2 cellSize = Vector2.one;

        [SerializeField, Tooltip("The position of BottomLeft Corner of the plot. Click the green-frustum-button on the axis-navigator (the 3D preview of axes) in scene view)")]
        private Vector3 anchorPosition = Vector3.zero;

        [SerializeField, Tooltip("The differance in Y-coordinate of any two floors")]
        private float floorGap = 2f;
        
        [SerializeField, Tooltip("Maximum distance (in Cells) between player's current cell and the cell that player can place on")]
        internal int playerFOV = 1;

        [SerializeField, Tooltip("All the probes will be spawned as child of this gameobject")]
        internal Transform probesParent;

        private BuiltSystemState currentState;
        private Dictionary<int, DuoPlaceGrid<CellOccupantMono, EdgeOccupantMono>> allGrids;
        internal DuoPlaceGrid<CellOccupantMono, EdgeOccupantMono> gridCurrent;
        
        public BuildBrainBase Brain { get; private set; }
        public DuoPlaceGrid<CellOccupantMono, EdgeOccupantMono> GridCurrent => gridCurrent;
        public Transform Player => player;
        public Camera PlayerCamera => playerCamera;
        public int CurrentFloor { get; private set; }
        public int CurrentFloorByPlayerPos => (int)((player.position.y - anchorPosition.y) / floorGap);


        private void Awake()
        {
            SwitchState<BSS_Idle>();
            allGrids = new Dictionary<int, DuoPlaceGrid<CellOccupantMono, EdgeOccupantMono>>();
            if (Brain == null) Brain = new BuildBrainBase();
        }

        private void Start()
        {
            if (!ProbsLayer.IsSingleLayer()) Debug.LogError("ProbsLayer LayerMask contains multiple layers, make sure there's only one selected.");
            if (AllPlaceableData == null || AllPlaceableData.Count == 0) Debug.LogError("AllPlaceableData in build system brain is empty, Save & Load wont work.");
            
            LoadFloor(CurrentFloorByPlayerPos);
            BuildEvents.Call_BuildSystemCreated(this);
            BuildEvents.Call_GridUpdate();
            
            
        }
        
        private void Update()
        {
            if (CurrentFloorByPlayerPos != CurrentFloor)
            {
                LoadFloor(CurrentFloorByPlayerPos);
            }

            if (Brain.ShouldEnterDeleteMove(currentState))
            {
                CancelBuild(true);
                SwitchState<BSS_Deleting>();
                return;
            }

            currentState.OnUpdate();
        }

        private void OnDestroy()
        {
            BuildEvents.Call_BuildSystemDestroyed(this);
        }
        
        private void LoadFloor(int floor)
        {
            if (allGrids.ContainsKey(floor))
            {
                gridCurrent = allGrids[floor];
            }
            else
            {
                float ypos = (floor * floorGap) + anchorPosition.y;
                gridCurrent = new DuoPlaceGrid<CellOccupantMono, EdgeOccupantMono>(lastCellNumber, cellSize, new Vector2(anchorPosition.x, anchorPosition.z), ypos);
                allGrids.Add(floor, gridCurrent);
            }
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
        
        public void UseBrain<T>(T brainInstance) where T : BuildBrainBase
        {
            if (brainInstance == null)
            {
                Debug.LogError("Given BrainInstance is null");
                return;
            }

            this.Brain = brainInstance;
        }
        
        public void StartBuild(EdgePlaceable edgePlaceable)
        {
            CancelBuild(false);
            if (edgePlaceable.isDecorator) SwitchState<BSS_PlacingEdgeDecorator>().Setup(edgePlaceable);
            else                             SwitchState<BSS_PlacingEdge>().Setup(edgePlaceable);
        }

        public void StartBuild(CellPlaceable cellPlaceable)
        {
            CancelBuild(false);
            if (cellPlaceable.isDecorator)   SwitchState<BSS_PlacingCellDecorator>().Setup(cellPlaceable);
            else                             SwitchState<BSS_PlacingCell>().Setup(cellPlaceable);
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
            if (activeType == typeof(BSS_PlacingCellDecorator)) ((BSS_PlacingCellDecorator)currentState).CancelPlacement();
            else if (activeType == typeof(BSS_PlacingEdgeDecorator)) ((BSS_PlacingEdgeDecorator)currentState).CancelPlacement();
            else if (activeType == typeof(BSS_PlacingCell)) ((BSS_PlacingCell)currentState).CancelPlacement();
            else if (activeType == typeof(BSS_PlacingEdge)) ((BSS_PlacingEdge)currentState).CancelPlacement();
            else if (switchToIdle && activeType != typeof(BSS_Idle))
            {
                SwitchState<BSS_Idle>();
            }
        }

        public void EnterDeleteMode()
        {
            CancelBuild(false);
            SwitchState<BSS_Deleting>();
        }
        
        public string Serialize()
        {
            Dictionary<int, string> allFloorsData = new Dictionary<int, string>();
            foreach (KeyValuePair<int,DuoPlaceGrid<CellOccupantMono,EdgeOccupantMono>> duoPlaceGrid in allGrids)
            {
                string data = duoPlaceGrid.Value.SerializeWithOccupants(
                    cellOccupantSerializer: cellPlaceable => JsonConvert.SerializeObject(new CellOccupantMono.Serializer(cellPlaceable)),
                    edgeOccupantSerializer: edgePlaceable => JsonConvert.SerializeObject(new EdgeOccupantMono.Serializer(edgePlaceable))
                );
                
                allFloorsData.Add(duoPlaceGrid.Key, data);
            }

            return JsonConvert.SerializeObject(allFloorsData);
        }

        public void Deserialize(string data)
        {
            Dictionary<int, string> allFloorsData = JsonConvert.DeserializeObject<Dictionary<int, string>>(data);
            allGrids = new Dictionary<int, DuoPlaceGrid<CellOccupantMono, EdgeOccupantMono>>();

            foreach (KeyValuePair<int,string> floorInfo in allFloorsData)
            {
                gridCurrent = new DuoPlaceGrid<CellOccupantMono, EdgeOccupantMono>();
                gridCurrent.DeserializeWithOccupants(floorInfo.Value,
                    cellOccupantDeserializer: cellData => CellOccupantMono.Serializer.Deserialize(JsonConvert.DeserializeObject<CellOccupantMono.Serializer>(cellData), this),
                    edgeOccupantDeserializer: edgeData => EdgeOccupantMono.Serializer.Deserialize(JsonConvert.DeserializeObject<EdgeOccupantMono.Serializer>(edgeData), this)
                );
                allGrids.Add(floorInfo.Key, gridCurrent);
            }
            LoadFloor(CurrentFloorByPlayerPos);
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
        public static BuildSystem Setup(Transform player, Camera playerCamera, int probLayer, CellNumber lastCellNumber, Vector2 cellSize, Vector3 anchorPosition, float floorGap)
        {
            BuildSystem buildSystem = new GameObject("Build System").AddComponent<BuildSystem>();
            buildSystem.Brain = new BuildBrainBase();
            buildSystem.player = player;
            buildSystem.playerCamera = playerCamera;
            buildSystem.ProbsLayer = probLayer;
            buildSystem.lastCellNumber = lastCellNumber;
            buildSystem.cellSize = cellSize;
            buildSystem.anchorPosition = anchorPosition;
            buildSystem.floorGap = floorGap;

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
            bool scaleVisualsByCellSize = true
            )
        {
            BuildSystem system = Setup(player, playerCamera, probLayer, lastCellNumber, cellSize, anchorPosition, floorGap);
            BuildSystemVisuals visuals = new GameObject("BuildSystemVisuals").AddComponent<BuildSystemVisuals>();
            visuals.transform.parent = system.transform;
            visuals.Setup(cellVisuals, edgeVisuals, displayCellNumber, scaleVisualsByCellSize);
            return system;
        }

        public DuoPlaceGrid<CellOccupantMono, EdgeOccupantMono> GetGrid(int floorNumber)
        {
            if (allGrids.ContainsKey(floorNumber)) return allGrids[floorNumber];
            return null;
        }
    }
}