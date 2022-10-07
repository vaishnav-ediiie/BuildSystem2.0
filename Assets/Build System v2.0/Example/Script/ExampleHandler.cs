using System.Collections.Generic;
using UnityEngine;

namespace CustomBuildSystem.Example
{
    public class ExampleHandler : MonoBehaviour
    {
        [SerializeField] protected BuildSystem buildSystem;
        [SerializeField] protected Canvas canvas;
        [SerializeField] private KeyCode quitKeyCode;
        [SerializeField] protected SelectItemPopup selectionPopup;
        [SerializeField] internal EdgePlaceableSO[] edgePlaceables;
        [SerializeField] internal CellPlaceableSO[] cellPlaceables;

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            buildSystem.UseBrain(new ExampleBrain());
            buildSystem.Brain.OnCellStateChanged += OnCellStateChanged;
            buildSystem.Brain.OnEdgeStateChanged += OnEdgeStateChanged;
        }

        private void OnEdgeStateChanged(BSS_PlacingEdge placeable, PlacingStage newState)
        {
            if (newState == PlacingStage.Placed) buildSystem.StartBuild(placeable.PlaceableSo);
        }

        private void OnCellStateChanged(BSS_PlacingCell placeable, PlacingStage newState)
        {
            if (newState == PlacingStage.Placed) buildSystem.StartBuild(placeable.PlaceableSo);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                // placingEdge = !placingEdge;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Instantiate(selectionPopup, canvas.transform)
                    .Init(edgePlaceables, cellPlaceables, UpdatePlacingState);
            }

            if (Input.GetKey(KeyCode.Tab)) return;

            if (Input.GetKeyDown(quitKeyCode))
            {
                buildSystem.CancelBuild(switchToIdle: true);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                string data = buildSystem.Serialize();
                PlayerPrefs.SetString("___Data___", data);
                Debug.Log(data);
            }
            
            if (Input.GetKeyDown(KeyCode.L))
            {
                Dictionary<int, PlaceableSOBase> allPlaceableData = new Dictionary<int, PlaceableSOBase>();
                foreach (EdgePlaceableSO so in edgePlaceables)
                {
                    Debug.Log($"Added ID: {so.ID}");
                    allPlaceableData.Add(so.ID, so);
                }
                foreach (CellPlaceableSO so in cellPlaceables)
                {
                    Debug.Log($"Added ID: {so.ID}");
                    allPlaceableData.Add(so.ID, so);
                }
                buildSystem.Deserialize(PlayerPrefs.GetString("___Data___"), allPlaceableData);
            }
            
        }

        private void UpdatePlacingState(EdgePlaceableSO edge, CellPlaceableSO cell)
        {
            if (edge != null)
            {
                buildSystem.CancelBuild(true);
                buildSystem.StartBuild(edge);
            }
            else if (cell != null)
            {
                buildSystem.CancelBuild(true);
                buildSystem.StartBuild(cell);
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}